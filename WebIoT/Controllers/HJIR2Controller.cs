using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using WebIoT.Hubs;
using WebIoT.Peripherals.HJIR;
using WebIoT.Playground.HJIR;

namespace WebIoT.Controllers
{
    public class HJIR2Controller : Controller
    {
        public static string isbz = "stop";
        //private readonly HJR2RightClient _right;
        //private readonly HJIR2LeftClient _left;
        private readonly IHJR2Client _hjr2;
        private readonly IHubContext<ChatHub> _chatHub;
        public HJIR2Controller(IHubContext<ChatHub> chatHub, IHJR2Client hjr2)
        {
            _hjr2 = hjr2;
            _chatHub = chatHub;
        }
        public async Task<IActionResult> HJIR2On()
        {
            _hjr2.OnDataAvailable += async (s, e) =>
            {
                bool l = false; bool r = false;
                foreach (var item in e)
                {
                    if (item.IsValid && item.HasObstacles)
                    {
                        switch (item.type)
                        {
                            case 1:
                                l = true;
                                break;
                            case 2:
                                r = true;
                                break;
                        }
                    }
                }
                if (l && r)
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "2", "左右红外避障发现障碍物");
                }
                else if (l)
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "2", "左红外避障发现障碍物");
                }
                else if (r)
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "2", "右红外避障发现障碍物");
                }
                else
                {
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "2", "none");
                }

            };
            _hjr2.Start();
            isbz = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "60", "红外避障打开通知");
            return Content("红外避障打开");
        }
        public async Task<IActionResult> HJIR2Off()
        {
            _hjr2.Stop();
            isbz = "stop";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "61", "红外避障关闭通知");
            return Content("红外避障关闭");
        }
    }
}