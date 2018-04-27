using System;
using System.Collections.Generic;
using System.Linq;
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
    }
}
