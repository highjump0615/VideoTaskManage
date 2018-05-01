using System;
using System.Windows;
using System.Windows.Input;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class CameraViewDetailListModel : CameraViewListModel
    {
        /// <summary>
        /// 显示地图命令
        /// </summary>
        public RelayCommandEx ShowItemMapCommand
        {
            get;
            private set;
        }

        public CameraViewDetailListModel(DataItemBase owner, Object parentViewModel = null) : base(owner, parentViewModel)
        {
            ShowItemMapCommand = new RelayCommandEx(ShowMap);
        }

        /// <summary>
        /// 显示地图
        /// </summary>
        /// <param name="sender"></param>
        public void ShowMap(Object parameter)
        {
            //CameraItem item = (CameraItem)parameter;

            //if (ParentViewModel != null && ParentViewModel.GetType() == typeof(CameraViewModel))
            //    ((CameraViewModel)ParentViewModel).ShowCameraMap(item.Longitude, item.Latitude);
        }
    }
}
