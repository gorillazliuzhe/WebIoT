using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebIoT.Peripherals;

namespace WebIoT.Controllers
{
    public class LedController : Controller
    {
        private readonly LedClient _ledClient;
        public LedController(LedClient ledClient)
        {
            Unosquare.RaspberryIO.Pi.Timing.SleepMicroseconds(2);
            _ledClient = ledClient;
        }
        public IActionResult LedOn()
        {
            _ledClient.LedOn();
            return Content("Led on");
        }
        public IActionResult LedOff()
        {
            _ledClient.LedOff();
            return Content("Led off");
        }
    }
}