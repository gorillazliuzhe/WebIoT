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

        public IActionResult Up(int sd=40,int zy=2)
        {
            _l298n.Up(sd,zy);
            return Content("前进");
        }
        public IActionResult Down(int sd = 40)
        {
            _l298n.Down(sd);
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
        public IActionResult Stop()
        {
            _l298n.Stop();
            return Content("停止");
        }

        public IActionResult Dis()
        {
            _l298n.Dispose(true);
            return Content("销毁了");
        }
    }
}