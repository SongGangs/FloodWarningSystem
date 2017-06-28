using System;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using FWS.EarthquakeHelper;
using FWS.temp;
using FWS.utility;
using FWS.View;
using FWS.WeatherHelper;

namespace FWS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private AccessDataBase db = new AccessDataBase();
        public MainWindow()
        {
            InitializeComponent();
        }

        private void MsgBtns_Click(object sender, RoutedEventArgs e)
        {
            this.EarthquakeTempPanel.Visibility = Visibility.Visible;
            /* 
           IEarthquakeHandler earthquakeObj=new EarthquakeHandlerImpl();
           List<EarthquakeMsg>list= earthquakeObj.GetEarthquakrMsgs();
           earthquakeObj.DeleteEarthquakrMsgs();
           earthquakeObj.SaveEarthquakrMsgs(list);
            
            IWeatherHandler weatherObj=new WeatherHandlerImpl();
            List<IWeatherMsg> list = weatherObj.GetWeatherByName("南充");
            weatherObj.DeleteWeatherMsg("南充");
            weatherObj.SaveWeatherMsg(list,"南充");*/
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            this.NowTime.Content = DateTime.Now.ToLongDateString()+"    " + DateTime.Now.ToLongTimeString();
        }

        private void WeatherMsgBtns_Click(object sender, RoutedEventArgs e)
        {
            bool isExist= WindowFormUtility.CheckFormIsExist("FWS.CitySelectWindow");
            if (!isExist)
            {
                CitySelectWindow citySelectWindow=new CitySelectWindow(this.WeatherTempPanel);
                citySelectWindow.Show();
            }
        }
        private void InqueryMsgBtns_Click(object sender, RoutedEventArgs e)
        {
            ShowWeather(6418, "南充");
        }
        private void CloseTempBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button=sender as Button;
            switch (button.Tag.ToString())
            {
                case "Weather":
                    this.WeatherTempPanel.Visibility = Visibility.Hidden;
                    break;
                case "Earthquake":
                    this.EarthquakeTempPanel.Visibility = Visibility.Hidden;
                    break;
            }
        }

        public void ShowWeatherTempPanel()
        {
            this.WeatherTempPanel.Visibility = Visibility.Visible;
        }


        private void TagBtn_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromRgb(236, 238, 165);
            Color color2 = Color.FromRgb(255, 255, 255);
            Button btn = sender as Button;
            if (btn.Tag.ToString().Contains("forecast"))
            {
                (this.TagBtn_forecast.Template.FindName("TagBtnBorder", this.TagBtn_forecast) as Border).Background = new SolidColorBrush(color);
                (this.TagBtn_detail.Template.FindName("TagBtnBorder2", this.TagBtn_detail) as Border).Background = new SolidColorBrush(color2);
            }
            else
            {
                (this.TagBtn_forecast.Template.FindName("TagBtnBorder", this.TagBtn_forecast) as Border).Background = new SolidColorBrush(color2);
                (this.TagBtn_detail.Template.FindName("TagBtnBorder2", this.TagBtn_detail) as Border).Background = new SolidColorBrush(color);
            }
        }

        /// <summary>
        /// 获取天气展示类别的信息
        /// </summary>
        /// <param name="id">城市ID</param>
        /// <param name="name">城市名字</param>
        /// <returns></returns>
        private Dictionary<string,object> GetWeatherByCity(int id,string name)
        {
            Dictionary<string,object> dic=new Dictionary<string, object>();
            List<WeatherItem> weatherLists=new List<WeatherItem>();
            WeatherItem weatherItem = new WeatherItem();
            string sql = "select * from DayRainInfo where AreaID=" + id;
            DataSet ds = db.ReturnDataSet(sql);
            DataTable dt = ds.Tables[0];
            if (dt.Rows.Count == 0)
            {
                MessageBox.Show("暂无天气数据");
                return null;
            }
            if (string.IsNullOrEmpty(dt.Rows[0]["maxTemp"].ToString()))
            {
                OnlyDayWeatherInfo od = new OnlyDayWeatherInfo();
                od.city = name;
                od.date = DateTime.Parse(dt.Rows[0]["time"].ToString()).ToString("MM月dd日");
                od.week = DateTime.Parse(dt.Rows[0]["time"].ToString()).ToString("dddd");
                od.imgSrc = "Images/night/晚间" + GetWeatherImgSrcByWeatherStatus(dt.Rows[0]["weatherStatus"].ToString().Split('转')[0].Trim());
                od.temperature = dt.Rows[0]["minTemp"].ToString();
                od.wind = dt.Rows[0]["wind"].ToString().Trim();
                od.windL = dt.Rows[0]["windL"].ToString().Trim();
                dic.Add("onlyday", od);
            }
            else
            {
                DayAndNightWeatherInfo dn = new DayAndNightWeatherInfo();
                dn.city = name;
                dn.date = DateTime.Parse(dt.Rows[0]["time"].ToString()).ToString("MM月dd日");
                dn.week = DateTime.Parse(dt.Rows[0]["time"].ToString()).ToString("dddd");
                dn.imgSrc_day="Images/day/日间"+ GetWeatherImgSrcByWeatherStatus(dt.Rows[0]["weatherStatus"].ToString().Split('转')[0].Trim());
                dn.imgSrc_night="Images/night/夜间"+ GetWeatherImgSrcByWeatherStatus(dt.Rows[0]["weatherStatus"].ToString().Split('转')[0].Trim());
                dn.maxTemperature = dt.Rows[0]["maxTemp"].ToString();
                dn.minTemperature = dt.Rows[0]["minTemp"].ToString();
                dn.wind_day = dt.Rows[0]["wind"].ToString().Split('转')[0].Trim();
                dn.wind_night = dt.Rows[0]["wind"].ToString().Split('转')[1].Trim();
                dn.windL_day = dt.Rows[0]["windL"].ToString().Split('转')[0].Trim();
                dn.windL_night = dt.Rows[0]["windL"].ToString().Split('转')[1].Trim();
                dic.Add("dayandnight",dn);
            }
            for (int i = 1; i < dt.Rows.Count; i++)
            {
                weatherItem = new WeatherItem();
                weatherItem.date = DateTime.Parse(dt.Rows[i]["time"].ToString()).ToString("MM月dd日");
                weatherItem.maxTemperature = dt.Rows[i]["maxTemp"].ToString();
                weatherItem.minTemperature = dt.Rows[i]["minTemp"].ToString();
                string weatherStatus = dt.Rows[i]["weatherStatus"].ToString().Split('转')[0].Trim();
                weatherItem.weatherStatus = weatherStatus;
                weatherItem.imageSrc = "Images/night/晚间" + GetWeatherImgSrcByWeatherStatus(weatherStatus);
                weatherLists.Add(weatherItem);
            }
            dic.Add("weatherItem", weatherLists);
            return dic;
        }

        public void ShowWeather(int id,string name)
        {
            try
            {
                Dictionary<string, object> dic = GetWeatherByCity(id, name);
                this.ListView.ItemsSource = dic["weatherItem"] as List<WeatherItem>;
                if (dic.ContainsKey("onlyday"))
                {
                    this.dayandnightPanel.Visibility = Visibility.Collapsed;
                    this.onlynightPanel.Visibility = Visibility.Visible;
                    this.NowWeatherPanel.DataContext = dic["onlyday"] as OnlyDayWeatherInfo;
                }
                else if (dic.ContainsKey("dayandnight"))
                {
                    this.dayandnightPanel.Visibility = Visibility.Visible;
                    this.onlynightPanel.Visibility = Visibility.Collapsed;
                    this.NowWeatherPanel.DataContext = dic["dayandnight"] as DayAndNightWeatherInfo;
                }
            }
            catch (Exception)
            {
            }
            
        }
        /// <summary>
        /// 根据天气状况名字返回对应的图片地址
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        private static string GetWeatherImgSrcByWeatherStatus(string name)
        {
            string result = "";
            switch (name)
            {
                case "大雨":
                    result = "大雨.png";
                    break;
                case "中雨":
                    result = "中雨.png";
                    break;
                case "小雨":
                    result = "小雨.png";
                    break;
                case "阵雨":
                    result = "阵雨.png";
                    break;
                case "雷阵雨":
                    result = "雷阵雨.png";
                    break;
                case "多云":
                    result = "多云.png";
                    break;
                case "晴":
                    result = "晴.png";
                    break;
                case "阴":
                    result = "阴.png";
                    break;
                default:
                     result = "晴.png";
                    break;
            }
            return result;
        }
    }
}
