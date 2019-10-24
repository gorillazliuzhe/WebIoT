using System;
using System.Device.Gpio;

namespace WebIoT.Peripherals
{
    public class LedClient : IDisposable
    {
        private const int LedPin = 17;
        private readonly GpioController _controller = new GpioController();
        private bool disposedValue = false;
        private readonly object _locker = new object();
        public LedClient()
        {
            _controller.OpenPin(LedPin, PinMode.Output);
            _controller.Write(LedPin, PinValue.Low);
        }
        public void LedOn()
        {
            lock (_locker)
            {
                _controller.Write(LedPin, PinValue.High);
            }
        }
        public void LedOff()
        {
            lock (_locker)
            {
                _controller.Write(LedPin, PinValue.Low);
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
