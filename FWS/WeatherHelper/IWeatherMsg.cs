using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{

    public interface IWeatherMsg
    {
        /// <summary>
        /// 用来判断是每个小时的 还是每天的天气数据
        /// flag==1 表示每个小时、 
        /// flag==2 表示每天的
        /// </summary>
        int flag { get;  }
    }
}
