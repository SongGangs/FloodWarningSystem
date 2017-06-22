﻿using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
     public interface IGetWeather
     {
         /// <summary>
         /// 根据返回结果的flag标志 来判断是每天还是每个小时的数据 
         /// 在实例化对应的类
         /// </summary>
         /// <param name="name">城市名字</param>
         /// <returns></returns>
         List<IWeatherMsg> GetWeatherByName(string name);
         /// <summary>
         /// 返回可用的天气城市
         /// </summary>
         /// <returns></returns>
         ArrayList GetEnableWeather();
     }
}