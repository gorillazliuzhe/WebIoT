using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using Unosquare.RaspberryIO.Peripherals;

namespace WebIoT.Controllers
{
    public class CarController : Controller
    {
        private readonly UltrasonicHcsr04 _hcsr04;

        public CarController(UltrasonicHcsr04 hcsr04)
        {
            _hcsr04 = hcsr04;
        }
        public IActionResult Index()
        {
            return View();
        }
        
        public IActionResult Hcsr04On()
        {
            var blinkingPin = Pi.Gpio[BcmPin.Gpio17];
            blinkingPin.PinMode = GpioPinDriveMode.Output;
            _hcsr04.OnDataAvailable += (s, e) =>
            {
                if (!e.IsValid)
                {
                    Console.WriteLine("声波没有返回,被折射掉了.");
                    blinkingPin.Write(GpioPinValue.Low);
                }
                else if (e.HasObstacles)
                {
                    Console.WriteLine($"距离:{e.Distance:N2}cm");
                    if (e.Distance > 20)
                    {
                        blinkingPin.Write(GpioPinValue.Low);
                    }
                    else
                    {
                        blinkingPin.Write(GpioPinValue.High);
                    }
                }
                else
                {
                    Console.WriteLine("未检测到障碍物.");
                    blinkingPin.Write(GpioPinValue.Low);
                }
            };
            _hcsr04.Start();
            return Content("超声波打开");
        }
        public IActionResult Hcsr04Off()
        {
            _hcsr04.Stop();
            return Content("超声波关闭");
        }
    }
}