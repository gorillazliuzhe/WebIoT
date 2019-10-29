using System.Device.Gpio;

namespace WebIoT.Peripherals.L298N
{

    public class L298NClient
    {
        private const int in1 = 16;
        private const int in2 = 20;
        private const int in3 = 21;        
        private const int in4 = 26;
        private readonly GpioController _controller = new GpioController();
        private bool disposedValue;
        private readonly object _locker = new object();

        public L298NClient()
        {
            Start();
            Pause();
        }

        /// <summary>
        /// 前进
        /// </summary>
        /// <param name="sd">速度</param>
        public void Up()
        {
            Pause();
            _controller.Write(in1, PinValue.Low);
            _controller.Write(in2, PinValue.High);
            _controller.Write(in3, PinValue.Low);
            _controller.Write(in4, PinValue.High);
        }
        /// <summary>
        /// 后退
        /// </summary>
        /// <param name="sd">速度</param>
        public void Down()
        {
            Pause();
            _controller.Write(in1, PinValue.High);
            _controller.Write(in2, PinValue.Low);
            _controller.Write(in3, PinValue.High);
            _controller.Write(in4, PinValue.Low);
        }
        public void Right()
        {
            Pause();
            _controller.Write(in1, PinValue.Low);
            _controller.Write(in2, PinValue.High);
            _controller.Write(in3, PinValue.Low);
            _controller.Write(in4, PinValue.Low);
        }
        public void Left()
        {
            Pause();
            _controller.Write(in1, PinValue.Low);
            _controller.Write(in2, PinValue.Low);
            _controller.Write(in3, PinValue.Low);
            _controller.Write(in4, PinValue.High);
        }
        public void Pause()
        {
            _controller.Write(in1, PinValue.Low);
            _controller.Write(in2, PinValue.Low);
            _controller.Write(in3, PinValue.Low);
            _controller.Write(in4, PinValue.Low);
        }

        public void Start()
        {
            if (!_controller.IsPinOpen(in1))
            {
                _controller.OpenPin(in1, PinMode.Output);
            }
            if (!_controller.IsPinOpen(in2))
            {
                _controller.OpenPin(in2, PinMode.Output);
            }
            if (!_controller.IsPinOpen(in3))
            {
                _controller.OpenPin(in3, PinMode.Output);
            }
            if (!_controller.IsPinOpen(in4))
            {
                _controller.OpenPin(in4, PinMode.Output);
            }
        }
        public void Stop()
        {
            Pause();
            if (_controller.IsPinOpen(in1))
            {
                _controller.ClosePin(in1);
            }
            if (_controller.IsPinOpen(in2))
            {
                _controller.ClosePin(in2);
            }
            if (_controller.IsPinOpen(in3))
            {
                _controller.ClosePin(in3);
            }
            if (_controller.IsPinOpen(in4))
            {
                _controller.ClosePin(in4);
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
            Stop();
            Dispose(true);
        }
    }
}
