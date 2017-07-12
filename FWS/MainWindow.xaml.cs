using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Esri.ArcGISRuntime.Controls;
using Esri.ArcGISRuntime.Data;
using Esri.ArcGISRuntime.Geometry;
using Esri.ArcGISRuntime.Layers;
using Esri.ArcGISRuntime.LocalServices;
using Esri.ArcGISRuntime.Symbology;
using FWS.EarthquakeHelper;
using FWS.MapHelper;
using FWS.temp;
using FWS.TreeView;
using FWS.utility;
using FWS.View;
using FWS.WeatherHelper;
using Microsoft.Win32;
using Visifire.Charts;

namespace FWS
{
    /// <summary>
    /// MainWindow.xaml 的交互逻辑
    /// </summary>
    public partial class MainWindow : Window
    {
        private static AccessDataBase db = new AccessDataBase(); //数据库操作帮助类
        private static Dictionary<string, object> weatherDictionary; //获取天气展示类别的信息  字典
        private static IEarthquakeHandler earthquake = new EarthquakeHandlerImpl();
        private static IWeatherHandler weatherHandler = new WeatherHandlerImpl();

        public MainWindow()
        {
            InitializeComponent();
            CrackHelper.Crack();
            DataContext = this;
        }

        private void MsgBtns_Click(object sender, RoutedEventArgs e)
        {

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

        /// <summary>
        /// 地震信息点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EarthquakeMsgBtns_Click(object sender, RoutedEventArgs e)
        {
            
            await earthquake.GetEarthquakrMsgs();
            this.EarthquakeListView.ItemsSource = earthquake.EarthquakeMsgs;
            this.EarthquakeTempPanel.Visibility = Visibility.Visible;
            List<EarthquakeMsg> list = earthquake.EarthquakeMsgs;
            GraphicsLayer graphicsLayer = new GraphicsLayer();
            graphicsLayer.DisplayName = "地震信息点";
            graphicsLayer.ID = "earthquake";
            for (int i = 0; i < list.Count; i++)
            {
                Graphic g = new Graphic();
                Esri.ArcGISRuntime.Geometry.Geometry geometry = new MapPoint(list[i].longitude, list[i].latitude,
                    SpatialReferences.Wgs84);
                g.Geometry = geometry;
                g.Attributes.Add("position", list[i].position);
                g.Symbol = new SimpleMarkerSymbol() {Color = MapHandler.GetRandomColor(), Size = 15};
                graphicsLayer.Graphics.Add(g);
            }
            this.MyMapView.Map.Layers.Add(graphicsLayer);
        }


        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            //时间定时器 实时更新
            DispatcherTimer timer = new DispatcherTimer();
            timer.Tick += new EventHandler(timer_Tick);
            timer.Interval = TimeSpan.FromSeconds(1); //设置刷新的间隔时间
            timer.Start();

        }

        private void timer_Tick(object sender, EventArgs e)
        {
            this.NowTime.Content = DateTime.Now.ToLongDateString() + "    " + DateTime.Now.ToLongTimeString();
        }


        private void WeatherMsgBtns_Click(object sender, RoutedEventArgs e)
        {
            bool isExist = WindowFormUtility.CheckFormIsExist("FWS.CitySelectWindow");
            if (!isExist)
            {
                CitySelectWindow citySelectWindow = new CitySelectWindow(this);
                citySelectWindow.Show();
            }
        }

        private void InqueryMsgBtns_Click(object sender, RoutedEventArgs e)
        {
            //ShowWeather(6418, "南充");
        }

        private void CloseTempBtn_Click(object sender, RoutedEventArgs e)
        {
            Button button = sender as Button;
            switch (button.Tag.ToString())
            {
                case "Weather":
                    this.WeatherTempPanel.Visibility = Visibility.Collapsed;
                    break;
                case "Earthquake":
                    this.EarthquakeTempPanel.Visibility = Visibility.Collapsed;
                    break;
                case "earthquakeMsgDetail":
                    this.earthquakeMsgDetailBorder.Visibility=Visibility.Collapsed;
                    break;
            }
        }

