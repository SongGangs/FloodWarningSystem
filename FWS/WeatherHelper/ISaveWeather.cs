using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
   public interface ISaveWeather
   {
       /// <summary>
       /// 存储今天的天气
       /// </summary>
       /// <param name="msg"></param>
       void SaveWeatherDayMsg(WeatherDayMsg msg);
       /// <summary>
       /// 存储每个小时的天气
       /// </summary>
       /// <param name="msg"></param>
       void SaveWeatherHoursMsg(WeatherHoursMsg msg);

   }
}
