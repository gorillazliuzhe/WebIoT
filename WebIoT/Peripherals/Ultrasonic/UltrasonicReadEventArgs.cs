using System;

namespace WebIoT.Peripherals
{
    public class UltrasonicReadEventArgs : EventArgs
    {
        internal UltrasonicReadEventArgs(double distance)
        {
            Distance = distance;
        }

        private UltrasonicReadEventArgs(bool isValid) : this(UltrasonicHcsr04Client.NoObstacleDistance)
        {
            IsValid = isValid;
        }

        /// <summary>
        /// 如果读数有效，则返回true.
        /// </summary>
        public bool IsValid { get; } = true;

        /// <summary>
        /// 获取一个值，该值指示是否检测到任何障碍物.
        /// </summary>
        public bool HasObstacles => Distance != UltrasonicHcsr04Client.NoObstacleDistance;

        /// <summary>
        /// 获取到障碍物的实际距离，以厘米为单位, 如果没有检测到障碍物 <see cref="UltrasonicHcsr04Client.NoObstacleDistance"/> .
        /// </summary>
        public double Distance { get; }


        internal static UltrasonicReadEventArgs CreateInvalidReading() => new UltrasonicReadEventArgs(false);
    }
}
