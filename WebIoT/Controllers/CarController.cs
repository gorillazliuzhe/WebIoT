using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
//using Unosquare.RaspberryIO;
//using Unosquare.RaspberryIO.Peripherals;
using WebIoT.Hubs;
using WebIoT.Models;

namespace WebIoT.Controllers
{
    public class CarController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly int _dthpin;
        public CarController(IOptions<SiteConfig> option, IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
            _dthpin = option.Value.DHT22Pin;
        }
        public IActionResult Index(CancellationToken cancellationToken)
        {
            ViewBag.IsCSB = Hcsr04Controller.iscsb;
            ViewBag.IsBZ = HJIR2Controller.isbz;
            ViewBag.IsWF = L298NController.iswifi;
            ViewBag.IsUP = L298NController.isup;
            ViewBag.IsDown = L298NController.isdown;
            ViewBag.IsLeft = L298NController.isleft;
            ViewBag.IsRight = L298NController.isright;
            ViewBag.IsDownLeft = L298NController.isdownleft;
            ViewBag.IsDownRight = L298NController.isdownright;
            ViewBag.IsPause = L298NController.ispause;
            ViewBag.IsHW = HX1838Controller.ishw;

            #region 温度湿度

            //Task.Run(async () => // 会出现Nan情况
            //{
            //    using Iot.Device.DHTxx.Dht22 dht = new Iot.Device.DHTxx.Dht22(_dthpin);
            //    while (true)
            //    {
            //        await Task.Delay(5000);
            //        if (dht.TryReadTemperature(out UnitsNet.Temperature temperature) && dht.TryReadHumidity(out UnitsNet.RelativeHumidity humidity))
            //        {
            //            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "4", $"{temperature:0.00} # {humidity:00.0}", cancellationToken);
            //        }         
            //    }
            //});

            //Task.Run(() =>  // 第三方库,没有NAN情况
            //{
            //    var sensor = DhtSensor.Create(DhtType.Dht22, Pi.Gpio[_dthpin]);
            //    sensor.OnDataAvailable += async (s, e) =>
            //    {
            //        if (!e.IsValid)
            //            return;
            //        await _chatHub.Clients.All.SendAsync("ReceiveMessage", "4", $"{e.Temperature:0.00}°C # {e.HumidityPercentage:00.0}%");
            //    };
            //    sensor.Start();
            //}, cancellationToken);

            #endregion

            return View();
        }
    }
}