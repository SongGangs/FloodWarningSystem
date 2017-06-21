using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
   public class WeatherDayMsg
    {
       /// <summary>
       /// 最高温度
       /// </summary>
        public string maxTemp { get; set; }
       /// <summary>
       /// 最低温低
       /// </summary>
        public string minTemp { get; set; }
       /// <summary>
       /// 日出时间
       /// </summary>
        public DateTime sunSet { get; set; }
       /// <summary>
       /// 日落时间
       /// </summary>
        public DateTime sunRise { get; set; }
    }
}
