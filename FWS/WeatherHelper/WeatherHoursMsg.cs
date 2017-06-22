using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
    /// <summary>
    /// 这是记录小时的天气信息
    /// 每隔3个小时一次
    /// </summary>
    public class WeatherHoursMsg : IWeatherMsg
    {
        /// <summary>
        /// 小时时间
        /// </summary>
        public DateTime time { get; set; }

        /// <summary>
        /// 气温
        /// </summary>
        public string temperature { get; set; }

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
        /// <summary>
        /// 相对湿度
        /// </summary>
        public string humidity { get; set; }

        /// <summary>
        /// 这个标志代表==1 代表每个小时
        /// </summary>
        public int flag
        {
            get { return 1; }
        }

    }
}
