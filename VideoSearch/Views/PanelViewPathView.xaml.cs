using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using System.Windows.Navigation;
using VideoSearch.Model;
using VideoSearch.ViewModel;

namespace VideoSearch.Views
{
    /// <summary>
    /// Interaction logic for PanelViewPathView.xaml
    /// </summary>
    public partial class PanelViewPathView : UserControl
    {
        public PanelViewPathView()
        {
            InitializeComponent();

            webBrowser.Navigate(Path.Combine(Environment.CurrentDirectory, "Map\\pathMapView.html"));
            webBrowser.LoadCompleted += new LoadCompletedEventHandler(BrowserLoadCompleted);
        }

        private void BrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            var vm = (PanelViewPathModel)this.DataContext;
            var cameraItems = vm.LoadArticles();
            var cameras = new List<CameraSimple>();

            foreach (CameraItem c in cameraItems)
            {
                var simpleCam = new CameraSimple(c);
                cameras.Add(simpleCam);
            }

            var jsonCameraList = new JavaScriptSerializer().Serialize(cameras.ToArray());

            // 地图移动到指定位置
            webBrowser.InvokeScript("setCameraInfo", jsonCameraList);
        }
    }
}
