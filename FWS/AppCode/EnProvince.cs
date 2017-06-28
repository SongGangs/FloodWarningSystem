using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FWS.AppCode
{
    /// <summary>
    /// 省份的实体类
    /// </summary>
    class EnProvince
    {
        /// <summary>
        /// id
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// 省份名字
        /// </summary>
        public string ProvinceName { get; set; }
        /// <summary>
        /// 省份识别码 天气预报网
        /// </summary>
        public string ProvinceCode { get; set; }
    }
}
