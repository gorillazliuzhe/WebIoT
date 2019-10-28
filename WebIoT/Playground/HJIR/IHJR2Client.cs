using System;
using System.Collections.Generic;

namespace WebIoT.Playground
{
    public interface IHJR2Client
    {
        event EventHandler<List<HJIR2ReadEventArgs>> OnDataAvailable;
        void Start();
        void Stop();
    }
}
