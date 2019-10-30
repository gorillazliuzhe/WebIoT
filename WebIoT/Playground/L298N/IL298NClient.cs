namespace WebIoT.Playground
{
    public interface IL298NClient
    {
        /// <summary>
        /// 移动
        /// </summary>
        /// <param name="type">1:前进 2:后退 3:左转 4:右转 5:暂停</param>
        /// <param name="speed">速度 0-1</param>
        void Move(int type, double speed);
        //void Up();
        //void Down();
        //void Right();
        //void Left();
        //void Pause();
        void Start();
        void Stop();
    }
}
