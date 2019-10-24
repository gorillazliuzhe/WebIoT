using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using WebIoT.Peripherals.HJIR;

namespace WebIoT.Controllers
{
    public class HJIR2Controller : Controller
    {
        private readonly HJIR2Client _right = new HJIR2Client(8);
        private readonly HJIR2Client _left = new HJIR2Client(7);

        // http://192.168.1.88/HJIR2/HJIR2RightOn
        public IActionResult HJIR2RightOn()
        {
            _right.OnDataAvailable += (s, e) =>
            {
                if (e.HasObstacles)
                {
                    Console.WriteLine("右侧红外避障发现障碍物");
                }
                else
                {
                    //Console.WriteLine("当前前方20cm,右侧红外避障没有发现障碍物");
                }
            };
            _right.Start();
            return Content("右侧红外避障打开");
        }
        public IActionResult HJIR2RightOff()
        {
            _right.Stop();
            return Content("右侧红外避障关闭");
        }

        // http://192.168.1.88/HJIR2/HJIR2LeftOn
        public IActionResult HJIR2LeftOn()
        {
            _left.OnDataAvailable += (s, e) =>
            {
                if (e.HasObstacles)
                {
                    Console.WriteLine("左侧红外避障发现障碍物");
                }
                else
                {
                    //Console.WriteLine("当前前方20cm,左侧红外避障没有发现障碍物");
                }
            };
            _left.Start();
            return Content("左侧红外避障打开");
        }
        public IActionResult HJIR2LeftOff()
        {
            _left.Stop();
            return Content("左侧红外避障关闭");
        }
    }
}