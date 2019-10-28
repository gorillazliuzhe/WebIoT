using Iot.Units;
using Microsoft.Extensions.Options;
using System;
using System.Device;
using System.Device.Gpio;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using WebIoT.Models;

namespace WebIoT.Playground
{
    public class Dth22 : IDisposable
    {
        private int _period;
        private readonly int _datapin;
        private readonly Timer _readTimer;
        private bool _disposedValue;
        private const long BitPulseMidMicroseconds = 60;
        private readonly uint _loopCount = 10000;
        protected readonly GpioController _controller;
        private readonly Stopwatch _stopwatch = new Stopwatch();
        public event EventHandler<DhtReadEventArgs> OnDataAvailable;

        public bool IsRunning { get; private set; }
        protected uint PullDownMicroseconds = 1300;

        public Dth22(IOptions<SiteConfig> option, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical)
        {
            _datapin = option.Value.DHT22Pin;
            _controller = new GpioController(pinNumberingScheme);
            _readTimer = new Timer(PerformContinuousReads, null, Timeout.Infinite, Timeout.Infinite);
        }

        public void Start() => Start(2);
        public void Start(int period)
        {
            if (period < 2)
                throw new InvalidOperationException("周期不得少于2秒.");

            _period = period * 1000;
            IsRunning = true;
            _readTimer.Change(0, Timeout.Infinite);
        }
        public void Stop() => StopContinuousReads();
        private void StopContinuousReads()
        {
            _readTimer.Change(Timeout.Infinite, Timeout.Infinite);
            IsRunning = false;
        }

        /// <summary>
        /// 读取传感器.
        /// </summary>
        private void PerformContinuousReads(object state)
        {
            if (!IsRunning)
                return;

            try
            {
                var sensorData = RetrieveSensorData();
                if (IsRunning)
                {
                    OnDataAvailable?.Invoke(this, sensorData);
                    _readTimer.Change(_period, Timeout.Infinite);
                }
            }
            catch { }
        }

        /// <summary>
        /// 返回传感器数据.
        /// </summary>
        /// <returns>从传感器读取的事件参数.</returns>
        private DhtReadEventArgs RetrieveSensorData()
        {
            // 准备缓冲区以存储度量和校验和
            var _readBuff = new byte[5];

            byte readVal = 0;
            uint count;

            // keep data line HIGH
            _controller.SetPinMode(_datapin, PinMode.Output);
            _controller.Write(_datapin, PinValue.High);
            DelayHelper.DelayMilliseconds(20, true);

            // send trigger signal
            _controller.Write(_datapin, PinValue.Low);
            // wait at least 18 milliseconds
            // here wait for 18 milliseconds will cause sensor initialization to fail
            DelayHelper.DelayMilliseconds(20, true);

            // pull up data line
            _controller.Write(_datapin, PinValue.High);
            // wait 20 - 40 microseconds
            DelayHelper.DelayMicroseconds(30, true);

            _controller.SetPinMode(_datapin, PinMode.InputPullUp);

            // DHT corresponding signal - LOW - about 80 microseconds
            count = _loopCount;
            while (_controller.Read(_datapin) == PinValue.Low)
            {
                if (count-- == 0)
                {
                    IsLastReadSuccessful = false;
                    return;
                }
            }

            // HIGH - about 80 microseconds
            count = _loopCount;
            while (_controller.Read(_datapin) == PinValue.High)
            {
                if (count-- == 0)
                {
                    IsLastReadSuccessful = false;
                    return;
                }
            }

            // the read data contains 40 bits
            for (int i = 0; i < 40; i++)
            {
                // beginning signal per bit, about 50 microseconds
                count = _loopCount;
                while (_controller.Read(_datapin) == PinValue.Low)
                {
                    if (count-- == 0)
                    {
                        IsLastReadSuccessful = false;
                        return;
                    }
                }

                // 26 - 28 microseconds represent 0
                // 70 microseconds represent 1
                _stopwatch.Restart();
                count = _loopCount;
                while (_controller.Read(_datapin) == PinValue.High)
                {
                    if (count-- == 0)
                    {
                        IsLastReadSuccessful = false;
                        return;
                    }
                }
                _stopwatch.Stop();

                // bit to byte
                // less than 40 microseconds can be considered as 0, not necessarily less than 28 microseconds
                // here take 30 microseconds
                readVal <<= 1;
                if (!(_stopwatch.ElapsedTicks * 1000000F / Stopwatch.Frequency <= 30))
                {
                    readVal |= 1;
                }

                if (((i + 1) % 8) == 0)
                {
                    _readBuff[i / 8] = readVal;
                }
            }

            _lastMeasurement = Environment.TickCount;

            if ((_readBuff[4] == ((_readBuff[0] + _readBuff[1] + _readBuff[2] + _readBuff[3]) & 0xFF)))
            {
                IsLastReadSuccessful = (_readBuff[0] != 0) || (_readBuff[2] != 0);
            }
            else
            {
                IsLastReadSuccessful = false;
            }
        }

        ///<summary>
        /// 解码温度.
        /// </summary>
        /// <param name="data">数据.</param>
        /// <returns>代表当前温度的值.</returns>
        protected double DecodeHumidity(byte[] data) => ((data[0] << 8) | data[1]) * 0.1;

        /// <summary>
        /// 解码湿度
        /// </summary>
        /// <param name="data">数据</param>
        /// <returns>代表当前湿度的值</returns>
        protected double DecodeTemperature(byte[] data)
        {
            var temp = (((data[2] & 0x7F) << 8) | data[3]) * 0.1;
            if ((data[2] & 0x80) == 0x80)
                temp *= -1;

            return temp;
        }


        public void Dispose() => Dispose(true);

        protected virtual void Dispose(bool disposing)
        {
            if (_disposedValue) return;

            if (disposing)
            {
                if (IsRunning)
                    StopContinuousReads();

                _readTimer.Dispose();
            }
            _disposedValue = true;
        }
    }
}
