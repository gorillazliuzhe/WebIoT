using Microsoft.Extensions.Options;
using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using WebIoT.Models;

namespace WebIoT.Playground
{
    public class L298NClient : IL298NClient, IDisposable
    {
        private readonly int _in1;
        private readonly int _in2;
        private readonly int _in3;
        private readonly int _in4;
        private readonly int _ena;
        private readonly int _enb;
        private readonly int _frequency;
        private PwmChannel _pwma;
        private PwmChannel _pwmb;
        private readonly GpioController _controller;
        private readonly object _locker = new object();
        public double Speed { get; set; }
        public L298NClient(IOptions<SiteConfig> option, GpioController controller)
        {
            _in1 = option.Value.L298nIn1Pin;
            _in2 = option.Value.L298nIn2Pin;
            _in3 = option.Value.L298nIn3Pin;
            _in4 = option.Value.L298nIn4Pin;
            _ena = option.Value.L298nENAPin;
            _enb = option.Value.L298nENBPin;
            _frequency = option.Value.Frequency;
            _controller = controller;
            Start();
            Pause();
        }

        /// <summary>
        /// 初始化PWM
        /// </summary>
        private void InitPWM()
        {
            _pwma = new SoftwarePwmChannel(_ena, _frequency, 0.0, controller: _controller);
            _pwma.Start();
            _pwmb = new SoftwarePwmChannel(_enb, _frequency, 0.0, controller: _controller);
            _pwmb.Start();
        }
      
        public void Move(int type, double speed)
        {
            lock (_locker)
            {
                Speed = speed;
                if (Speed > 0.0)
                {
                    switch (type)
                    {
                        case 1:
                            Up();
                            break;
                        case 2:
                            Down();
                            break;
                        case 3:
                            Left();
                            break;
                        case 4:
                            Right();
                            break;
                        case 5:
                            Pause();
                            break;
                        default:
                            Pause();
                            break;
                    }
                }
                else
                {
                    Pause();
                }
            }
        }

        private void Up()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.High);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.High);
                _pwma.DutyCycle = Math.Abs(Speed);
                _pwmb.DutyCycle = Math.Abs(Speed);
            }
        }
        private void Down()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.High);
                _controller.Write(_in2, PinValue.Low);
                _controller.Write(_in3, PinValue.High);
                _controller.Write(_in4, PinValue.Low);
                _pwma.DutyCycle = Math.Abs(Speed);
                _pwmb.DutyCycle = Math.Abs(Speed);
            }
        }
        private void Right()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.High);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.Low);
                _pwma.DutyCycle = Math.Abs(Speed);
                _pwmb.DutyCycle = Math.Abs(0);
            }
        }
        private void Left()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.Low);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.High);
                _pwma.DutyCycle = Math.Abs(0);
                _pwmb.DutyCycle = Math.Abs(Speed);
            }
        }
        private void Pause()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.Low);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.Low);
                _pwma.DutyCycle = Math.Abs(0);
                _pwmb.DutyCycle = Math.Abs(0);
            }
        }
        public void Start()
        {
            lock (_locker)
            {
                InitPWM();
                if (!_controller.IsPinOpen(_in1))
                    _controller.OpenPin(_in1, PinMode.Output);
                if (!_controller.IsPinOpen(_in2))
                    _controller.OpenPin(_in2, PinMode.Output);
                if (!_controller.IsPinOpen(_in3))
                    _controller.OpenPin(_in3, PinMode.Output);
                if (!_controller.IsPinOpen(_in4))
                    _controller.OpenPin(_in4, PinMode.Output);
                Pause();
            }
        }
        public void Stop()
        {
            lock (_locker)
            {
                Pause();
                if (_controller.IsPinOpen(_in1))
                    _controller.ClosePin(_in1);
                if (_controller.IsPinOpen(_in2))
                    _controller.ClosePin(_in2);
                if (_controller.IsPinOpen(_in3))
                    _controller.ClosePin(_in3);
                if (_controller.IsPinOpen(_in4))
                    _controller.ClosePin(_in4);
                _pwma.DutyCycle = Math.Abs(0);
                _pwmb.DutyCycle = Math.Abs(0);
                Dispose(true);
            }
        }
        protected virtual void Dispose(bool disposing)
        {
            if (disposing)
            {
                _pwma.Stop();
                _pwmb.Stop();
                _pwma?.Dispose();
                _pwmb?.Dispose();
            }
        }
        public void Dispose() => Stop();
    }
}
