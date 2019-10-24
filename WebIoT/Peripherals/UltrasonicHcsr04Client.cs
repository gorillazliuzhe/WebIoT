using System;
using System.Device.Gpio;
using System.Threading;

namespace WebIoT.Peripherals
{
    /// <summary>
    /// 超声波 HC-SR04 型号模块
    /// </summary>
    public class UltrasonicHcsr04Client : IDisposable
    {
        public const int NoObstacleDistance = -1; // 当未检测到障碍物时报告此值.
        private const long NoObstaclePulseMicroseconds = 32000;
        private bool disposedValue = false;
        private const int TriggerPin = 23; // 控制端
        private const int EchoPin = 24; // 接收端
        private Thread _readWorker;
        private Swan.Diagnostics.HighResolutionTimer _measurementTimer;
        private readonly GpioController _controller = new GpioController();
        public UltrasonicHcsr04Client()
        {
            _readWorker = new Thread(PerformContinuousReads);
        }

        /// <summary>
        /// 当来自传感器的数据可用时发生.
        /// </summary>
        public event EventHandler<UltrasonicReadEventArgs> OnDataAvailable;

        /// <summary>
        /// 获取一个值，该值指示此实例是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 开启传感器
        /// </summary>
        /// <param name="ct"></param>
        /// <returns></returns>
        public void Start()
        {
            _controller.OpenPin(TriggerPin, PinMode.Output);
            _controller.OpenPin(EchoPin, PinMode.Input);
            _measurementTimer = new Swan.Diagnostics.HighResolutionTimer();
            IsRunning = true;
            _readWorker.Start();
        }

        public void Stop() => IsRunning = false;


        private void PerformContinuousReads(object state)
        {
            while (IsRunning)
            {
                var sensorData = RetrieveSensorData();

                if (!IsRunning) continue;
                OnDataAvailable?.Invoke(this, sensorData);
                Thread.Sleep(200);
            }
            Dispose(true);
        }
        private UltrasonicReadEventArgs RetrieveSensorData()
        {
            try
            {
                _controller.Write(TriggerPin, PinValue.Low);
                Unosquare.RaspberryIO.Pi.Timing.SleepMicroseconds(2);
                _controller.Write(TriggerPin, PinValue.High);
                Unosquare.RaspberryIO.Pi.Timing.SleepMicroseconds(12);
                _controller.Write(TriggerPin, PinValue.Low);
                WaitForValue(EchoPin, 0);
                _measurementTimer.Start();
                WaitForValue(EchoPin, 1);
                _measurementTimer.Stop();
                var elapsedTime = _measurementTimer.ElapsedMicroseconds;
                _measurementTimer.Reset();
                var distance = elapsedTime / 58.0;
                if (elapsedTime > NoObstaclePulseMicroseconds)
                    distance = NoObstacleDistance;
                return new UltrasonicReadEventArgs(distance);
            }
            catch
            {
                return UltrasonicReadEventArgs.CreateInvalidReading();
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _controller.Dispose();
                }
                disposedValue = true;
            }
        }

        public void Dispose() => Stop();

        /// <summary>
        /// 等待一个引脚的值
        /// </summary>
        /// <param name="pin">引脚</param>
        /// <param name="status">0:低电平 1:高电平</param>
        /// <param name="timeOutMillisecond">超时时间</param>
        private void WaitForValue(int pin, int status, int timeOutMillisecond = 50)
        {
            var outstime = DateTime.Now.Millisecond;
            do
            {
                var outseime = DateTime.Now.Millisecond;
                if ((outseime - outstime) > timeOutMillisecond)
                {
                    throw new TimeoutException();
                }
            } while (_controller.Read(pin) == status);
        }
    }
}
