using Microsoft.Extensions.Options;
using System;
using System.Device.Gpio;
using WebIoT.Models;

namespace WebIoT.Playground
{
    public class L298NClient : IL298NClient, IDisposable
    {
        private readonly int _in1;
        private readonly int _in2;
        private readonly int _in3;
        private readonly int _in4;
        private bool disposedValue;
        private GpioController _controller;
        private readonly object _locker = new object();

        public L298NClient(IOptions<SiteConfig> option, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _in1 = option.Value.L298nIn1Pin;
            _in2 = option.Value.L298nIn2Pin;
            _in3 = option.Value.L298nIn3Pin;
            _in4 = option.Value.L298nIn4Pin;
            _controller = new GpioController(pinNumberingScheme);
            Start();
            Pause();
        }
        /// <summary>
        /// 前进
        /// </summary>
        /// <param name="sd">速度</param>
        public void Up()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.High);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.High);
            }
        }
        /// <summary>
        /// 后退
        /// </summary>
        /// <param name="sd">速度</param>
        public void Down()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.High);
                _controller.Write(_in2, PinValue.Low);
                _controller.Write(_in3, PinValue.High);
                _controller.Write(_in4, PinValue.Low);
            }
        }
        public void Right()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.High);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.Low);
            }
        }
        public void Left()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.Low);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.High);
            }
        }
        public void Pause()
        {
            lock (_locker)
            {
                _controller.Write(_in1, PinValue.Low);
                _controller.Write(_in2, PinValue.Low);
                _controller.Write(_in3, PinValue.Low);
                _controller.Write(_in4, PinValue.Low);
            }
        }

        public void Start()
        {
            lock (_locker)
            {
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
