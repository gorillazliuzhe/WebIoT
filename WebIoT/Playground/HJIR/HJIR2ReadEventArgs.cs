namespace WebIoT.Playground
{
    public class HJIR2ReadEventArgs
    {
        public HJIR2ReadEventArgs(bool isValid)
        {
            IsValid = isValid;
        }

        /// <summary>
        /// 1:left 2:right
        /// </summary>
        public int type { get; set; }

        /// <summary>
        /// 如果读数有效，则返回true.
        /// </summary>
        public bool IsValid { get; } = true;

        public bool HasObstacles { get; set; } = false;

        internal static HJIR2ReadEventArgs CreateInvalidReading() => new HJIR2ReadEventArgs(false);
    }
}
