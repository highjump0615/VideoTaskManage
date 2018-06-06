using System;
using System.Collections.Generic;
using System.IO;
using System.Web.Script.Serialization;
using System.Windows.Controls;
using System.Windows.Navigation;
using VideoSearch.Model;
using VideoSearch.ViewModel;
using VideoSearch.Windows;

namespace VideoSearch.Views
{
    /// <summary>
    /// Interaction logic for CameraViewMapView.xaml
    /// </summary>
    public partial class CameraViewMapView : UserControl
    {
        public CameraViewMapView()
        {
            InitializeComponent();

            this.webBrowser.Navigate(Path.Combine(Environment.CurrentDirectory, "Map\\cameraMapView.html"));
            this.webBrowser.LoadCompleted += new LoadCompletedEventHandler(BrowserLoadCompleted);
        }

        private void BrowserLoadCompleted(object sender, NavigationEventArgs e)
        {
            var vm = (CameraViewMapModel)this.DataContext;
            var cameras = new List<CameraSimple>();

            if (vm.Owner is CameraItem cItem)
            {
                // 单摄像头
                var simpleCam = new CameraSimple(cItem);
                cameras.Add(simpleCam);
            }
            else
            {
                // 摄像头列表
                foreach (DataItemBase data in vm.Owner)
                {
                    var cameraData = (CameraItem)data;

                    var simpleCam = new CameraSimple(cameraData);
                    cameras.Add(simpleCam);
                }
            }

            var jsonCameraList = new JavaScriptSerializer().Serialize(cameras.ToArray());

            // 地图移动到指定位置
            this.webBrowser.InvokeScript("setCameraInfo", jsonCameraList);
        }
    }
}
