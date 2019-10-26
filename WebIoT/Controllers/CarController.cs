using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebIoT.Hubs;
using WebIoT.Peripherals;

namespace WebIoT.Controllers
{
    public class CarController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHub;
        public CarController(IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
        }
        public IActionResult Index()
        {
            ViewBag.IsCSB = Hcsr04Controller.iscsb;
            ViewBag.IsBZ = HJIR2Controller.isbz;
            ViewBag.IsWF = L298NController.iswifi;
            ViewBag.IsUP = L298NController.isup;
            ViewBag.IsDown = L298NController.isdown;
            ViewBag.IsLeft = L298NController.isleft;
            ViewBag.IsRight = L298NController.isright;
            ViewBag.IsPause = L298NController.ispause;
            return View();
        }

        public async Task<IActionResult> SengMsg(string msg = "测试数据")
        {
            await _chatHub.Clients.Group("lz").SendAsync("ReceiveMessage", msg);
            return Content("发出");
        }
    }
}