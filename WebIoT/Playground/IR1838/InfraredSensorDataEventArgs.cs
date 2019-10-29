using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIoT.Playground.IR1838
{
    /// <summary>
    /// 表示准备好解码接收器缓冲区的事件参数。
    /// </summary>
    /// <seealso cref="EventArgs" />
    public class InfraredSensorDataEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfraredSensorDataEventArgs" /> class.
        /// </summary>
        /// <param name="pulses">The pulses.</param>
        /// <param name="flushReason">The state.</param>
        internal InfraredSensorDataEventArgs(InfraredPulse[] pulses, ReceiverFlushReason flushReason)
        {
            Pulses = pulses;
            FlushReason = flushReason;
            TrainDurationUsecs = pulses.Sum(s => s.DurationUsecs);
        }

        /// <summary>
        /// 获取红外脉冲
        /// </summary>
        public InfraredPulse[] Pulses { get; }

        /// <summary>
        /// 获取触发事件的接收者的状态。
        /// </summary>
        public ReceiverFlushReason FlushReason { get; }

        /// <summary>
        /// 获取脉冲序列的持续时间（以微秒为单位）。
        /// </summary>
        public long TrainDurationUsecs { get; }
    }
}
