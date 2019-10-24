using Microsoft.AspNetCore.Mvc;
using WebIoT.Peripherals;

namespace WebIoT.Controllers
{
    public class CarController : Controller
    {

        public CarController()
        {

        }
        public IActionResult Index()
        {
            return View();
        }
    
    
    }
}