        public void ShowWeatherTempPanel()
        {
            this.WeatherTempPanel.Visibility = Visibility.Visible;
        }


        /// <summary>
        /// 天气预报、天气详情切换按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void TagBtn_Click(object sender, RoutedEventArgs e)
        {
            Color color = Color.FromRgb(236, 238, 165);
            Color color2 = Color.FromRgb(255, 255, 255);
            Button btn = sender as Button;
            if (btn.Tag.ToString().Contains("forecast")) //如果点击的是天气预报
            {
                (this.TagBtn_forecast.Template.FindName("TagBtnBorder", this.TagBtn_forecast) as Border).Background =
                    new SolidColorBrush(color);
                (this.TagBtn_detail.Template.FindName("TagBtnBorder2", this.TagBtn_detail) as Border).Background =
                    new SolidColorBrush(color2);
                this.WeatherForecastPanel.Visibility = Visibility.Visible;
                this.WeatherDetailPanel.Visibility = Visibility.Collapsed;
            }
            else
            {
                (this.TagBtn_forecast.Template.FindName("TagBtnBorder", this.TagBtn_forecast) as Border).Background =
                    new SolidColorBrush(color2);
                (this.TagBtn_detail.Template.FindName("TagBtnBorder2", this.TagBtn_detail) as Border).Background =
                    new SolidColorBrush(color);
                this.WeatherDetailPanel.Visibility = Visibility.Visible;
                this.WeatherForecastPanel.Visibility = Visibility.Collapsed;
            }
        }

        /// <summary>
        /// 异步更新天气导数据库
        /// </summary>
        /// <param name="name"></param>
        /// <param name="list"></param>
        private async Task SaveWeatherMsgAsync(string name)
        {
           
            await weatherHandler.DeleteWeatherMsg(name);
            await weatherHandler.SaveWeatherMsg(weatherHandler.WeatherMsgs, name);
        }

