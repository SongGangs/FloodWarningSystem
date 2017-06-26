using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
    /// <summary>
    /// 地震信息
    /// </summary>
   public class EarthquakrMsg
    {
       /// <summary>
       /// 震级
       /// </summary>
       public float level { get ;set ;}
       /// <summary>
       /// 时间
       /// </summary>
       public DateTime time { get; set; }
       /// <summary>
       /// 经度
       /// </summary>
       public float longitude { get; set; }
       /// <summary>
       /// 纬度
       /// </summary>
       public float latitude { get; set; }
       /// <summary>
       /// 深度
       /// </summary>
       public float depth { get; set; }
       /// <summary>
       /// 位置
       /// </summary>
       public string position { get; set; }

    }
}
