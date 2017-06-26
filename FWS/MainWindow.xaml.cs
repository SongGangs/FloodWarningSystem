using System;
using System.Collections.Generic;
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
using FWS.temp;
using FWS.WeatherHelper;

namespace FWS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
           IEarthquakeHandler earthquakeObj=new EarthquakeHandlerImpl();
            earthquakeObj.GetEarthquakrMsgs();
            /* 
            IWeatherHandler weatherObj=new WeatherHandlerImpl();
            List<IWeatherMsg> list = weatherObj.GetWeatherByName("南充");
            weatherObj.DeleteWeatherMsg("南充");
            weatherObj.SaveWeatherMsg(list,"南充");*/


        }
      
    }
}
