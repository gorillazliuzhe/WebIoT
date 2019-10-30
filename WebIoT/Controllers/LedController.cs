using Microsoft.AspNetCore.Mvc;
using System.Threading;
using System.Threading.Tasks;
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
        public IActionResult LedPWMOn()
        {
            _ledClient.LedHardPWMOn();
            return Content("Led hardpwm on");
        }
        public IActionResult LedPWMOff()
        {
            _ledClient.LedHardPWMOff();
            return Content("Led hardpwm off");
        }
        public IActionResult LedSoftPWMOn(CancellationToken cancellationToken)
        {
            _ledClient.LedSoftPWMOn(cancellationToken);
            return Content("Led Softpwm on");
        }
        public IActionResult LedSoftPWMOff(CancellationToken cancellationToken)
        {
            _ledClient.LedSoftPWMOff(cancellationToken);
            return Content("Led Softpwm off");
        }
    }
}