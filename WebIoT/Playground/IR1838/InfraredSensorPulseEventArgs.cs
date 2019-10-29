using System;

namespace WebIoT.Playground.IR1838
{

    /// <summary>
    /// 包含原始传感器数据事件参数。
    /// </summary>
    public class InfraredSensorPulseEventArgs : EventArgs
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="InfraredSensorPulseEventArgs" /> class.
        /// </summary>
        /// <param name="pulse">The pulse.</param>
        internal InfraredSensorPulseEventArgs(InfraredPulse pulse)
        {
            Value = pulse.Value;
            DurationUsecs = pulse.DurationUsecs;
        }

        /// <summary>
        /// Prevents a default instance of the <see cref="InfraredSensorPulseEventArgs"/> class from being created.
        /// </summary>
        private InfraredSensorPulseEventArgs()
        {
            // placeholder
        }

        /// <summary>
        /// Gets the signal value.
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// Gets the duration microseconds.
        /// </summary>
        public long DurationUsecs { get; }
    }
}
