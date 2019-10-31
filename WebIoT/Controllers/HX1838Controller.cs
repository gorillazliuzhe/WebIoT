using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using System;
using System.Threading.Tasks;
using Unosquare.RaspberryIO;
using Unosquare.RaspberryIO.Abstractions;
using WebIoT.Hubs;
using WebIoT.Peripherals.Infrared;
using WebIoT.Playground;

namespace WebIoT.Controllers
{
    public class HX1838Controller : Controller
    {
        public static string ishw = "stop";
        private readonly IL298NClient _l298n;
        private static InfraredSensor sensor;
        private readonly IHubContext<ChatHub> _chatHub;
        public HX1838Controller(IL298NClient l298n, IHubContext<ChatHub> chatHub)
        {
            _chatHub = chatHub;
            _l298n = l298n;
        }

        public async Task<IActionResult> Start(double speed = 0.8)
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
                        var decdata = BitConverter.ToString(necData);
                        Console.WriteLine($"NEC Data: {decdata.Replace("-", " "),12}    Pulses: {e.Pulses.Length,4}    Duration(us): {e.TrainDurationUsecs,6}    Reason: {e.FlushReason}");

                        #region 遥控器控制小车
                        switch (decdata)
                        {
                            case "DD-22-0B-F4":
                                _l298n.Move(1, speed);
                                L298NController.InitFX();
                                L298NController.isup = "start";
                                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "42", "前进.");
                                break;
                            case "DD-22-0D-F2":
                                _l298n.Move(2, speed);
                                L298NController.InitFX();
                                L298NController.isdown = "start";
                                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "43", "后退.");
                                break;
                            case "DD-22-0E-F1":
                                _l298n.Move(3, speed);
                                L298NController.InitFX();
                                L298NController.isleft = "start";
                                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "44", "左转.");
                                break;
                            case "DD-22-0C-F3":
                                _l298n.Move(4, speed);
                                L298NController.InitFX();
                                L298NController.isright = "start";
                                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "45", "右转.");
                                break;
                            case "DD-22-0A-F5":
                                _l298n.Move(5, 0);
                                L298NController.InitFX();
                                L298NController.ispause = "start";
                                await _chatHub.Clients.All.SendAsync("ReceiveMessage", "46", "暂停.");
                                break;
                        }
                        await _chatHub.Clients.All.SendAsync("ReceiveMessage", "3", $"NEC Data: {BitConverter.ToString(necData).Replace("-", " "),12}");
                        #endregion
                        if (InfraredSensor.NecDecoder.IsRepeatCode(e.Pulses))
                            return;
                    }
                    else
                    {
                        if (e.Pulses.Length >= 4)
                        {
                            var debugData = InfraredSensor.DebugPulses(e.Pulses);
                            //Console.WriteLine($"RX    Length: {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}");
                            //Console.WriteLine($"Debug data: {debugData}");
                            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "3", $"Debug data: {debugData}");
                        }
                        else
                        {
                            //Console.WriteLine($"RX (Garbage): {e.Pulses.Length,5}; Duration: {e.TrainDurationUsecs,7}; Reason: {e.FlushReason}");
                        }
                    }
                };
            }
            ishw = "start";
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "70", "红外遥控器开启");
            return Content("红外遥控器开启");
        }

        /// <summary>
        /// Stop后有问题,暂时不可用关闭,要关闭要重启站点
        /// </summary>
        /// <returns></returns>
        public async Task<IActionResult> Stop()
        {
            ishw = "stop";
            sensor?.Dispose();
            sensor = null;
            await _chatHub.Clients.All.SendAsync("ReceiveMessage", "71", "红外遥控器关闭");
            return Content("红外遥控器关闭");
        }
    }
}