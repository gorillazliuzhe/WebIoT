using Microsoft.Extensions.Options;
using System;
using System.Device.Gpio;
using System.Device.Pwm;
using System.Device.Pwm.Drivers;
using System.Threading;
using System.Threading.Tasks;
using WebIoT.Models;

namespace WebIoT.Playground
{
    public class LedClient : ILedClient
    {
        private readonly int _ledPin;
        private readonly int _softpwmPin;

        private readonly GpioController _controller;
        private readonly object _locker = new object();
        public LedClient(IOptions<SiteConfig> option, GpioController controller)
        {
            _ledPin = option.Value.LedPin;
            _softpwmPin = option.Value.SoftPWMPin;
            _controller = controller;
        }
        public bool IsRunning { get; set; }
        public void LedOn()
        {
            lock (_locker)
            {
                if (!_controller.IsPinOpen(_ledPin))
                {
                    _controller.OpenPin(_ledPin, PinMode.Output);
                }
                _controller.Write(_ledPin, PinValue.High);
            }
        }

        public void LedOff()
        {
            lock (_locker)
            {
                if (!_controller.IsPinOpen(_ledPin))
                {
                    _controller.OpenPin(_ledPin, PinMode.Output);
                }
                _controller.Write(_ledPin, PinValue.Low);
            }
        }

        public void LedHardPWMOn()
        {
            lock (_locker)
            {
                IsRunning = true;
                Task.Run(() =>
                {
                    int brightness = 0;
                    using PwmChannel _pwm = PwmChannel.Create(chip: 0, channel: 0, frequency: 400, dutyCyclePercentage: 0);
                    _pwm.Start();
                    while (IsRunning)
                    {
                        while (brightness != 255)
                        {
                            _pwm.DutyCycle = brightness / 255D;
                            brightness++;
                            Thread.Sleep(10);
                        }

                        while (brightness != 0)
                        {
                            _pwm.DutyCycle = brightness / 255D;
                            brightness--;
                            Thread.Sleep(10);
                        }
                    }
                });
            }
        }

        public void LedHardPWMOff()
        {
            lock (_locker)
            {
                IsRunning = false;
                using PwmChannel _pwm = PwmChannel.Create(chip: 0, channel: 0, frequency: 400, dutyCyclePercentage: 0);
                _pwm.Stop();
            }
        }

        public Task LedSoftPWMOn(CancellationToken cancellationToken)
        {
            lock (_locker)
            {
                IsRunning = true;
                return Task.Run(() =>
                {
                    int brightness = 0;
                    using PwmChannel _pwm = new SoftwarePwmChannel(_softpwmPin, frequency: 400, dutyCycle: 0);
                    _pwm.Start();
                    try
                    {
                        while (IsRunning)
                        {
                            while (brightness != 255)
                            {
                                _pwm.DutyCycle = brightness / 255D;
                                brightness++;
                                Thread.Sleep(10);
                            }

                            while (brightness != 0)
                            {
                                _pwm.DutyCycle = brightness / 255D;
                                brightness--;
                                Thread.Sleep(10);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(ex);
                    }

                });
            }
        }
        public void LedSoftPWMOff(CancellationToken cancellationToken)
        {
            lock (_locker)
            {
                IsRunning = false;
                using PwmChannel _pwm = new SoftwarePwmChannel(_softpwmPin, frequency: 400, dutyCycle: 0);
                _pwm.Stop();
            }
        }
    }
}
