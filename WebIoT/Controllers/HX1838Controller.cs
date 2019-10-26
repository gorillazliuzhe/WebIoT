using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using WebIoT.Hubs;
using WebIoT.Peripherals.Infrared;
using WebIoT.Peripherals.L298N;

namespace WebIoT.Controllers
{
    public class HX1838Controller : Controller
    {
        public static string ishw = "stop";
        private readonly L298NClient _l298n;
        private static InfraredSensor sensor;
        private readonly IHubContext<ChatHub> _chatHub;
        public HX1838Controller(L298NClient l298n, IHubContext<ChatHub> chatHub)
        {

            _chatHub = chatHub;
            _l298n = l298n;
        }

        public async Task<IActionResult> Start()
        {
            if (ishw == "stop")
            {
                var inputPin = Pi.Gpio[BcmPin.Gpio25]; // BCM Pin 25 or Physical pin 22 on the right side of the header.
                sensor = new InfraredSensor(inputPin, true);
                sensor.DataAvailable += async (s, e) =>
                {
                    Console.Clear();
                    var necData = InfraredSensor.NecDecoder.DecodePulses(e.Pulses);
                    if (necData != null)
                    {
                        Console.WriteLine($"NEC Data: {BitConverter.ToString(necData).Replace("-", " "),12}    Pulses: {e.Pulses.Length,4}    Duration(us): {e.TrainDurationUsecs,6}    Reason: {e.FlushReason}");

                        await _chatHub.Clients.All.SendAsync("ReceiveMessage", "3", $"NEC Data: {BitConverter.ToString(necData).Replace("-", " "),12}");

                        if (InfraredSensor.NecDecoder.IsRepeatCode(e.Pulses))
                            return;
                    }
                    else
                    {
                        if (e.Pulses.Length >= 4)
                        {
                            var debugData = InfraredSensor.DebugPulses(e.Pulses);
                            Console.WriteLine($"RX    Length: {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}");
                            Console.WriteLine($"Debug data: {debugData}");
                            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "3", $"Debug data: {debugData}");
                        }
                        else
                        {
                            Console.WriteLine($"RX (Garbage): {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}");
                        }
                    }

                    Console.WriteLine("红外线");
                };
            }           
            ishw = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "70", "红外遥控器开启");
            return Content("红外遥控器开启");
        }
        public async Task<IActionResult> Stop()
        {
            ishw = "stop";
            sensor.Dispose();
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "71", "红外遥控器关闭");
            return Content("红外遥控器关闭");
        }
    }
}