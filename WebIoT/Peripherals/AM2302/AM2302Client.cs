using Swan.Diagnostics;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;

namespace WebIoT.Peripherals.AM2302
{
    public class AM2302Client : IDisposable
    {
        private const long BitPulseMidMicroseconds = 60; // 位脉冲微妙时间 (26 ... 28)µs for false; (29 ... 70)µs for true

        private readonly IGpioPin _dataPin = Pi.Gpio[BcmPin.Gpio17];
        private readonly Timer _readTimer;
        private bool _disposedValue;
        private int _period;     
        public AM2302Client()
        {            
            _readTimer = new Timer(PerformContinuousReads, null, Timeout.Infinite, Timeout.Infinite);
        }

        /// <summary>
        /// 当来自传感器的数据可用时执行
        /// </summary>
        public event EventHandler<DhtReadEventArgs> OnDataAvailable;

        /// <summary>
        /// 获取一个值，该值指示传感器是否正在运行.
        /// </summary>
        public bool IsRunning { get; private set; }


        /// <summary>
        /// 获取下拉微秒以开始通信.
        /// </summary>
        protected uint PullDownMicroseconds { get; set; } = 1300;

        /// <summary>
        /// 以2秒的最小时间间隔开始读取传感器.
        /// </summary>
        public void Start() => Start(2);

        /// <summary>
        /// 开始读取传感器.
        /// </summary>
        /// <param name="period">两次读取调用之间的时间间隔，以秒为单位。最小时间间隔必须为2秒.</param>
        /// <exception cref="InvalidOperationException">周期不得少于2秒.</exception>
        public void Start(int period)
        {
            if (period < 2)
                throw new InvalidOperationException("周期不得少于2秒.");

            _period = period * 1000;
            IsRunning = true;
            _readTimer.Change(0, Timeout.Infinite);
        }

        /// <summary>
        /// 停止读取.
        /// </summary>
        public void Stop() => StopContinuousReads();

        public void Dispose() => Dispose(true);

        /// <summary>
        /// 释放非托管和（可选）托管资源.
        /// </summary>
        /// <param name="disposing"><c>true</c> 释放托管和非托管资源；<c> false </ c> 仅释放非托管资源.</param>
        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                if (IsRunning)
                    StopContinuousReads();

                _readTimer.Dispose();
            }

            _disposedValue = true;
        }

        ///<summary>
        /// 解码温度.
        /// </summary>
        /// <param name="data">数据.</param>
        /// <returns>代表当前温度的值.</returns>
        protected double DecodeHumidity(byte[] data) =>
            ((data[0] << 8) | data[1]) * 0.1;

        /// <summary>
        /// 解码湿度
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>代表当前湿度的值</returns>
        protected double DecodeTemperature(byte[] data)
        {
            var temp = (((data[2] & 0x7F) << 8) | data[3]) * 0.1;
            if ((data[2] & 0x80) == 0x80)
                temp *= -1;

            return temp;
        }

        /// <summary>
        /// 读取传感器.
        /// </summary>
        private void PerformContinuousReads(object state)
        {
            if (!IsRunning)
                return;

            try
            {
                // Acquire measure
                var sensorData = RetrieveSensorData();

                if (IsRunning)
                {
                    OnDataAvailable?.Invoke(this, sensorData);
                    _readTimer.Change(_period, Timeout.Infinite);
                }
            }
            catch
            {
                // swallow
            }
        }

        /// <summary>
        /// 返回传感器数据.
        /// </summary>
        /// <returns>从传感器读取的事件参数.</returns>
        private DhtReadEventArgs RetrieveSensorData()
        {
            // 准备缓冲区以存储度量和校验和
            var data = new byte[5];

            // 开始与传感器通信
            // 通知传感器必须完成最后一次执行并将其状态置于空闲状态
            _dataPin.PinMode = GpioPinDriveMode.Output;

            // 发送请求以将传输从板传输到传感器
            _dataPin.Write(GpioPinValue.Low);
            Pi.Timing.SleepMicroseconds(PullDownMicroseconds);
            _dataPin.Write(GpioPinValue.High);

            // 等待传感器响应
            _dataPin.PinMode = GpioPinDriveMode.Input;

            try
            {
                // 读取传感器的确认
                if (!_dataPin.WaitForValue(GpioPinValue.Low, 50))
                    throw new TimeoutException();

                if (!_dataPin.WaitForValue(GpioPinValue.High, 50))
                    throw new TimeoutException();

                // 开始数据传输
                if (!_dataPin.WaitForValue(GpioPinValue.Low, 50))
                    throw new TimeoutException();

                // 读取40位以获取:
                //   16 bit -> Humidity (湿度)
                //   16 bit -> Temperature (温度)
                //   8 bit -> Checksum (校验和)
                var stopwatch = new HighResolutionTimer();

                for (var i = 0; i < 40; i++)
                {
                    stopwatch.Reset();
                    if (!_dataPin.WaitForValue(GpioPinValue.High, 50))
                        throw new TimeoutException();

                    stopwatch.Start();
                    if (!_dataPin.WaitForValue(GpioPinValue.Low, 50))
                        throw new TimeoutException();

                    stopwatch.Stop();

                    data[i / 8] <<= 1;

                    // 检查信号是1还是0
                    if (stopwatch.ElapsedMicroseconds > BitPulseMidMicroseconds)
                        data[i / 8] |= 1;
                }

                // 结束传输数据
                if (!_dataPin.WaitForValue(GpioPinValue.High, 50))
                    throw new TimeoutException();

                // 完成校验
                return IsDataValid(data) ?
                        new DhtReadEventArgs(
                            humidityPercentage: DecodeHumidity(data),
                            temperatureCelsius: DecodeTemperature(data)) :
                        DhtReadEventArgs.CreateInvalidReading();
            }
            catch
            {
                return DhtReadEventArgs.CreateInvalidReading();
            }
        }

        private static bool IsDataValid(byte[] data) =>((data[0] + data[1] + data[2] + data[3]) & 0xff) == data[4];

        /// <summary>
        /// 终止读取.
        /// </summary>
        private void StopContinuousReads()
        {
            _readTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsRunning = false;
        }
    }
}
