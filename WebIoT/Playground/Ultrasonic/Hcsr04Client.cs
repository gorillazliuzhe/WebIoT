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
    public class Hcsr04Client : IHcsr04Client, IDisposable
    {
        private readonly int _echo;
        private readonly int _trigger;
        private int _lastMeasurment = 0;
        private readonly GpioController _controller;
        public const int NoObstacleDistance = -1;
        private readonly object _locker = new object();
        private readonly Stopwatch _timer = new Stopwatch();
        public event EventHandler<Hcsr04ReadEventArgs> OnDataAvailable;

        public bool IsRunning { get; set; }

        public Hcsr04Client(IOptions<SiteConfig> option, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _echo = option.Value.EchoPin;
            _trigger = option.Value.TriggerPin;
            _controller = new GpioController(pinNumberingScheme);
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
            Dispose(true);
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

                _controller.Write(_trigger, PinValue.High);
                Thread.Sleep(TimeSpan.FromMilliseconds(0.01));
                _controller.Write(_trigger, PinValue.Low);


                if (!GpioEX.WaitForValue(_controller, _echo, PinValue.Low))
                    throw new TimeoutException();

                _lastMeasurment = Environment.TickCount;

                _timer.Start();

                if (!GpioEX.WaitForValue(_controller, _echo, PinValue.High))
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

        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _controller?.Dispose();
            }
        }
        public void Dispose() => Stop();
    }
}
