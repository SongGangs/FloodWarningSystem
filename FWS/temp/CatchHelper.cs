using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using FWS.utility;
using FWS.WeatherHelper;
using HtmlAgilityPack;

namespace FWS.temp
{
    public class CatchHelper
    {
        private static AccessDataBase db = new AccessDataBase();

        /// <summary>
        /// 获取并存储省信息
        /// </summary>
        public static void GetAndSaveProvince()
        {
            List<string> sqlList = new List<string>();
            string url = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\", "temp\\province.html");
            HtmlWeb web = new HtmlWeb();
            HtmlDocument doc = web.Load(url);
            var htmls = doc.DocumentNode.SelectNodes("//select[@id='provinceSel']");
            do
            {
                doc = web.Load(url);
                htmls = doc.DocumentNode.SelectNodes("//select[@id='provinceSel']");
            } while (htmls == null);
            var option = htmls.First().ChildNodes;
            for (int i = 0; i < option.Count/2 ; i++)
            {
                string sql = String.Format("INSERT INTO Province ([ProvinceCode],[ProvinceName]) VALUES ('{0}','{1}')",
                    option[2*i].Attributes["value"].Value, option[2*i + 1].InnerHtml.Trim());
                sqlList.Add(sql);
            }
            db.insertToAccessByBatch(sqlList);
        }
        /// <summary>
        /// 获取并存储地区信息
        /// </summary>
        public static void GetAndSaveArea()
        {
            List<string> sqlList = new List<string>();
            DataTable dt = db.ReturnDataSet("select * from Province").Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                string url = AppDomain.CurrentDomain.BaseDirectory.Replace("bin\\Debug\\", "temp\\") + dt.Rows[i]["ProvinceCode"].ToString() + ".html";
                HtmlWeb web = new HtmlWeb();
                HtmlDocument doc = web.Load(url);
                var htmls = doc.DocumentNode.SelectNodes("//select[@id='citySel']");
                do
                {
                    doc = web.Load(url);
                    htmls = doc.DocumentNode.SelectNodes("//select[@id='citySel']");
                } while (htmls == null);
                var option = htmls.First().ChildNodes;
                for (int j = 0; j < option.Count / 2; j++)
                {
                    string sql =
                        String.Format(
                            "INSERT INTO Area ([AreaID],[AreaName],[AreaCode],[ProvinceID]) VALUES ('{0}','{1}','{2}','{3}')",
                            option[2*j].Attributes["value"].Value, option[2*j + 1].InnerHtml.Trim(),
                            option[2*j].Attributes["url"].Value.Split('/')[
                                option[2*j].Attributes["url"].Value.Split('/').Length - 1].Replace(".html", String.Empty),
                            dt.Rows[i]["ID"]);
                    sqlList.Add(sql);
                }
            }
            db.insertToAccessByBatch(sqlList);
        }
    }
}