        /// <summary>
        /// 获取天气展示类别的信息
        /// </summary>
        /// <param name="id">城市ID</param>
        /// <param name="name">城市名字</param>
        /// <returns></returns>
        private async Task GetWeatherByCity(int id, string name, string urlcode)
        {
            weatherDictionary = null; //先初始化为空。
            await weatherHandler.GetWeatherByUrl(urlcode);
            List<IWeatherMsg> list = weatherHandler.WeatherMsgs;
            Dictionary<string, object> dic = new Dictionary<string, object>();
            List<WeatherItem> weatherLists = new List<WeatherItem>();
            WeatherItem weatherItem = new WeatherItem();

            #region 读取数据库

            /*
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
                weatherItem.areaID = id;
                weatherItem.date = DateTime.Parse(dt.Rows[i]["time"].ToString()).ToString("MM月dd日");
                weatherItem.maxTemperature = dt.Rows[i]["maxTemp"].ToString();
                weatherItem.minTemperature = dt.Rows[i]["minTemp"].ToString();
                string weatherStatus = dt.Rows[i]["weatherStatus"].ToString().Split('转')[0].Trim();
                weatherItem.weatherStatus = weatherStatus;
                weatherItem.imageSrc = "Images/night/晚间" + GetWeatherImgSrcByWeatherStatus(weatherStatus);
                weatherLists.Add(weatherItem);
            }
            dic.Add("weatherItem", weatherLists);
             */

            #endregion

            if (list.Count == 0)
            {
                MessageBox.Show("暂无天气数据");
                return;
            }
            for (int i = 0; i < list.Count; i++)
            {
                if (list[i].flag == 2) //每天的数据
                {
                    WeatherDayMsg dayMsg = list[i] as WeatherDayMsg;
                    if (dayMsg.time == DateTime.Today) //今天的数据
                    {
                        if (string.IsNullOrEmpty(dayMsg.maxTemp)) //如果只有半天数据
                        {
                            OnlyDayWeatherInfo od = new OnlyDayWeatherInfo();
                            od.city = name;
                            od.date = dayMsg.time.ToString("MM月dd日");
                            od.week = dayMsg.time.ToString("dddd");
                            od.weatherStatus = dayMsg.weatherStatus;
                            od.imgSrc = "Images/night/晚间" + GetWeatherImgSrcByWeatherStatus(dayMsg.weatherStatus);
                            od.temperature = dayMsg.minTemp;
                            od.wind = dayMsg.wind;
                            od.windL = dayMsg.windL;
                            dic.Add("onlyday", od);
                        }
                        else
                        {
                            DayAndNightWeatherInfo dn = new DayAndNightWeatherInfo();
                            dn.city = name;
                            dn.date = dayMsg.time.ToString("MM月dd日");
                            dn.week = dayMsg.time.ToString("dddd");
                            dn.weatherStatus_day = dayMsg.weatherStatus.Split('转')[0].Trim();
                            dn.weatherStatus_night = dayMsg.weatherStatus.Split('转')[1].Trim();
                            dn.imgSrc_day = "Images/day/日间" + GetWeatherImgSrcByWeatherStatus(dn.weatherStatus_day);
                            dn.imgSrc_night = "Images/night/晚间" +
                                              GetWeatherImgSrcByWeatherStatus(dn.weatherStatus_night);
                            dn.maxTemperature = dayMsg.maxTemp;
                            dn.minTemperature = dayMsg.minTemp;
                            dn.wind_day = dayMsg.wind.Split('转')[0].Trim();
                            dn.wind_night = dayMsg.wind.Split('转')[1].Trim();
                            dn.windL_day = dayMsg.windL.Split('转')[0].Trim();
                            dn.windL_night = dayMsg.windL.Split('转')[1].Trim();
                            dic.Add("dayandnight", dn);
                        }
                    }
                    else //之后7几天的数据
                    {
                        weatherItem = new WeatherItem();
                        weatherItem.areaID = id;
                        weatherItem.date = dayMsg.time.ToString("MM月dd日");
                        weatherItem.maxTemperature = dayMsg.maxTemp;
                        weatherItem.minTemperature = dayMsg.minTemp;
                        string weatherStatus = dayMsg.weatherStatus.Split('转')[0].Trim();
                        weatherItem.weatherStatus = weatherStatus;
                        weatherItem.imageSrc = "../Images/night/晚间" + GetWeatherImgSrcByWeatherStatus(weatherStatus);
                        weatherLists.Add(weatherItem);
                    }
                }
            }
            dic.Add("weatherItem", weatherLists);
            weatherDictionary = dic;
        }

        public async Task ShowWeather(int id, string name, string urlcode)
        {
            try
            {
                await GetWeatherByCity(id, name, urlcode);
                Dictionary<string, object> dic = weatherDictionary;
                this.WeatherForecastListView.ItemsSource = dic["weatherItem"] as List<WeatherItem>;
                if (dic.ContainsKey("onlyday"))
                {
                    //天气预报
                    this.dayandnightPanel.Visibility = Visibility.Collapsed;
                    this.onlynightPanel.Visibility = Visibility.Visible;
                    this.WeatherForecastPanel.DataContext = dic["onlyday"] as OnlyDayWeatherInfo;
                    //天气详情
                    this.dayandnightPanel_detail.Visibility = Visibility.Collapsed;
                    this.onlynightPanel_detail.Visibility = Visibility.Visible;
                    this.WeatherDetailPanel.DataContext = dic["onlyday"] as OnlyDayWeatherInfo;
                }
                else if (dic.ContainsKey("dayandnight"))
                {
                    //天气预报
                    this.dayandnightPanel.Visibility = Visibility.Visible;
                    this.onlynightPanel.Visibility = Visibility.Collapsed;
                    this.WeatherForecastPanel.DataContext = dic["dayandnight"] as DayAndNightWeatherInfo;
                    //天气详情
                    this.dayandnightPanel_detail.Visibility = Visibility.Visible;
                    this.onlynightPanel_detail.Visibility = Visibility.Collapsed;
                    this.WeatherDetailPanel.DataContext = dic["dayandnight"] as DayAndNightWeatherInfo;
                }
                //await SaveWeatherMsgAsync(name);

                //图表渲染
                await RendererCharts(id, DateTime.Parse(this.NowTime.Content.ToString()).ToString("MM月dd日"));
            }
            catch (Exception)
            {
            }

        }

