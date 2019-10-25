using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebIoT.Hubs;
using WebIoT.Peripherals;

namespace WebIoT.Controllers
{
    // http://192.168.1.88/Hcsr04/Hcsr04Off
    public class Hcsr04Controller : Controller
    {
        private readonly UltrasonicHcsr04Client _hcsr04;
        private readonly IHubContext<ChatHub> _chatHub;
        public Hcsr04Controller(UltrasonicHcsr04Client hcsr04, IHubContext<ChatHub> chatHub)
        {
            _hcsr04 = hcsr04;
            _chatHub= chatHub;
        }
        public IActionResult Hcsr04On()
        {
            _hcsr04.OnDataAvailable += async (s, e) =>
            {
                if (!e.IsValid)
                {
                    await _chatHub.Clients.Group("3603631297").SendAsync("ReceiveMessage", "声波没有返回,被折射掉了.");
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