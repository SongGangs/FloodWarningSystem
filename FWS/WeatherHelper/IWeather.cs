using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
     public interface IWeather
     {
         /// <summary>
         /// 根据城市名字获取天气信息
         /// </summary>
         /// <param name="name">城市名字</param>
         /// <returns></returns>
         List<WeatherMsg> GetWeatherByName(string name);
         /// <summary>
         /// 返回可用的天气城市
         /// </summary>
         /// <returns></returns>
         ArrayList GetEnableWeather();
     }
}
