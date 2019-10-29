using Microsoft.AspNetCore.Mvc;
using WebIoT.Playground;

namespace WebIoT.Controllers
{
    // http://192.168.1.88/led/LedOn
    public class LedController : Controller
    {
        private readonly ILedClient _ledClient;
        public LedController(ILedClient ledClient)
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