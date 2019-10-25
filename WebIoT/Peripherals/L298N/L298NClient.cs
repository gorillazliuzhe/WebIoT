using System.Collections.Generic;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.WiringPi;

namespace WebIoT.Peripherals.L298N
{
    /// <summary>
    /// 使用第三方库实现,原生支持没找到PWM Demo
    /// </summary>
    public class L298NClient
    {
        private IGpioPin in1 = Pi.Gpio[16];
        private IGpioPin in2 = Pi.Gpio[20];
        private IGpioPin in3 = Pi.Gpio[21];
        private IGpioPin in4 = Pi.Gpio[26];
        private GpioPin ena = (GpioPin)Pi.Gpio[18];
        private GpioPin enb = (GpioPin)Pi.Gpio[19];
        private bool disposedValue;

        public L298NClient()
        {
            in1.PinMode = GpioPinDriveMode.Output;
            in2.PinMode = GpioPinDriveMode.Output;
            in3.PinMode = GpioPinDriveMode.Output;
            in4.PinMode = GpioPinDriveMode.Output;
            ena.PinMode = GpioPinDriveMode.PwmOutput;
            ena.PwmMode = PwmMode.Balanced;
            ena.PwmClockDivisor = 2;
            enb.PinMode = GpioPinDriveMode.PwmOutput;
            enb.PwmMode = PwmMode.Balanced;
            enb.PwmClockDivisor = 2;
        }

        /// <summary>
        /// 前进
        /// </summary>
        /// <param name="sd">速度</param>
        public void Up(int sd)
        {
            List<int> DutyCycle = new List<int> { 30, 40, 50, 60, 70, 80, 90, 100 };
            ena.PwmRegister = 0;
            enb.PwmRegister = 0;
            in1.Write(GpioPinValue.Low);
            in2.Write(GpioPinValue.High);
            in3.Write(GpioPinValue.Low);
            in4.Write(GpioPinValue.High);
            ena.PwmRegister = sd;
            enb.PwmRegister = sd;
        }
        /// <summary>
        /// 后退
        /// </summary>
        /// <param name="sd">速度</param>
        public void Down(int sd)
        {
            ena.PwmRegister = 0;
            enb.PwmRegister = 0;
            in1.Write(GpioPinValue.High);
            in2.Write(GpioPinValue.Low);
            in3.Write(GpioPinValue.High);
            in4.Write(GpioPinValue.Low);
            ena.PwmRegister = sd;
            enb.PwmRegister = sd;
        }
        public void Right()
        {
            in1.Write(GpioPinValue.Low);
            in2.Write(GpioPinValue.High);
            in3.Write(GpioPinValue.Low);
            in4.Write(GpioPinValue.Low);
            ena.PwmRegister = 40;
        }
        public void Left()
        {
            in1.Write(GpioPinValue.Low);
            in2.Write(GpioPinValue.Low);
            in3.Write(GpioPinValue.Low);
            in4.Write(GpioPinValue.High);
            enb.PwmRegister = 40;
        }
        /// <summary>
        /// 停止
        /// </summary>
        public void Stop()
        {
            ena.PwmRegister = 0;
            enb.PwmRegister = 0;
            in1.Write(GpioPinValue.Low);
            in2.Write(GpioPinValue.Low);
            in3.Write(GpioPinValue.Low);
            in4.Write(GpioPinValue.Low);
        }
        public void Dispose() => Stop();
        public void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    in1 = null;
                    in2 = null;
                    in3 = null;
                    in4 = null;
                    ena = null;
                    ena = null;
                    ena = null;
                    enb = null;
                    enb = null;
                    enb = null;
                }
                disposedValue = true;
            }
        }
    }
}
