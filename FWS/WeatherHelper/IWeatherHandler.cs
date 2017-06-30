using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
    interface IWeatherHandler
    {
        /// <summary>
        /// 盛装 从网上抓取的天气信息
        /// </summary>
        List<IWeatherMsg> WeatherMsgs { get; }
        /// <summary>
        /// 根据返回结果的flag标志 来判断是每天还是每个小时的数据 
        /// 在实例化对应的类
        /// </summary>
        /// <param name="url">城市名字</param>
        /// <returns></returns>
        Task GetWeatherByUrl(string urlcode);
        /// <summary>
        /// 返回可用的天气城市
        /// </summary>
        /// <returns></returns>
        ArrayList GetEnableWeather();
        /// <summary>
        /// 存储天气
        /// </summary>
        /// <param name="msglist">天气信息list</param>
        /// <param name="name">地区名字</param>
         Task SaveWeatherMsg(List<IWeatherMsg> msglist,string name);
        /// <summary>
        /// 存入数据时删除这一区域的数据
        /// </summary>
        /// <param name="name"></param>
         Task DeleteWeatherMsg(string name);
    }
}
