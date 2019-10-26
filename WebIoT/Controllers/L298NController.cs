using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebIoT.Peripherals.L298N;

namespace WebIoT.Controllers
{
    public class L298NController : Controller
    {
        private readonly L298NClient _l298n;
        public L298NController(L298NClient l298n)
        {
            _l298n = l298n;
        }

        public IActionResult Start()
        {
            _l298n.Start();
            return Content("开启WiFi控制");
        }
        public IActionResult Up()
        {
            _l298n.Up();
            return Content("前进");
        }
        public IActionResult Down()
        {
            _l298n.Down();
            return Content("后退");
        }
        public IActionResult Left()
        {
            _l298n.Left();
            return Content("左转");
        }
        public IActionResult Right()
        {
            _l298n.Right();
            return Content("右转");
        }
        public IActionResult Pause()
        {
            _l298n.Pause();
            return Content("右转");
        }
        public IActionResult Stop()
        {
            _l298n.Stop();
            return Content("停止WiFi控制");
        }

    }
}