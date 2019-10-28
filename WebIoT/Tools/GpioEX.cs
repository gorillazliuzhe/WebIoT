using System;
using System.Collections.Generic;
using System.Device.Gpio;
using System.Linq;
using System.Threading.Tasks;

namespace WebIoT.Tools
{
    public static class GpioEX
    {
        public static bool WaitForValue(GpioController gpio, int pinNumber, PinValue value, int timeOutMillisecond = 50)
        {
            var outstime = DateTime.Now.Millisecond;
            do
            {
                var outseime = DateTime.Now.Millisecond;
                if ((outseime - outstime) > timeOutMillisecond)
                {
                    return false;
                }
            } while (gpio.Read(pinNumber) == value);
            return true;
        }
    }
}
