namespace WebIoT.Peripherals.Infrared
{
    using System;
    using System.Linq;

    public sealed partial class InfraredSensor
    {
        /// <summary>
        /// 表示已经准备好解码接收器缓冲区的事件参数。
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
            /// Gets the array fo IR pulses.
            /// </summary>
            public InfraredPulse[] Pulses { get; }

            /// <summary>
            /// Gets the state of the receiver that triggered the event.
            /// </summary>
            public ReceiverFlushReason FlushReason { get; }

            /// <summary>
            /// Gets the pulse train duration in microseconds.
            /// </summary>
            public long TrainDurationUsecs { get; }
        }
    }
}
