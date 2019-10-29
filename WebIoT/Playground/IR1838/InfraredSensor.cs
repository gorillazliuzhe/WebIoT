using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using WebIoT.Models;

namespace WebIoT.Playground.IR1838
{
    public class InfraredSensor
    {
        private volatile bool _isDisposed;
        private volatile bool _isInReadInterrupt;
        private volatile bool _currentValue;
        private Timer _idleChecker;
        private readonly int _dataPin;
        private GpioController _controller;

        /// <summary>
        /// 当单个传感器脉冲可用时发生.
        /// </summary>
        public event EventHandler<InfraredSensorPulseEventArgs> PulseAvailable;

        /// <summary>
        /// 当数据缓冲区可用时发生.
        /// </summary>
        public event EventHandler<InfraredSensorDataEventArgs> DataAvailable;
        public bool IsActiveLow { get; } //获取一个值，该值指示传感器是否处于低电平有效状态.
        public InfraredSensor(IOptions<SiteConfig> option, PinNumberingScheme pinNumberingScheme = PinNumberingScheme.Logical, bool isActiveLow=true)
        {
            IsActiveLow = isActiveLow;
            _dataPin = option.Value.HXPin;
            _controller = new GpioController(pinNumberingScheme);
        }
    }

    /// <summary>
    /// 列举了接收方刷新缓冲区的不同原因.
    /// </summary>
    public enum ReceiverFlushReason
    {
        /// <summary>
        /// 空闲状态
        /// </summary>
        Idle,

        /// <summary>
        /// 溢出状态
        /// </summary>
        Overflow,
    }
}
