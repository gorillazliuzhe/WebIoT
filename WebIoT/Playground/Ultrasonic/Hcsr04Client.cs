using Microsoft.Extensions.Options;
using System;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebIoT.Models;
using WebIoT.Tools;

namespace WebIoT.Playground
{
    public class Hcsr04Client : IHcsr04Client
    {
        private readonly int _echo;
        private readonly int _trigger;
        private int _lastMeasurment = 0;       
        public const int NoObstacleDistance = -1;
        private readonly object _locker = new object();
        private readonly GpioController _controller;
        private readonly Stopwatch _timer = new Stopwatch();
        public event EventHandler<Hcsr04ReadEventArgs> OnDataAvailable;

        public bool IsRunning { get; set; }

        public Hcsr04Client(IOptions<SiteConfig> option, GpioController controller)
        {
            _echo = option.Value.EchoPin;
            _trigger = option.Value.TriggerPin;
            _controller = controller;
        }

        public void Start()
        {
            lock (_locker)
            {
                IsRunning = true;
                if (!_controller.IsPinOpen(_echo))
                    _controller.OpenPin(_echo, PinMode.Input);

                if (!_controller.IsPinOpen(_trigger))
                {
                    _controller.OpenPin(_trigger, PinMode.Output);
                    _controller.Write(_trigger, PinValue.Low);
                }
                Task.Run(() => PerformContinuousReads());
            }
        }
        public void Stop()
        {
            lock (_locker)
            {
                IsRunning = false;
                if (_controller.IsPinOpen(_trigger))
                    _controller.ClosePin(_trigger);
                if (_controller.IsPinOpen(_echo))
                    _controller.ClosePin(_echo);
            }
        }
        private void PerformContinuousReads()
        {
            while (IsRunning)
            {
                var sensorData = RetrieveSensorData();

                if (!IsRunning) continue;
                OnDataAvailable?.Invoke(this, sensorData);
                Thread.Sleep(200);
            }
        }
        private Hcsr04ReadEventArgs RetrieveSensorData()
        {
            try
            {
                _timer.Reset();

                // Trigger input for 10uS to start ranging
                // ref https://components101.com/sites/default/files/component_datasheet/HCSR04%20Datasheet.pdf
                while (Environment.TickCount - _lastMeasurment < 60)
                {
                    Thread.Sleep(TimeSpan.FromMilliseconds(Environment.TickCount - _lastMeasurment));
                }

                _controller.Write(_trigger, PinValue.High);     // 上高电平
                Thread.Sleep(TimeSpan.FromMilliseconds(0.01));  // 持续一段时间,发出足够的脉冲
                _controller.Write(_trigger, PinValue.Low);      // 设置低电平


                if (!GpioEX.WaitForValue(_controller, _echo, PinValue.Low)) // 等待低电平结束,记录时间
                    throw new TimeoutException();

                _lastMeasurment = Environment.TickCount;

                _timer.Start();

                if (!GpioEX.WaitForValue(_controller, _echo, PinValue.High)) // 等待高电平结束,记录时间
                    throw new TimeoutException();

                _timer.Stop();

                TimeSpan elapsed = _timer.Elapsed;

                // distance = (time / 2) × velocity of sound (34300 cm/s)
                var distance = elapsed.TotalMilliseconds / 2.0 * 34.3;

                return new Hcsr04ReadEventArgs(distance);

            }
            catch
            {
                return Hcsr04ReadEventArgs.CreateInvalidReading();
            }

        }
    }
}
