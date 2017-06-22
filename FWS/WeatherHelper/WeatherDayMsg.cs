using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
    public class WeatherDayMsg : IWeatherMsg
    {
        /// <summary>
        /// 时间
        /// </summary>
        public DateTime time { get; set; }

        /// <summary>
        /// 最高温度
        /// </summary>
        public string maxTemp { get; set; }

        /// <summary>
        /// 最低温低
        /// </summary>
        public string minTemp { get; set; }

        /// <summary>
        /// 风向
        /// </summary>
        public string wind { get; set; }

        /// <summary>
        /// 风速
        /// </summary>
        public string windL { get; set; }
        /*
        /// <summary>
        /// 相对湿度
        /// </summary>
        public string humidity { get; set; }

        /// <summary>
        /// 日出时间
        /// </summary>
        public DateTime sunSet { get; set; }

        /// <summary>
        /// 日落时间
        /// </summary>
        public DateTime sunRise { get; set; }
        /// <summary>
        /// 降水
        /// </summary>
        public string rains { get; set; }

        /// <summary>
        /// 空气质量
        /// </summary>
        public string level { get; set; }
        */
        /// <summary>
        /// 预警
        /// </summary>
        public string alarmmsg { get; set; }
        /// <summary>
        /// 天气状况
        /// </summary>
        public string weatherStatus { get; set; }

        /// <summary>
        /// 这个标志代表==2 代表每天
        /// </summary>
        public int flag
        {
            get { return 2; }
        }
    }
}
