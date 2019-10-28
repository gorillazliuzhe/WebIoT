using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebIoT.Models
{
    public class SiteConfig
    {
        /// <summary>
        /// led灯 默认17
        /// </summary>
        public int LedPin { get; set; }

        /// <summary>
        /// 超声波控制端 默认23
        /// </summary>
        public int TriggerPin { get; set; }

        /// <summary>
        /// 超声波接收端 默认24
        /// </summary>
        public int EchoPin { get; set; }

        /// <summary>
        /// L298N IN1 默认16
        /// </summary>
        public int L298nIn1Pin { get; set; }

        /// <summary>
        /// L298N IN2 默认20
        /// </summary>
        public int L298nIn2Pin { get; set; }

        /// <summary>
        /// L298N IN3 默认21
        /// </summary>
        public int L298nIn3Pin { get; set; }

        /// <summary>
        /// L298N IN4 默认26
        /// </summary>
        public int L298nIn4Pin { get; set; }

        /// <summary>
        /// PWM1 默认18
        /// </summary>
        public int PWM1Pin { get; set; }

        /// <summary>
        /// HX1838 默认25
        /// </summary>
        public int HXPin { get; set; }

        /// <summary>
        /// IR1(left) 默认7
        /// </summary>
        public int IR1Pin { get; set; }

        /// <summary>
        /// IR2(right) 默认8
        /// </summary>
        public int IR2Pin { get; set; }

        /// <summary>
        /// DHT22 默认17
        /// </summary>
        public int DHT22Pin { get;set;}
    }
}
