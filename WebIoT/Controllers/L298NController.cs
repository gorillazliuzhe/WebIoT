using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using WebIoT.Hubs;
using WebIoT.Peripherals.L298N;

namespace WebIoT.Controllers
{
    public class L298NController : Controller
    {
        public static string iswifi = "start"; // 默认开启
        public static string isup = "stop";
        public static string isdown = "stop";
        public static string isleft = "stop";
        public static string isright = "stop";
        public static string ispause = "stop";
        private readonly L298NClient _l298n;
        private readonly IHubContext<ChatHub> _chatHub;
        public L298NController(L298NClient l298n, IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
            _l298n = l298n;
        }

        public async Task<IActionResult> Start()
        {
            _l298n.Start(); iswifi = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "40", "开启WiFi控制.");
            return Content("开启WiFi控制");
        }
        public async Task<IActionResult> Up()
        {
            _l298n.Up();
            InitFX();
            isup = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "42", "前进.");
            return Content("前进");
        }
        public async Task<IActionResult> Down()
        {
            _l298n.Down();
            InitFX();
            isdown = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "43", "后退.");
            return Content("后退");
        }
        public async Task<IActionResult> Left()
        {
            _l298n.Left();
            InitFX();
            isleft = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "44", "左转.");
            return Content("左转");
        }
        public async Task<IActionResult> Right()
        {
            _l298n.Right();
            InitFX();
            isright = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "45", "右转.");
            return Content("右转");
        }
        public async Task<IActionResult> Pause()
        {
            _l298n.Pause();
            InitFX();
            ispause = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "46", "暂停.");
            return Content("暂停");
        }
        public async Task<IActionResult> Stop()
        {
            _l298n.Stop(); iswifi = "stop"; InitFX();
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "41", "停止WiFi控制.");
            return Content("停止WiFi控制");
        }

        /// <summary>
        /// 初始化方向
        /// </summary>
        private void InitFX()
        {
            isup = "stop";
            isdown = "stop";
            isleft = "stop";
            isright = "stop";
            ispause = "stop";
        }
    }
}