namespace WebIoT.Playground.IR1838
{
    public class InfraredPulse
    {
        /// <summary>
        /// 初始化实例
        /// </summary>
        /// <param name="value">信号值</param>
        /// <param name="durationUsecs">持续时间</param>
        internal InfraredPulse(bool value, long durationUsecs)
        {
            Value = value;
            DurationUsecs = durationUsecs;
        }

        /// <summary>
        /// 防止创建默认实例
        /// </summary>
        private InfraredPulse()
        {

        }

        /// <summary>
        /// 获取信号值
        /// </summary>
        public bool Value { get; }

        /// <summary>
        /// 获取持续时间 微秒.
        /// </summary>
        public long DurationUsecs { get; }
    }
}
