using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWS.utility;
using HtmlAgilityPack;

namespace FWS.EarthquakeHelper
{
    internal class EarthquakeHandlerImpl : IEarthquakeHandler
    {
        public List<EarthquakeMsg> EarthquakeMsgs { get; private set; }
        private static string url = "http://www.ceic.ac.cn/";
        private static AccessDataBase db = new AccessDataBase();

        public async Task GetEarthquakrMsgs()
        {
            EarthquakeMsgs = null;//先初始化
            List<string> provinces = GetProvinceNames();
            List<EarthquakeMsg> list = new List<EarthquakeMsg>();
            EarthquakeMsg earthquakrMsg;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            //找到地震信息的Div
            var htmls = doc.DocumentNode.SelectNodes("//table[@class='news-table']").First();
            do
            {
                doc = web.Load(url);
                htmls = doc.DocumentNode.SelectNodes("//table[@class='news-table']").First();
            } while (htmls == null);
            for (int i = 1; i < htmls.ChildNodes.Count/2; i++)
            {
                var tr = htmls.ChildNodes[2*i + 1];
                //这个行不通  因为画出来的是中国边界的外接矩形
                /*   //中国的经纬度范围大约为：纬度3.86~53.55，经度73.66~135.05
                float lat = float.Parse(tr.ChildNodes[5].InnerText.Trim());
                float lon = float.Parse(tr.ChildNodes[7].InnerText.Trim());
                //如果经纬度不在中国范围内  则不记录这条数据
                if (lat < 3.86 && lat > 53.55 && lon < 73.66 && lon > 135.05)
                    continue;*/
                string position = tr.ChildNodes[11].InnerText.Trim();
                for (int j = 0; j < provinces.Count; j++)
                {
                    if (position.Contains(provinces[j]))
                    {
                        earthquakrMsg = new EarthquakeMsg();
                        earthquakrMsg.level = float.Parse(tr.ChildNodes[1].InnerText.Trim());
                        earthquakrMsg.time = DateTime.Parse(tr.ChildNodes[3].InnerText.Trim());
                        earthquakrMsg.latitude = float.Parse(tr.ChildNodes[5].InnerText.Trim());
                        earthquakrMsg.longitude = float.Parse(tr.ChildNodes[7].InnerText.Trim());
                        earthquakrMsg.depth = float.Parse(tr.ChildNodes[9].InnerText.Trim());
                        earthquakrMsg.position = tr.ChildNodes[11].InnerText.Trim();
                        list.Add(earthquakrMsg);
                    }
                }
            }
            EarthquakeMsgs= list;
        }

        public async Task SaveEarthquakrMsgs(List<EarthquakeMsg> list)
        {
            List<string> sqllist = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {

                string sql =
                    String.Format(
                        "INSERT INTO EarthquakeInfo ([level],[time],[longitude],[latitude],[depth],[position]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}')",
                        list[i].level, list[i].time, list[i].longitude, list[i].latitude, list[i].depth,
                        list[i].position);
                sqllist.Add(sql);
            }
            db.insertToAccessByBatch(sqllist);
        }

        public async Task DeleteEarthquakrMsgs()
        {
            String sql = "DELETE  FROM EarthquakeInfo ";
            db.deleteDt(sql);
        }

        /// <summary>
        /// 获取所以省级名称 去掉省、市、自治区等关键字
        /// </summary>
        /// <returns></returns>
        private static List<string> GetProvinceNames()
        {
            List<string> provinces = new List<string>();
            string sql = "select AliasName from Province";
            DataSet ds = db.ReturnDataSet(sql);
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
                provinces.Add(dt.Rows[i]["AliasName"].ToString());
            return provinces;
        }
    }
}
