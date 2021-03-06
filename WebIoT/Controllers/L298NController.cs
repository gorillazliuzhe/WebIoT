﻿using Iot.Device.DCMotor;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System.Threading.Tasks;
using WebIoT.Hubs;
using WebIoT.Playground;

namespace WebIoT.Controllers
{
    public class L298NController : Controller
    {
        public static string iswifi = "start"; // 默认开启
        public static string isup = "stop";
        public static string isdown = "stop";
        public static string isleft = "stop";
        public static string isright = "stop";
        public static string isdownleft = "stop";
        public static string isdownright = "stop";
        public static string ispause = "stop";
        private readonly IL298NClient _l298n;
        private readonly IHubContext<ChatHub> _chatHub;
        public L298NController(IL298NClient l298n, IHubContext<ChatHub> chatHub)
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
        public async Task<IActionResult> Up(double speed = 0.6)
        {
            _l298n.Move(1, speed);
            InitFX();
            isup = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "42", "前进.");
            return Content("前进");
        }
        public async Task<IActionResult> Down(double speed = 0.6)
        {
            _l298n.Move(2, speed);
            InitFX();
            isdown = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "43", "后退.");
            return Content("后退");
        }
        // 前左转
        public async Task<IActionResult> Left(double speed = 0.6)
        {
            _l298n.Move(3, speed);
            InitFX();
            isleft = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "44", "左转.");
            return Content("左转");
        }
        // 前右转
        public async Task<IActionResult> Right(double speed = 0.6)
        {
            _l298n.Move(4, speed);
            InitFX();
            isright = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "45", "右转.");
            return Content("右转");
        }
        // 后左转
        public async Task<IActionResult> DownLeft(double speed = 0.6)
        {
            _l298n.Move(6, speed);
            InitFX();
            isdownleft = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "47", "后左转.");
            return Content("左转");
        }
        // 后右转
        public async Task<IActionResult> DownRight(double speed = 0.6)
        {
            _l298n.Move(7, speed);
            InitFX();
            isdownright = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "48", "后右转.");
            return Content("后右转");
        }
        public async Task<IActionResult> Pause()
        {
            _l298n.Move(5, 0);
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
        public static void InitFX()
        {
            isup = "stop";
            isdown = "stop";
            isleft = "stop";
            isright = "stop";
            ispause = "stop";
            isdownleft = "stop";
            isdownright = "stop";
        }
    }
}