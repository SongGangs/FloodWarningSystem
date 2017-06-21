using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
    /// <summary>
    /// 这是记录每个小时的天气信息
    /// </summary>
   public class WeatherHoursMsg
    {
       /// <summary>
       /// 小时时间
       /// </summary>
        public DateTime time { get; set; }
        /// <summary>
        /// 气温
        /// </summary>
        public float temperature { get; set; }
        /// <summary>
        /// 雨量
        /// </summary>
        public float rains { get; set; }
        /// <summary>
        /// 风向
        /// </summary>
        public string wind { get; set; }
        /// <summary>
        /// 风速
        /// </summary>
        public string windL { get; set; }
        /// <summary>
        /// 天气状况
        /// </summary>
        public string weatherStatus { get; set; }
     
    }
}
