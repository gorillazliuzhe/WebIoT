namespace WebIoT.Peripherals.Infrared
{
    public sealed partial class InfraredSensor
    {
        /// <summary>
        /// Represents data of an infrared pulse.
        /// 代表红外脉冲的数据.
        /// </summary>
        public class InfraredPulse
        {
            /// <summary>
            /// Initializes a new instance of the <see cref="InfraredPulse"/> class.
            /// </summary>
            /// <param name="value">if set to <c>true</c> [value].</param>
            /// <param name="durationUsecs">The duration usecs.</param>
            internal InfraredPulse(bool value, long durationUsecs)
            {
                Value = value;
                DurationUsecs = durationUsecs;
            }

            /// <summary>
            /// Prevents a default instance of the <see cref="InfraredPulse"/> class from being created.
            /// </summary>
            private InfraredPulse()
            {
                // placeholder
            }

            /// <summary>
            /// Gets the signal value.
            /// </summary>
            public bool Value { get; }

            /// <summary>
            /// Gets the duration microseconds.
            /// 获取持续时间微秒.
            /// </summary>
            public long DurationUsecs { get; }
        }
    }
}
