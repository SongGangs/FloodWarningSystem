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
       /// 存储天气
       /// </summary>
       /// <param name="msg"></param>
       void SaveWeatherMsg(IWeatherMsg msg);

   }
}
