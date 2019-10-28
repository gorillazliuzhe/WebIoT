using Microsoft.Extensions.Options;
using System;
using System.Device.Gpio;
using WebIoT.Models;

namespace WebIoT.Playground
{
    public class LedClient : ILedClient, IDisposable
    {
        private readonly int _ledPin;
        private bool disposedValue;
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
        public void Dispose()
        {
            Dispose(true);
        }
    }
}
