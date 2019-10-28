using Microsoft.AspNetCore.Mvc;
using WebIoT.Peripherals;

namespace WebIoT.Controllers
{
    // http://192.168.1.88/led/LedOn
    public class LedController : Controller
    {
        private readonly LedClient _ledClient;
        public LedController(LedClient ledClient)
        {
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