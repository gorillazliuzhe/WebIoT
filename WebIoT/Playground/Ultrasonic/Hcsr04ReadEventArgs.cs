using System;

namespace WebIoT.Playground
{
    public class Hcsr04ReadEventArgs : EventArgs
    {
        internal Hcsr04ReadEventArgs(double distance)
        {
            Distance = distance;
        }

        private Hcsr04ReadEventArgs(bool isValid) : this(Hcsr04Client.NoObstacleDistance)
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
        public bool HasObstacles => Distance != Hcsr04Client.NoObstacleDistance;

        /// <summary>
        /// 获取到障碍物的实际距离，以厘米为单位, 如果没有检测到障碍物 <see cref="UltrasonicHcsr04Client.NoObstacleDistance"/> .
        /// </summary>
        public double Distance { get; }


        internal static Hcsr04ReadEventArgs CreateInvalidReading() => new Hcsr04ReadEventArgs(false);
    }
}
