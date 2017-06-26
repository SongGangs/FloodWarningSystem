using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Documents;
using FWS.utility;
using HtmlAgilityPack;

namespace FWS.WeatherHelper
{
    class WeatherHandlerImpl:IWeatherHandler
    {

        /// <summary>
        /// 为什么这个mdb这么奇怪
        /// 各种sql语句语法错误呀！！
        /// 明明时正确的  
        /// 反而要用一些奇葩的语句！
        /// </summary>

        private string url = "http://www.nmc.cn/publish/forecast/";
        private AccessDataBase db=new AccessDataBase();
        public List<IWeatherMsg> GetWeatherByName(string name)
        {
            string code = db.GetCodeByName(name);
            if (code==null)
            {
                MessageBox.Show("当前地区无天气数据");
                return null;
            }
            url = url + code+".html";
            List<IWeatherMsg> List = new List<IWeatherMsg>();
            WeatherDayMsg dayMsg;
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            //记录每天的数据（一周）
            var htmls = doc.DocumentNode.SelectNodes("//div[@class='today']");
            do
            {
                doc = web.Load(url);
                htmls = doc.DocumentNode.SelectNodes("//div[@class='today']");
            } while (htmls == null);
            /*   //删除注释，script，style
            htmls.Descendants()
                .Where(n => n.Name == "script" || n.Name == "style" || n.Name == "#comment")
                .ToList().ForEach(n => n.Remove());*/
            for (int k = 0; k < htmls.Count; k++)
            {
                dayMsg = new WeatherDayMsg();
                var tr = htmls[k].ChildNodes[1].ChildNodes[1].ChildNodes;
                dayMsg.alarmmsg = doc.DocumentNode.SelectNodes("//a[@id='realWarn']").First().InnerHtml;
                if (k == 0 && tr[5].ChildNodes.Count == 3)
                {

                    dayMsg.time = DateTime.Parse(tr[1].ChildNodes[3].InnerText.Trim());
                    dayMsg.weatherStatus = tr[5].ChildNodes[1].InnerText.Trim();
                    dayMsg.minTemp = tr[7].ChildNodes[1].InnerText.Trim();
                    dayMsg.wind = tr[9].ChildNodes[1].InnerText.Trim();
                    dayMsg.windL = tr[11].ChildNodes[1].InnerText.Trim();
                    List.Add(dayMsg);
                    continue;
                }
                for (int i = 0; i < tr.Count; i++)
                {
                    switch (i)
                    {
                        case 1:
                            dayMsg.time = k < 3
                                ? DateTime.Parse(tr[i].ChildNodes[3].InnerText.Trim())
                                : DateTime.Parse(tr[i].ChildNodes[1].InnerText.Trim());
                            break;
                        case 5:
                            dayMsg.weatherStatus = tr[i].ChildNodes[1].InnerText.Trim() + " 转 " +
                                                   tr[i].ChildNodes[3].InnerText.Trim();
                            break;
                        case 7:
                            dayMsg.maxTemp = tr[i].ChildNodes[1].InnerText.Trim();
                            dayMsg.minTemp = tr[i].ChildNodes[3].InnerText.Trim();
                            break;
                        case 9:
                            dayMsg.wind = tr[i].ChildNodes[1].InnerText.Trim() + " 转 " +
                                          tr[i].ChildNodes[3].InnerText.Trim();
                            break;
                        case 11:
                            dayMsg.windL = tr[i].ChildNodes[1].InnerText.Trim() + " 转 " +
                                           tr[i].ChildNodes[3].InnerText.Trim();
                            break;
                    }
                }
                List.Add(dayMsg);
            }


            //记录每隔3小时的数据
            htmls = doc.DocumentNode.SelectNodes("//*[@id='hour3']/div[@class='hour3']");
            do
            {
                doc = web.Load(url);
                htmls = doc.DocumentNode.SelectNodes("//*[@id='hour3']/div[@class='hour3']");
            } while (htmls == null);
            //    List<WeatherHoursMsg> hoursList = new List<WeatherHoursMsg>();
            WeatherHoursMsg hoursMsg;
            for (int k = 0; k < htmls.Count; k++)
            {
                var div = htmls[k].ChildNodes;
                int flag = 9; //临时变量 用来判断时间是否已经变了一天
                for (int j = 1; j < 9; j++)
                {
                    hoursMsg = new WeatherHoursMsg();
                    if (div[1].ChildNodes[1 + j * 2].InnerText.Trim().Contains("日"))
                    {
                        hoursMsg.time = DateTime.Parse(div[1].ChildNodes[1 + j * 2].InnerText.Trim());
                        flag = j;
                    }
                    else
                    {
                        hoursMsg.time = flag > j
                            ? DateTime.Parse(div[1].ChildNodes[1 + j * 2].InnerText.Trim()).AddDays(k)
                            : DateTime.Parse(div[1].ChildNodes[1 + j * 2].InnerText.Trim()).AddDays(k + 1);
                    }
                    hoursMsg.temperature = div[5].ChildNodes[1 + j * 2].InnerText.Trim();
                    hoursMsg.rains = div[7].ChildNodes[1 + j * 2].InnerText.Trim().Contains("无降水")
                        ? 0.0f
                        : float.Parse(div[7].ChildNodes[1 + j * 2].InnerText.Replace("毫米", String.Empty).Trim());
                    hoursMsg.windL = div[9].ChildNodes[1 + j * 2].InnerText.Trim();
                    hoursMsg.wind = div[11].ChildNodes[1 + j * 2].InnerText.Trim();
                    hoursMsg.humidity = div[15].ChildNodes[1 + j * 2].InnerText.Trim();
                    List.Add(hoursMsg);
                }
            }
            return List;
        }
        public ArrayList GetEnableWeather()
        {
            throw new NotImplementedException();
        }


      
        public void SaveWeatherMsg(List<IWeatherMsg> msglist,string name)
        {
            List<string> sqllist = new List<string>();
           int id= db.GetAreaIDByName(name);
            if (id==0)
            {
                MessageBox.Show("该地区暂无天气信息");
                return;
            }
            for (int i = 0; i < msglist.Count; i++)
            {
                string sql;
                if (msglist[i].flag==1)//小时
                {
                    WeatherHoursMsg hoursMsg=msglist[i] as WeatherHoursMsg;
                    sql =
                        String.Format(
                            "INSERT INTO HoursRainInfo ([time],[temperature],[rains],[wind],[windL],[humidity],[AreaID]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}')",
                            hoursMsg.time, hoursMsg.temperature, hoursMsg.rains, hoursMsg.wind, hoursMsg.windL,
                            hoursMsg.humidity, id);
                }
                else
                {
                    WeatherDayMsg dayMsg = msglist[i] as WeatherDayMsg;
                    sql =
                        String.Format(
                            "INSERT INTO DayRainInfo ([time],[maxTemp],[minTemp],[wind],[windL],[alarmmsg],[weatherStatus],[AreaID]) VALUES ('{0}','{1}','{2}','{3}','{4}','{5}','{6}','{7}')",
                            dayMsg.time, dayMsg.maxTemp, dayMsg.minTemp, dayMsg.wind, dayMsg.windL, dayMsg.alarmmsg,
                            dayMsg.weatherStatus, id);
                }
                sqllist.Add(sql);
            }
            db.insertToAccessByBatch(sqllist);
        }

        public void DeleteWeatherMsg(string name)
        {
            int id = db.GetAreaIDByName(name);
            string sql = "DELETE  FROM HoursRainInfo WHERE AreaID = " + id ;
            db.deleteDt(sql);
            sql = "DELETE  FROM DayRainInfo WHERE AreaID = " + id ;
            db.deleteDt(sql);
        }
    }
}
