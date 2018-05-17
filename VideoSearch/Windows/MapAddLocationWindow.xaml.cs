using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web.Script.Serialization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;

namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for MapAddLocationWindow.xaml
    /// </summary>
    public partial class MapAddLocationWindow : Window
    {
        private WebBrowserOverlay _wbo;

        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public MapAddLocationWindow(double lat, double lng)
        {
            InitializeComponent();

            this.Latitude = lat;
            this.Longitude = lng;

            _wbo = new WebBrowserOverlay(bdMap);
            _wbo.webBrowser.Navigate(Path.Combine(Environment.CurrentDirectory, "Map\\addLocation.html"));

            _wbo.webBrowser.LoadCompleted += new LoadCompletedEventHandler(BrowserLoadCompleted);
        }

        private void BrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            // 地图移动到指定位置
            _wbo.webBrowser.InvokeScript("moveToPosition", this.Latitude, this.Longitude);
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWnd = curApp.MainWindow;
            this.Left = mainWnd.Left + (mainWnd.Width - this.ActualWidth) / 2;
            this.Top = mainWnd.Top + (mainWnd.Height - this.ActualHeight) / 2;
        }

        /// <summary>
        /// 点击确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButOk(object sender, RoutedEventArgs e)
        {
            // 获取当前地图位置
            var strLatLong = _wbo.webBrowser.InvokeScript("getCenterPoint").ToString();
            var objLatLong = new JavaScriptSerializer().Deserialize<Dictionary<String, Double>>(strLatLong);

            this.Latitude = objLatLong["lat"];
            this.Longitude = objLatLong["lng"];

            DialogResult = true;
        }
    }
}
