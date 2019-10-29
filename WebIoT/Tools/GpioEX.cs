using System;
using System.Device.Gpio;

namespace WebIoT.Tools
{
    public static class GpioEX
    {
        public static bool WaitForValue(GpioController gpio, int pinNumber, PinValue value, int timeOutMillisecond = 50)
        {
            try
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
            catch (Exception)
            {
                throw;
            }
        }
    }
}
