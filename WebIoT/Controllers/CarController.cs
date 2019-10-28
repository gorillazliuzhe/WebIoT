using Iot.Device.DHTxx;
using Iot.Device.Hcsr04;
using Iot.Units;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using WebIoT.Hubs;
using WebIoT.Peripherals.AM2302;

namespace WebIoT.Controllers
{
    public class CarController : Controller
    {
        private readonly AM2302Client _am2302;
        private readonly IHubContext<ChatHub> _chatHub;
        public CarController(AM2302Client am2302, IHubContext<ChatHub> chatHub)
        {
            _am2302 = am2302;
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
            ViewBag.IsHW = HX1838Controller.ishw;

            #region 温度湿度
            _am2302.OnDataAvailable += async (s, e) =>
            {
                if (!e.IsValid)
                    return;
                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "4", $"{e.Temperature:0.00}°C # {e.HumidityPercentage:00.0}%");
            };
            _am2302.Start();
            #endregion

            return View();
        }
    }
}