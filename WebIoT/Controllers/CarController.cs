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
            return View();
        }

        public async Task<IActionResult> SengMsg(string msg= "测试数据")
        {
            await _chatHub.Clients.Group("3603631297").SendAsync("ReceiveMessage", msg);
            return Content("发出");
        }
    }
}