        /// <summary>
        /// 渲染图表
        /// </summary>
        /// <param name="areaID"></param>
        /// <param name="date"></param>
        /// <returns></returns>
        private async Task RendererCharts(int areaID, string date)
        {
            string sql = String.Format("select * from HoursRainInfo where AreaID={0} and day=#{1}#", areaID,
                DateTime.Parse(date));
            DataTable dt = db.ReturnDataSet(sql).Tables[0];
            string Charttitle = date + "天气详情";
            List<DateTime> LsTime = new List<DateTime>();
            List<string> temperatures = new List<string>();
            for (int i = 0; i < dt.Rows.Count; i++)
            {
                LsTime.Add(DateTime.Parse(dt.Rows[i]["time"].ToString()));
                temperatures.Add(dt.Rows[i]["temperature"].ToString().Replace("℃", String.Empty));
            }
            Simon.Children.Clear();
            CreateChartSpline(Charttitle, LsTime, temperatures);
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

        /// <summary>
        /// 天气预报中的 预报信息列表 点击事件
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void WeatherForecastListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            WeatherItem weatherItem = this.WeatherForecastListView.SelectedItem as WeatherItem;
            if (weatherItem == null)
            {
                MessageBox.Show("点击的天气暂无信息。");
                return;
            }
            RendererCharts(weatherItem.areaID, weatherItem.date);
            TagBtn_Click(this.TagBtn_detail, null);
        }

        /// <summary>
        /// 地震信息
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private async void EarthquakeListView_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (earthquakeMsgDetailBorder.Visibility == Visibility.Visible)
                earthquakeMsgDetailBorder.Visibility = Visibility.Collapsed;
            EarthquakeMsg msg = this.EarthquakeListView.SelectedItem as EarthquakeMsg;
            if (msg == null)
            {
                MessageBox.Show("点击的地震信息暂无信息。");
                return;
            }
            GraphicsLayer layer = this.MyMapView.Map.Layers["earthquake"] as GraphicsLayer;
            if (layer == null)
            {
                return;
            }
            foreach (Graphic g in layer.Graphics)
            {
                if (g.Attributes["position"].Equals(msg.position))
                {
                    g.IsSelected = true;
                    await this.MyMapView.SetViewAsync(g.Geometry.Extent.GetCenter(), 100000);
                }
                else
                {
                    g.IsSelected = false;
                }
            }
        }

        #region 折线图

