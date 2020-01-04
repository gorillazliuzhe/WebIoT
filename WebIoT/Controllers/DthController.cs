using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Peripherals;
using WebIoT.Hubs;
using WebIoT.Models;

namespace WebIoT.Controllers
{
    public class DthController : Controller
    {
        private readonly IHubContext<ChatHub> _chatHub;
        private readonly int _dthpin;
        public DthController(IOptions<SiteConfig> option, IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
            _dthpin = option.Value.DHT22Pin;
        }
        public IActionResult Index(CancellationToken cancellationToken)
        {
            //Task.Run(async () => // 会出现Nan情况
            //{
            //    using Iot.Device.DHTxx.Dht22 dht = new Iot.Device.DHTxx.Dht22(_dthpin);
            //    while (true)
            //    {
            //        Iot.Units.Temperature temperature = dht.Temperature;
            //        double humidity = dht.Humidity;
            //        if (!double.IsNaN(temperature.Celsius) && !double.IsNaN(humidity))
            //        {
            //            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "dth", $"{temperature.Celsius:0.00}#{humidity:00.0}");
            //        }
            //        Thread.Sleep(2000);
            //    }
            //}, cancellationToken);
            var sensor = DhtSensor.Create(DhtType.Dht22, Pi.Gpio[_dthpin]);
            sensor.OnDataAvailable += async (s, e) =>
            {
                if (!e.IsValid)
                    return;
                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "dth", $"{e.Temperature:0.00}#{e.HumidityPercentage:00.0}");
            };
            sensor.Start();
            return View();
        }
    }
}