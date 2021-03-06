﻿using Microsoft.Extensions.Options;
using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Threading;
using System.Threading.Tasks;
using WebIoT.Models;

namespace WebIoT.Playground
{
    public class HJR2Client : IHJR2Client
    {
        private readonly int _left;
        private readonly int _right;
        private readonly GpioController _controller;
        private readonly object _locker = new object();
        public event EventHandler<List<HJIR2ReadEventArgs>> OnDataAvailable;
        public bool IsRunning { get; set; }
        public HJR2Client(IOptions<SiteConfig> option, GpioController controller)
        {
            _left = option.Value.IR1Pin;
            _right = option.Value.IR2Pin;
            _controller = controller;
        }
        public void Start()
        {
            lock (_locker)
            {
                IsRunning = true;
                if (!_controller.IsPinOpen(_left))
                    _controller.OpenPin(_left, PinMode.Input);
                if (!_controller.IsPinOpen(_right))
                    _controller.OpenPin(_right, PinMode.Input);
                Task.Run(() => PerformContinuousReads());
            }           
        }
        public void Stop()
        {
            lock (_locker)
            {
                IsRunning = false;
                if (_controller.IsPinOpen(_left))
                    _controller.ClosePin(_left);
                if (_controller.IsPinOpen(_right))
                    _controller.ClosePin(_right);
            }            
        }
        private void PerformContinuousReads()
        {
            while (IsRunning)
            {
                var sensorData = RetrieveSensorData();

                if (!IsRunning) continue;
                OnDataAvailable?.Invoke(this, sensorData);
                Thread.Sleep(200);
            }
        }
        private List<HJIR2ReadEventArgs> RetrieveSensorData()
        {
            List<HJIR2ReadEventArgs> list = new List<HJIR2ReadEventArgs>();
            try
            {
                if (_controller.Read(_left) == PinValue.Low)
                {
                    list.Add(new HJIR2ReadEventArgs(true) { type = 1, HasObstacles = true });
                }
                if (_controller.Read(_right) == PinValue.Low)
                {
                    list.Add(new HJIR2ReadEventArgs(true) { type = 2, HasObstacles = true });
                }
                return list;
            }
            catch
            {
                list.Add(HJIR2ReadEventArgs.CreateInvalidReading());
                return list;
            }
        }
    }
}