        public void CreateChartSpline(string Charttitle, List<DateTime> lsTime, List<string> temperatures)
        {
            //创建一个图标
            Chart chart = new Chart();

            //设置图标的宽度和高度
            chart.Width = 285;
            chart.Height = 300;
            //是否启用打印和保持图片
            chart.ToolBarEnabled = true;

            //设置图标的属性
            chart.ScrollingEnabled = false; //是否启用或禁用滚动
            chart.View3D = true; //3D效果显示

            //创建一个标题的对象
            Title title = new Title();

            //设置标题的名称
            title.Text = Charttitle;
            title.Padding = new Thickness(0, 10, 5, 0);

            //向图标添加标题
            chart.Titles.Add(title);

            //初始化一个新的Axis
            Axis xaxis = new Axis();
            //设置Axis的属性
            //图表的X轴坐标按什么来分类，如时分秒
            xaxis.IntervalType = IntervalTypes.Hours;
            //图表的X轴坐标间隔如2,3,20等，单位为xAxis.IntervalType设置的时分秒。
            xaxis.Interval = 3;
            //设置X轴的时间显示格式为7-10 11：20           
            xaxis.ValueFormatString = "HH时";
            //给图标添加Axis            
            chart.AxesX.Add(xaxis);

            Axis yAxis = new Axis();
            //设置图标中Y轴的最小值永远为0           
            yAxis.AxisMinimum = 0;
            //设置图表中Y轴的后缀          
            yAxis.Suffix = "℃";
            chart.AxesY.Add(yAxis);

            // 创建一个新的数据线。               
            DataSeries dataSeries = new DataSeries();
            // 设置数据线的格式。               
            dataSeries.LegendText = "温度";

            dataSeries.RenderAs = RenderAs.Spline; //折线图

            dataSeries.XValueType = ChartValueTypes.DateTime;
            // 设置数据点              
            DataPoint dataPoint;
            for (int i = 0; i < lsTime.Count; i++)
            {
                // 创建一个数据点的实例。                   
                dataPoint = new DataPoint();
                // 设置X轴点                    
                dataPoint.XValue = lsTime[i];
                //设置Y轴点                   
                dataPoint.YValue = double.Parse(temperatures[i]);
                dataPoint.MarkerSize = 8;
                //dataPoint.Tag = tableName.Split('(')[0];
                //设置数据点颜色                  
                // dataPoint.Color = new SolidColorBrush(Colors.LightGray);                   
                dataPoint.MouseLeftButtonDown += new MouseButtonEventHandler(dataPoint_MouseLeftButtonDown);
                //添加数据点                   
                dataSeries.DataPoints.Add(dataPoint);
            }

            // 添加数据线到数据序列。                
            chart.Series.Add(dataSeries);

            //将生产的图表增加到Grid，然后通过Grid添加到上层Grid.           
            Grid gr = new Grid();
            gr.Children.Add(chart);

            Simon.Children.Add(gr);
        }

        #endregion

        #region 点击事件

