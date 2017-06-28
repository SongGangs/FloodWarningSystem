using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.AppCode
{
    class EnArea
    {
        /// <summary>
        /// id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// id识别码 rest用的着
        /// </summary>
        public string AreaID { get; set; }
        /// <summary>
        /// 区域名字
        /// </summary>
        public string AreaName { get; set; }
        /// <summary>
        /// 区域识别码 天气预报网
        /// </summary>
        public string AreaCode { get; set; }
        /// <summary>
        /// 和省份 ID 的关联
        /// </summary>
        public int ProvinceID { get; set; }

    }
}
