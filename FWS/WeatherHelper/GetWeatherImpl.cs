using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Documents;
using HtmlAgilityPack;

namespace FWS.WeatherHelper
{
    public class GetWeatherImpl : IGetWeather
    {
        private string url = "http://www.nmc.cn/publish/forecast/";

        public List<IWeatherMsg> GetWeatherByName(string name)
        {
            List<IWeatherMsg> List = new List<IWeatherMsg>();
            WeatherDayMsg dayMsg;
            url = url + name + ".html";
            // url = "http://www.weather.com.cn/weather1d/101270501.shtml";
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
                    if (div[1].ChildNodes[1 + j*2].InnerText.Trim().Contains("日"))
                    {
                        hoursMsg.time = DateTime.Parse(div[1].ChildNodes[1 + j*2].InnerText.Trim());
                        flag = j;
                    }
                    else
                    {
                        hoursMsg.time = flag > j
                            ? DateTime.Parse(div[1].ChildNodes[1 + j*2].InnerText.Trim()).AddDays(k)
                            : DateTime.Parse(div[1].ChildNodes[1 + j*2].InnerText.Trim()).AddDays(k + 1);
                    }
                    hoursMsg.temperature = div[5].ChildNodes[1 + j*2].InnerText.Trim();
                    hoursMsg.rains = div[7].ChildNodes[1 + j*2].InnerText.Trim().Contains("无降水")
                        ? 0.0f
                        : float.Parse(div[7].ChildNodes[1 + j*2].InnerText.Replace("毫米", String.Empty).Trim());
                    hoursMsg.windL = div[9].ChildNodes[1 + j*2].InnerText.Trim();
                    hoursMsg.wind = div[11].ChildNodes[1 + j*2].InnerText.Trim();
                    hoursMsg.humidity = div[15].ChildNodes[1 + j*2].InnerText.Trim();
                    List.Add(hoursMsg);
                }
            }
            return List;
        }


        public WeatherDayMsg GetWeatherByName_Day(string name)
        {
            return new WeatherDayMsg();
        }

        public ArrayList GetEnableWeather()
        {
            ArrayList list = new ArrayList();
            return list;
        }
    }
}
