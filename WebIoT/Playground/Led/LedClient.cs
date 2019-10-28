using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;
using WebIoT.Models;

namespace WebIoT.Playground.Led
{
    public class LedClient : ILedClient, IDisposable
    {
        private readonly int _ledPin;
        private GpioController _controller;
        private readonly object _locker = new object();
        public LedClient(IOptions<SiteConfig> option, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _ledPin = option.Value.LedPin;
            _controller = new GpioController(pinNumberingScheme);
        }
        public void LedOff()
        {
            lock (_locker)
            {
                _controller.Write(_ledPin, PinValue.High);
            }
        }

        public void LedOn()
        {
            lock (_locker)
            {
                _controller.Write(_ledPin, PinValue.Low);
            }
        }
        public void Dispose()
        {
            if (_controller != null)
            {
                _controller.Dispose();
                _controller = null;
            }
        }
    }
}