        //点击事件
        private void dataPoint_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            //DataPoint dp = sender as DataPoint;
            //MessageBox.Show(dp.YValue.ToString());
        }

        #endregion

        #region 添加数据到地图

        private async void AddLayerDataBtn_OnClick(object sender, RoutedEventArgs e)
        {
            try
            {
                OpenFileDialog openFileDialog = new OpenFileDialog();
                openFileDialog.Filter = "Shapefiles|*.shp|Image files|*.bmp;*.png;*.sid;*.tif";
                openFileDialog.RestoreDirectory = true;
                openFileDialog.Multiselect = false;
                openFileDialog.Title = "文件选取";
                if (openFileDialog.ShowDialog() == true)
                {
                    string item = openFileDialog.SafeFileNames[0].Split('.')[1];
                    WorkspaceFactoryType type = WorkspaceFactoryType.Raster;
                    switch (item)
                    {
                        case "shp":
                            type = WorkspaceFactoryType.Shapefile;
                            break;
                        case "bmp":
                        case "png":
                        case "sid":
                        case "tif":
                            type = WorkspaceFactoryType.Raster;
                            break;
                    }

                    var dynLayer =
                        await MapHandler.AddFileDatasetToDynamicMapServiceLayer(type,
                            System.IO.Path.GetDirectoryName(openFileDialog.FileName),
                            new List<string>(openFileDialog.SafeFileNames), this.MyMapView.Map.Layers.Count);
                    // Add the dynamic map service layer to the map
                    if (dynLayer != null)
                    {
                        dynLayer.DisplayName = dynLayer.DynamicLayerInfos[0].Name;
                        MyMapView.Map.Layers.Add(dynLayer);
                        MyMapView.SetView(dynLayer.FullExtent.Extent.Expand(0.5));
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message, "Sample Error");
            }
        }

        private async void ZoomInBtn_OnClick(object sender, RoutedEventArgs e)
        {
            await ChangeMainMapSacleAsync(1);
        }
        private async void ZoomOutBtn_OnClick(object sender, RoutedEventArgs e)
        {
            await ChangeMainMapSacleAsync(2);
        }
        private async void PanBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.MyMapView.Cursor = Cursors.Hand;
        }
        private async void FullExtentBtn_OnClick(object sender, RoutedEventArgs e)
        {
            this.MyMapView.Cursor = Cursors.Arrow;
            menuZoomIn_Click(null, null);
        }

        private async Task LoadShapefile(string path)
        {
            try
            {
                // open shapefile table
                var shapefile = await ShapefileTable.OpenAsync(path);
                // clear existing map and spatial reference
                if (MyMapView.Map.Layers.Any())
                {
                    // MyMapView.Map.Layers.Clear();
                    MyMapView.Map = new Map();
                }
                // create feature layer based on the shapefile
                var flayer = new FeatureLayer(shapefile)
                {
                    ID = shapefile.Name,
                    DisplayName = path,
                };

                // Add the feature layer to the map
                MyMapView.Map.Layers.Add(flayer);
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error creating feature layer: " + ex.Message, "Sample Error");
            }
        }

        #endregion

        #region 地图缩略图

        public async void OverviewMap_LayerLoaded(object sender, LayerLoadedEventArgs e)
        {
            await AddSingleGraphicAsync();
        }


        public async void MyMapView_ExtentChanged(object sender, System.EventArgs e)
        {
            var graphicsOverlay = overviewMap.GraphicsOverlays["overviewOverlay"];

            Graphic g = graphicsOverlay.Graphics.FirstOrDefault();
            if (g == null) //first time
            {
                g = new Graphic();
                graphicsOverlay.Graphics.Add(g);
            }
            var currentViewpoint = MyMapView.GetCurrentViewpoint(ViewpointType.BoundingGeometry);
            var viewpointExtent = currentViewpoint.TargetGeometry.Extent;
            g.Geometry = viewpointExtent;

            await overviewMap.SetViewAsync(viewpointExtent.GetCenter(), MyMapView.Scale*15);
        }

        private async Task AddSingleGraphicAsync()
        {
            try
            {
                await MyMapView.LayersLoadedAsync();

                var graphicsOverlay = overviewMap.GraphicsOverlays["overviewOverlay"];
                Symbol symbol = symbol = Resources["RedFillSymbol"] as Symbol;
                while (true)
                {
                    var geometry = await overviewMap.Editor.RequestShapeAsync(DrawShape.Rectangle, symbol);
                    graphicsOverlay.Graphics.Clear();

                    var graphic = new Graphic(geometry, symbol);
                    graphicsOverlay.Graphics.Add(graphic);

                    var viewpointExtent = geometry.Extent;
                    await MyMapView.SetViewAsync(viewpointExtent);
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore cancellations from selecting new shape type
            }
            catch (Exception ex)
            {
            }
        }

        /// <summary>
        /// 改变主视图的范围  放大缩小
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        private async Task ChangeMainMapSacleAsync(int type)
        {
            try
            {
                await MyMapView.LayersLoadedAsync();
                var graphicsOverlay = MyMapView.GraphicsOverlays["overviewOverlay"];
                Symbol symbol = symbol = Resources["OutlineSymbol"] as Symbol;
                while (true)
                {
                    var geometry = await MyMapView.Editor.RequestShapeAsync(DrawShape.Rectangle, symbol);
                    graphicsOverlay.Graphics.Clear();

                    var graphic = new Graphic(geometry, symbol);
                    graphicsOverlay.Graphics.Add(graphic);

                    var viewpointExtent = geometry.Extent;
                    if (type==1)
                    {
                        await MyMapView.SetViewAsync(viewpointExtent);
                    }
                    else
                    {
                        await MyMapView.SetViewAsync(viewpointExtent.GetCenter(), MyMapView.Scale * 5);
                    }
                    graphicsOverlay.Graphics.Clear();
                }
            }
            catch (TaskCanceledException)
            {
                // Ignore cancellations from selecting new shape type
            }
            catch (Exception ex)
            {
            }
        }

        private void overviewMap_LayerLoaded_1(object sender, LayerLoadedEventArgs e)
        {

        }

        #endregion

        #region 用TreeView+属性改变事件  效率不好

        /*
        private static int m_LayerCount = 0;
        private void MyMapView_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            DependencyPropertyDescriptor dpd = DependencyPropertyDescriptor.FromProperty(MapView.MapProperty, typeof(MapView));
            if (dpd != null)
            {
                int count = this.MyMapView.Map.Layers.Count;
                if (count>m_LayerCount)
                {
                    m_LayerCount = count;
                    BindTreeViewDataContent(m_LayerCount);
                }
               
            }
        }

        private async void BindTreeViewDataContent(int i)
        {
                IList<TreeModel> list = new List<TreeModel>();
                for (int j = 0; j < m_LayerCount; j++) { 
                TreeModel tree1 = new TreeModel();
                tree1.Id = this.MyMapView.Map.Layers[j].ID;
                tree1.Name = this.MyMapView.Map.Layers[j ].DisplayName;
                list.Add(tree1);
                }
                MyTreeView.ItemsSourceData = list;
        }
        */

        #endregion


        /// <summary>
        /// 缩放至图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuZoomIn_Click(object sender, RoutedEventArgs e)
        {
            if (LayersListView.SelectedItem != null)
            {
                string id = (LayersListView.SelectedItem as Layer).ID.ToString();
                MyMapView.SetView(MyMapView.Map.Layers[id].FullExtent.GetCenter(), 5000);
            }
        }

        /// <summary>
        /// 移除图层
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void menuRemoveLayer_Click(object sender, RoutedEventArgs e)
        {
            if (LayersListView.SelectedItem != null)
            {
                string id = (LayersListView.SelectedItem as Layer).ID.ToString();
                MyMapView.Map.Layers.Remove(id);
            }
        }



        private void MyMapView_OnMapViewTapped(object sender, MapViewInputEventArgs e)
        {
            List<EarthquakeMsg> list = earthquake.EarthquakeMsgs;
            if (list==null)
                return;
            for (int i = 0; i < list.Count; i++)
            {
                var normalizedPoint = GeometryEngine.NormalizeCentralMeridian(e.Location);
                var projectedCenter = GeometryEngine.Project(normalizedPoint, SpatialReferences.Wgs84) as MapPoint;
                float.Parse(projectedCenter.X.ToString("0.00"));
                if (float.Parse(projectedCenter.X.ToString("0.00"))== list[i].longitude && float.Parse(projectedCenter.Y.ToString("0.00")) == list[i].latitude)
                {
                    double margin_left = e.Position.X - this.earthquakeMsgDetailBorder.ActualWidth;
                    double margin_top = e.Position.Y - this.earthquakeMsgDetailBorder.ActualHeight * 0.5;
                    double margin_right = this.MyMapView.ActualWidth - margin_left - this.earthquakeMsgDetailBorder.ActualWidth;
                    double margin_bottom = this.MyMapView.ActualHeight - margin_top - this.earthquakeMsgDetailBorder.ActualHeight;
                    this.earthquakeMsgDetailBorder.Margin = new Thickness(margin_left, margin_top, margin_right, margin_bottom);
                    this.earthquakeMsgDetailBorder.Visibility=Visibility.Visible;
                    this.earthquakeMsgDetailBorder.DataContext = list[i];
                }
            }
             
        }
    }
}
