using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.Threading;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Peripherals;
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
            ViewBag.IsPause = L298NController.ispause;
            ViewBag.IsHW = HX1838Controller.ishw;

            #region 温度湿度

            Task.Run(() =>
            {
                var sensor = DhtSensor.Create(DhtType.Dht22, Pi.Gpio[_dthpin]);
                sensor.OnDataAvailable += async (s, e) =>
                {
                    if (!e.IsValid)
                        return;
                    await _chatHub.Clients.All.SendAsync("ReceiveMessage", "4", $"{e.Temperature:0.00}°C # {e.HumidityPercentage:00.0}%");
                };
                sensor.Start();
            }, cancellationToken);


            //Task.Run(async () =>
            //{
            //    using (Iot.Device.DHTxx.Dht22 dht = new Iot.Device.DHTxx.Dht22(_dthpin))
            //    {
            //        while (true)
            //        {
            //            Temperature temperature = dht.Temperature;
            //            double humidity = dht.Humidity;
            //            if (!double.IsNaN(temperature.Celsius) && !double.IsNaN(humidity))
            //            {
            //                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "4", $"{temperature.Celsius:0.00}°C # {humidity:00.0}%");
            //            }
            //            Thread.Sleep(2000);
            //        }
            //    }
            //});
            #endregion

            return View();
        }
    }
}