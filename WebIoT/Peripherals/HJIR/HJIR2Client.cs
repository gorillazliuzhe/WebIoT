using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

namespace WebIoT.Peripherals.HJIR
{
    /// <summary>
    /// 红外避障模块 8 7 
    /// </summary>
    public class HJIR2Client : IDisposable
    {
        private readonly int inPin;
        private bool disposedValue;
        private Thread readWorker;
        private readonly GpioController controller = new GpioController();
        /// <summary>
        /// 红外避障模块
        /// </summary>
        /// <param name="outPin">连接CE* 低电平有效的芯片端口</param>
        public HJIR2Client(int pinnum = 8)
        {
            inPin = pinnum;
            readWorker = new Thread(PerformContinuousReads);
        }

        /// <summary>
        /// 当传感器有数据时候发生
        /// </summary>
        public event EventHandler<HJIR2ReadEventArgs> OnDataAvailable;

        /// <summary>
        /// 获取一个值，该值指示此实例是否正在运行
        /// </summary>
        public bool IsRunning { get; private set; }

        /// <summary>
        /// 开始传感器读取.
        /// </summary>
        public void Start()
        {
            controller.OpenPin(inPin, PinMode.Input);
            IsRunning = true;
            readWorker.Start();
        }

        /// <summary>
        /// 停止传感器读取
        /// </summary>
        public void Stop() => IsRunning = false;
        private void PerformContinuousReads(object obj)
        {
            while (IsRunning)
            {
                var sensorData = RetrieveSensorData(); // 获取数据
                if (!IsRunning) continue;
                OnDataAvailable?.Invoke(this, sensorData);
                Thread.Sleep(200);
            }
            Dispose(true);
        }

        private HJIR2ReadEventArgs RetrieveSensorData()
        {
            try
            {
                return new HJIR2ReadEventArgs(controller.Read(inPin) == PinValue.Low);
            }
            catch
            {
                return HJIR2ReadEventArgs.CreateInvalidReading();
            }
        }

        public void Dispose() => Stop();
        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    readWorker = null;
                    controller.Dispose();
                }
                disposedValue = true;
            }
        }
    }
}
