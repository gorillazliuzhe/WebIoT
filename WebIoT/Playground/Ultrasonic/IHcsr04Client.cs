using System;

namespace WebIoT.Playground
{
    public interface IHcsr04Client
    {
        event EventHandler<Hcsr04ReadEventArgs> OnDataAvailable;          
        void Start();
        void Stop();
    }
}
