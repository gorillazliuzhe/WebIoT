using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIoT.Playground.Ultrasonic
{
    public interface IHcsr04Client
    {
        bool IsRunning { get; set; }
        event EventHandler<Hcsr04ReadEventArgs> OnDataAvailable;
        void Start();
        void Stop();
    }
}
