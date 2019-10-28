﻿namespace WebIoT.Playground.L298N
{
    public interface IL298NClient
    {
        void Up();
        void Down();
        void Right();
        void Left();
        void Pause();
        void Start();
        void Stop();
    }
}