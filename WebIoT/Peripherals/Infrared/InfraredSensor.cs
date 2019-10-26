namespace WebIoT.Peripherals.Infrared
{
    using Swan;
    using Swan.Diagnostics;
    using System;
    using System.Collections.Generic;
    using System.Text;
    using System.Threading;
    using Unosquare.RaspberryIO.Abstractions;

    /// <summary>
    /// Implements a digital infrared sensor using the HX1838/VS1838 or the TSOP38238 38kHz digital receiver.
    /// It registers an interrupt on the pin and fires data events asynchronously to keep CPU usage low.
    /// Some primitive ideas taken from https://github.com/adafruit/IR-Commander/blob/master/ircommander.pde
    /// and https://github.com/z3t0/Arduino-IRremote/blob/master/IRremote.cpp.
    /// </summary>
    public sealed partial class InfraredSensor : IDisposable
    {
        private volatile bool _isDisposed; // To detect redundant calls 呼叫冗余
        private volatile bool _isInReadInterrupt; // 是否读取中断
        private volatile bool _currentValue;
        private Timer _idleChecker; // 空闲检查器

        /// <summary>
        /// Initializes a new instance of the <see cref="InfraredSensor" /> class.
        /// </summary>
        /// <param name="inputPin">The input pin.</param>
        /// <param name="isActiveLow">if set to <c>true</c> [is active low].</param>
        public InfraredSensor(IGpioPin inputPin, bool isActiveLow)
        {
            IsActiveLow = isActiveLow;
            InputPin = inputPin;
            ReadInterruptDoWork();
        }

        /// <summary>
        /// Occurs when a single sensor pulse is available.
        /// 当单个传感器脉冲可用时发生.
        /// </summary>
        public event EventHandler<InfraredSensorPulseEventArgs> PulseAvailable;

        /// <summary>
        /// Occurs when a data buffer is available.
        /// 当数据缓冲区可用时发生.
        /// </summary>
        public event EventHandler<InfraredSensorDataEventArgs> DataAvailable;

        /// <summary>
        /// Enumerates the different reasons why the receiver flushed the buffer.
        /// 列举了接收方刷新缓冲区的不同原因.
        /// </summary>
        public enum ReceiverFlushReason
        {
            /// <summary>
            /// The idle state
            /// 空闲状态
            /// </summary>
            Idle,

            /// <summary>
            /// The overflow state
            /// 溢出状态
            /// </summary>
            Overflow,
        }

        /// <summary>
        /// Gets the input pin.
        /// 获取输入引脚.
        /// </summary>
        public IGpioPin InputPin { get; }

        /// <summary>
        /// Gets a value indicating whether the sensor is active low.
        /// 获取一个值，该值指示传感器是否处于低电平有效状态.
        /// </summary>
        public bool IsActiveLow { get; }

        /// <summary>
        /// Creates a string representing all the pulses passed to this method.
        /// 创建一个字符串，表示传递给此方法的所有脉冲.
        /// </summary>
        /// <param name="pulses">The pulses. 脉冲</param>
        /// <param name="groupSize">Number of pulse data to output per line.每行输出的脉冲数据数.</param>
        /// <returns>A string representing the pulses.代表脉冲的字符串.</returns>
        public static string DebugPulses(InfraredPulse[] pulses, int groupSize = 4)
        {
            var builder = new StringBuilder(pulses.Length * 24);
            builder.AppendLine();

            for (var i = 0; i < pulses.Length; i += groupSize)
            {
                for (var offset = 0; offset < groupSize; offset++)
                {
                    if (i + offset >= pulses.Length)
                        break;

                    var p = pulses[i + offset];
                    builder.Append(p == null ? $" ? {-1,7} | " : $" {(p.Value ? "T" : "F")} {p.DurationUsecs,7} | ");
                }

                builder.AppendLine();
            }

            return builder.ToString();
        }

        /// <inheritdoc />
        public void Dispose() => Dispose(true);

        /// <summary>
        /// Releases unmanaged and - optionally - managed resources.
        /// 释放非托管和（可选）托管资源.
        /// </summary>
        /// <param name="alsoManaged"><c>true</c> to release both managed and unmanaged resources; <c>false</c> to release only unmanaged resources.</param>
        private void Dispose(bool alsoManaged)
        {
            if (_isDisposed) return;

            _isDisposed = true;
            if (alsoManaged)
            {
                // Dispose of Managed objects
                _idleChecker.Dispose();
            }
        }

        /// <summary>
        /// Reads the interrupt do work.
        /// 读取中断即可正常工作.
        /// </summary>
        private void ReadInterruptDoWork()
        {
            // Define some constants
            const long gapUsecs = 5000; // 缺口用例
            const long maxElapsedMicroseconds = 250000;
            const long minElapsedMicroseconds = 50;
            const int idleCheckIntervalMilliSecs = 32; // 空闲检查间隔微妙
            const int maxPulseCount = 128; // 最大脉冲数

            // Setup the input pin
            InputPin.PinMode = GpioPinDriveMode.Input;
            InputPin.InputPullMode = GpioPinResistorPullMode.PullUp;

            // Get the timers started!
            var pulseTimer = new HighResolutionTimer(); // 脉冲时间
            var idleTimer = new HighResolutionTimer();  // 空闲时间

            var pulseBuffer = new List<InfraredPulse>(maxPulseCount);
            var syncLock = new object();

            _idleChecker = new Timer(s =>
            {
                if (_isDisposed || _isInReadInterrupt)
                    return;

                lock (syncLock)
                {
                    if (idleTimer.ElapsedMicroseconds < gapUsecs || idleTimer.IsRunning == false || pulseBuffer.Count <= 0)
                        return;

                    OnInfraredSensorRawDataAvailable(pulseBuffer.ToArray(), ReceiverFlushReason.Idle);
                    pulseBuffer.Clear();
                    idleTimer.Reset();
                }
            });

            InputPin.RegisterInterruptCallback(EdgeDetection.FallingAndRisingEdge, () =>
            {
                if (_isDisposed) return;

                _isInReadInterrupt = true;

                lock (syncLock)
                {
                    idleTimer.Restart();
                    _idleChecker.Change(idleCheckIntervalMilliSecs, idleCheckIntervalMilliSecs);

                    var currentLength = pulseTimer.ElapsedMicroseconds;
                    var pulse = new InfraredPulse(
                        IsActiveLow ? !_currentValue : _currentValue,
                        currentLength.Clamp(minElapsedMicroseconds, maxElapsedMicroseconds));

                    // Restart for the next bit coming in
                    pulseTimer.Restart();

                    // For the next value
                    _currentValue = InputPin.Read();

                    // Do not add an idling pulse
                    if (pulse.DurationUsecs < maxElapsedMicroseconds)
                    {
                        pulseBuffer.Add(pulse);
                        OnInfraredSensorPulseAvailable(pulse);
                    }

                    if (pulseBuffer.Count >= maxPulseCount)
                    {
                        OnInfraredSensorRawDataAvailable(pulseBuffer.ToArray(), ReceiverFlushReason.Overflow);
                        pulseBuffer.Clear();
                    }
                }

                _isInReadInterrupt = false;
            });

            // Get the timers started
            pulseTimer.Start();
            idleTimer.Start();
            _idleChecker.Change(0, idleCheckIntervalMilliSecs);
        }

        /// <summary>
        /// Called when a single infrared sensor pulse becomes available.
        /// </summary>
        /// <param name="pulse">The pulse.</param>
        private void OnInfraredSensorPulseAvailable(InfraredPulse pulse)
        {
            if (_isDisposed || PulseAvailable == null) return;

            var args = new InfraredSensorPulseEventArgs(pulse);
            ThreadPool.QueueUserWorkItem(a =>
            {
                PulseAvailable?.Invoke(this, a as InfraredSensorPulseEventArgs);
            },
            args);
        }

        /// <summary>
        /// Called when an infrared sensor raw data buffer becomes available.
        /// </summary>
        /// <param name="pulses">The pulses.</param>
        /// <param name="state">The state.</param>
        private void OnInfraredSensorRawDataAvailable(InfraredPulse[] pulses, ReceiverFlushReason state)
        {
            if (_isDisposed || DataAvailable == null) return;

            var args = new InfraredSensorDataEventArgs(pulses, state);
            ThreadPool.QueueUserWorkItem(a =>
            {
                DataAvailable?.Invoke(this, a as InfraredSensorDataEventArgs);
            },
            args);
        }
    }
}
