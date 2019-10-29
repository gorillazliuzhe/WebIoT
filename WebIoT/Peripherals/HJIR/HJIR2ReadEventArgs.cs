using System;

namespace WebIoT.Peripherals.HJIR
{
    public class HJIR2ReadEventArgs : EventArgs
    {
        public HJIR2ReadEventArgs(bool isValid)
        {
            HasObstacles = isValid;
        }

        /// <summary>
        /// 是否有障碍物
        /// </summary>
        /// <param name="IsValid"></param>
        /// <returns></returns>
        public bool HasObstacles { get; set; } = false;
        internal static HJIR2ReadEventArgs CreateInvalidReading() => new HJIR2ReadEventArgs(false);
    }
}
