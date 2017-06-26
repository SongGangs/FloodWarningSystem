using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.WeatherHelper
{
    public interface IEarthquakeHandler
    {
        /// <summary>
        /// 获取地震信息
        /// 从中国地震网上抓取
        /// 最近一个月的
        /// </summary>
        /// <returns></returns>
        List<EarthquakrMsg> GetEarthquakrMsgs();
        /// <summary>
        /// 存储到数据库
        /// </summary>
        /// <param name="list">读取的地震信息</param>
        void SaveEarthquakrMsgs( List<EarthquakrMsg> list);
        /// <summary>
        /// 删除记录
        /// </summary>
        void DeleteEarthquakrMsgs();
    }
}
