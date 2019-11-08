using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebIoT.Hubs;
using WebIoT.Playground;

namespace WebIoT.Controllers
{
    // http://192.168.1.88/Hcsr04/Hcsr04Off
    public class Hcsr04Controller : Controller
    {
        public static string iscsb = "stop";
        private readonly IHcsr04Client _hcsr04;
        private readonly IHubContext<ChatHub> _chatHub;
        public Hcsr04Controller(IHcsr04Client hcsr04, IHubContext<ChatHub> chatHub)
        {
            _hcsr04 = hcsr04;
            _chatHub = chatHub;
        }
        public async Task<IActionResult> Hcsr04On()
        {
            _hcsr04.OnDataAvailable += async (s, e) =>
            {
                if (!e.IsValid)
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "声波没有返回,被折射掉了.");
                }
                else if (e.HasObstacles)
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", $"距离:{e.Distance:N2}cm.");
                }
                else
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "1", "未检测到障碍物.");
                }
            };
            _hcsr04.Start();
            iscsb = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "50", "开启超声波通知.");
            return Content("超声波打开");
        }
        public async Task<IActionResult> Hcsr04Off()
        {
            _hcsr04.Stop();
            iscsb = "stop";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "51", "超声波关闭通知.");
            return Content("超声波关闭");
        }
    }
}