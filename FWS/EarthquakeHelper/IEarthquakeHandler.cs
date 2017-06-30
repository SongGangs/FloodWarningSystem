using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.EarthquakeHelper
{
    public interface IEarthquakeHandler
    {
        /// <summary>
        /// 盛装 从网上抓取的地震信息
        /// </summary>
        List<EarthquakeMsg> EarthquakeMsgs { get; }
        /// <summary>
        /// 获取地震信息
        /// 从中国地震网上抓取
        /// 最近一个月的
        /// </summary>
        /// <returns></returns>
        Task GetEarthquakrMsgs();
        /// <summary>
        /// 存储到数据库
        /// </summary>
        /// <param name="list">读取的地震信息</param>
        Task SaveEarthquakrMsgs(List<EarthquakeMsg> list);
        /// <summary>
        /// 删除记录
        /// </summary>
        Task DeleteEarthquakrMsgs();
    }
}
