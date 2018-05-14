using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.ServiceProcess;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    class MainViewModel : ViewModelBase
    {
        /// <summary>
        /// 是否显示加载提示
        /// </summary>
        private Visibility _visibilityWorkMask = Visibility.Collapsed;
        public Visibility VisibilityWorkMask
        {
            get { return _visibilityWorkMask; }
            set {
                _visibilityWorkMask = value;
                PropertyChanging("VisibilityWorkMask");
            }
        }

        /// <summary>
        /// 提示内容
        /// </summary>
        private String _workMaskNotice = "正在加载...";
        public String WorkMaskNotice
        {
            get { return _workMaskNotice; }
            set
            {
                _workMaskNotice = value;
                PropertyChanging("WorkMaskNotice");
            }
        }

        /// <summary>
        /// 是否显示进度条
        /// </summary>
        private Visibility _visibilityWorkMaskProgress = Visibility.Visible;
        public Visibility VisibilityWorkMaskProgress
        {
            get { return _visibilityWorkMaskProgress; }
            set
            {
                _visibilityWorkMaskProgress = value;
                PropertyChanging("VisibilityWorkMaskProgress");
            }
        }

        /// <summary>
        /// 是否显示整体加载提示
        /// </summary>
        private Visibility _visibilityTotalMask = Visibility.Collapsed;
        public Visibility VisibilityTotalMask
        {
            get { return _visibilityTotalMask; }
            set
            {
                _visibilityTotalMask = value;
                PropertyChanging("VisibilityTotalMask");
                PropertyChanging("TotalAreaEnabled");
            }
        }

        /// <summary>
        /// 整体提示内容
        /// </summary>
        private String _totalMaskNotice = "正在加载...";
        public String TotalMaskNotice
        {
            get { return _totalMaskNotice; }
            set
            {
                _totalMaskNotice = value;
                PropertyChanging("TotalMaskNotice");
            }
        }

        /// <summary>
        /// 是否整体显示进度条
        /// </summary>
        private Visibility _visibilityTotalMaskProgress = Visibility.Visible;
        public Visibility VisibilityTotalMaskProgress
        {
            get { return _visibilityTotalMaskProgress; }
            set
            {
                _visibilityTotalMaskProgress = value;
                PropertyChanging("VisibilityTotalMaskProgress");
            }
        }

        /// <summary>
        /// 整体是否能用状态
        /// </summary>
        public bool TotalAreaEnabled
        {
            get
            {
                return VisibilityTotalMask == Visibility.Collapsed;
            }
        }

        public MainViewModel()
        {
        }

        /// <summary>
        /// 检查服务
        /// </summary>
        public void checkService()
        {
            // 服务是否在运行
            try
            {
                var strServiceName = ConfigurationManager.AppSettings["MainServiceName"];
                var service = new ServiceController(strServiceName);

                if (service.Status != ServiceControllerStatus.Running)
                {
                    ShowTotalMask("服务未启动，正在启动...");

                    // 启动服务
                    service.Start();
                    service.WaitForStatus(ServiceControllerStatus.Running);

                    HideTotalMask();
                }
            }
            catch (Exception e)
            {
                if (e is InvalidOperationException)
                {
                    ShowTotalMask("服务未安装，应用没法使用，请安装服务", false);
                }                
            }
            
        }

        /// <summary>
        /// 显示加载中提示
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="withProgress"></param>
        public void ShowWorkMask(String notice = "", bool withProgress = true)
        {
            VisibilityWorkMask = Visibility.Visible;

            if (!String.IsNullOrEmpty(notice))
            {
                WorkMaskNotice = notice;
            }
            VisibilityWorkMaskProgress = withProgress ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 隐藏加载中提示
        /// </summary>
        public void HideWorkMask()
        {
            VisibilityWorkMask = Visibility.Collapsed;
        }

        /// <summary>
        /// 显示加载中提示
        /// </summary>
        /// <param name="notice"></param>
        /// <param name="withProgress"></param>
        public void ShowTotalMask(String notice = "", bool withProgress = true)
        {
            VisibilityTotalMask = Visibility.Visible;

            if (!String.IsNullOrEmpty(notice))
            {
                TotalMaskNotice = notice;
            }
            VisibilityTotalMaskProgress = withProgress ? Visibility.Visible : Visibility.Hidden;
        }

        /// <summary>
        /// 隐藏加载中提示
        /// </summary>
        public void HideTotalMask()
        {
            VisibilityTotalMask = Visibility.Collapsed;
        }
    }
}
