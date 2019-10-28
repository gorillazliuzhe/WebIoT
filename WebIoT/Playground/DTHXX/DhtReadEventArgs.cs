using System;

namespace WebIoT.Playground
{
    /// <summary>
    /// 已读取的传感器数据.
    /// </summary>
    public sealed class DhtReadEventArgs : EventArgs
    {
        /// <summary>
        /// 初始化
        /// </summary>
        /// <param name="temperatureCelsius">摄氏温度.</param>
        /// <param name="humidityPercentage">湿度百分比.</param>
        internal DhtReadEventArgs(double temperatureCelsius, double humidityPercentage)
        {
            Temperature = temperatureCelsius;
            HumidityPercentage = humidityPercentage;
        }

        /// <summary>
        /// 防止创建DhtReadEventArgs默认实例
        /// </summary>
        private DhtReadEventArgs()
        {
            // placeholder
        }

        /// <summary>
        /// 如果传感器读数有效，则返回true；
        /// </summary>
        public bool IsValid { get; private set; } = true;

        /// <summary>
        /// 获取摄氏温度.
        /// </summary>
        public double Temperature { get; }

        /// <summary>
        /// 获取以华氏度为单位的温度.
        /// </summary>
        public double TemperatureFahrenheit => (Temperature * (9.0 / 5.0)) + 32.0;

        /// <summary>
        /// 获取湿度百分比.
        /// </summary>
        public double HumidityPercentage { get; }

        internal static DhtReadEventArgs CreateInvalidReading() => new DhtReadEventArgs { IsValid = false };
    }
}
