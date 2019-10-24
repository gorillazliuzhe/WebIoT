using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebIoT.Peripherals;

namespace WebIoT.Controllers
{
    public class Hcsr04Controller : Controller
    {
        private readonly UltrasonicHcsr04Client _hcsr04;
        private readonly CancellationTokenSource cancellationTokenSource = new CancellationTokenSource();
        public Hcsr04Controller(UltrasonicHcsr04Client hcsr04)
        {
            _hcsr04 = hcsr04;
        }
        public IActionResult Hcsr04On()
        {
            _hcsr04.OnDataAvailable += (s, e) =>
            {
                if (!e.IsValid)
                {
                    Console.WriteLine("声波没有返回,被折射掉了.");
                }
                else if (e.HasObstacles)
                {
                    Console.WriteLine($"距离:{e.Distance:N2}cm");
                }
                else
                {
                    Console.WriteLine("未检测到障碍物.");
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