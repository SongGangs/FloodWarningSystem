using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
   public class WeatherImpl : IWeather
   {
       private  string url = "http://www.webxml.com.cn/WebServices/WeatherWebService.asmx";
       public List<WeatherMsg> GetWeatherByName(string name)
       {
        List<WeatherMsg>list=new List<WeatherMsg>();
        return list;
       }

       public ArrayList GetEnableWeather()
       {
           ArrayList list=new ArrayList();
           return list;
       }
    }
}
