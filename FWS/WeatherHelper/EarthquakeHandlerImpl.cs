using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using FWS.utility;
using HtmlAgilityPack;

namespace FWS.WeatherHelper
{
    class EarthquakeHandlerImpl:IEarthquakeHandler
    {
        private string url = "http://www.ceic.ac.cn/";
        private AccessDataBase db = new AccessDataBase();
        public List<EarthquakrMsg> GetEarthquakrMsgs()
        {
            List<EarthquakrMsg>list=new List<EarthquakrMsg>();
            EarthquakrMsg earthquakrMsg=new EarthquakrMsg();
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            //找到地震信息的Div
            var htmls = doc.DocumentNode.SelectNodes("//table[@class='news-table']").First();
            do
            {
                doc = web.Load(url);
                htmls = doc.DocumentNode.SelectNodes("//table[@class='news-table']").First();
            } while (htmls==null);
            for (int i = 1; i < htmls.ChildNodes.Count/2; i++)
            {
                var tr = htmls.ChildNodes[2*i + 1];
                //中国的经纬度范围大约为：纬度3.86~53.55，经度73.66~135.05
                float lat = float.Parse(tr.ChildNodes[5].InnerText.Trim());
                float lon = float.Parse(tr.ChildNodes[7].InnerText.Trim());
                //如果经纬度不在中国范围内  则不记录这条数据
                if (lat < 3.86 && lat > 53.55 && lon < 73.66 && lon > 135.05)
                    continue;
                earthquakrMsg=new EarthquakrMsg();
                earthquakrMsg.level = float.Parse(tr.ChildNodes[1].InnerText.Trim());
                earthquakrMsg.time = DateTime.Parse(tr.ChildNodes[3].InnerText.Trim());
                earthquakrMsg.latitude = lat;
                earthquakrMsg.longitude = lon;
                earthquakrMsg.depth = float.Parse(tr.ChildNodes[9].InnerText.Trim());
                earthquakrMsg.position = tr.ChildNodes[11].InnerText.Trim();
                list.Add(earthquakrMsg);
            }
            return list;
        }

        public void SaveEarthquakrMsgs(List<EarthquakrMsg> list)
        {
            List<string> sqllist = new List<string>();
            for (int i = 0; i < list.Count; i++)
            {/*
                    string sql =
                        String.Format(
                            "INSERT INTO DayRainInfo ([time],[maxTemp],[minTemp],[wind],[windL],[alarmmsg],[weatherStatus],[AreaID]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                            dayMsg.time, dayMsg.maxTemp, dayMsg.minTemp, dayMsg.wind, dayMsg.windL, dayMsg.alarmmsg,
                            dayMsg.weatherStatus, id);
                sqllist.Add(sql);*/
            }
            db.insertToAccessByBatch(sqllist);
        }

        public void DeleteEarthquakrMsgs()
        {
          /*  String sql = "DELETE  FROM DayRainInfo ";
            db.deleteDt(sql);*/
        }
    }
}
