using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using FWS.AppCode;
using FWS.utility;

namespace FWS
{
    /// <summary>
    /// CitySelectWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CitySelectWindow : Window
    {
        private static AccessDataBase db = new AccessDataBase();
        private MainWindow m_MainWindow = null;

        public CitySelectWindow(MainWindow mainWindow)
        {
            InitializeComponent();
            this.m_MainWindow = mainWindow;
        }

        private static List<EnArea> m_arealist = null;

        /// <summary>
        /// 根据省份ID通过异步方法获取所有地区信息
        /// </summary>
        /// <param name="ID">省份ID</param>
        /// <returns></returns>
        private static async Task<List<EnArea>> QueryAreaFromDBAsync(int ID)
        {
            List<EnArea> areas = new List<EnArea>();
            EnArea area;
            string sql = "select * from Area where ProvinceID=" + ID + " order by AreaName";
            DataSet ds = db.ReturnDataSet(sql);
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                area = new EnArea();
                area.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                area.ProvinceID = int.Parse(dt.Rows[i]["ProvinceID"].ToString());
                area.AreaID = dt.Rows[i]["AreaID"].ToString();
                area.AreaName = dt.Rows[i]["AreaName"].ToString();
                area.AreaCode = dt.Rows[i]["AreaCode"].ToString();
                areas.Add(area);
            }
            return areas;
        }

        /// <summary>
        /// 通过异步方法获取所有省信息
        /// </summary>
        /// <returns></returns>
        private static async Task<List<EnProvince>> QueryProvinceFromDBAsync()
        {
            List<EnProvince> provinces = new List<EnProvince>();
            EnProvince province;
            string sql = "select * from Province";
            DataSet ds = db.ReturnDataSet(sql);
            DataTable dt = ds.Tables[0];
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                province = new EnProvince();
                province.ID = int.Parse(dt.Rows[i]["ID"].ToString());
                province.ProvinceCode = dt.Rows[i]["ProvinceCode"].ToString();
                province.ProvinceName = dt.Rows[i]["ProvinceName"].ToString();
                provinces.Add(province);
            }
            return provinces;
        }

        private async void Window_Loaded(object sender, RoutedEventArgs e)
        {
            List<EnProvince> provinces = await QueryProvinceFromDBAsync();
            this.ProvinceComboBox.ItemsSource = provinces;
        }

        private async void ProvinceComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            EnProvince province = this.ProvinceComboBox.SelectedItem as EnProvince;
            this.AreaComboBox.ItemsSource = await QueryAreaFromDBAsync(province.ID);
            this.AreaComboBox.SelectedIndex = 0; //选择省份后给他填充所有城市的第一个
        }

        private async void Button_Click(object sender, RoutedEventArgs e)
        {
            if (this.ProvinceComboBox.SelectedIndex < 0)
            {
                MessageBox.Show("请选择省份和城市");
                return;
            }
            string urlcode = this.ProvinceComboBox.SelectedValue.ToString() + "/" +
                             this.AreaComboBox.SelectedValue.ToString() + ".html";
            m_MainWindow.WeatherTempPanel.Visibility=Visibility.Visible;
            EnArea area = this.AreaComboBox.SelectedItem as EnArea;
            await m_MainWindow.ShowWeather(area.ID, area.AreaName, urlcode);
            this.Close();
        }
    }
}
