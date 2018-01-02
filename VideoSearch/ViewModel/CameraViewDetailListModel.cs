using System;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class CameraViewDetailListModel : CameraViewListModel
    {
        public CameraViewDetailListModel(DataItemBase owner, Object parentViewModel = null) : base(owner, parentViewModel)
        {
        }

        public void ShowMap(DataItemBase sender)
        {
            if (sender != null)
            {
                if (sender.GetType() == typeof(CameraItem))
                {
                    CameraItem item = (CameraItem)sender;

                    if (ParentViewModel != null && ParentViewModel.GetType() == typeof(CameraViewModel))
                        ((CameraViewModel)ParentViewModel).ShowCameraMap(item.Longitude, item.Latitude);
                }
            }
        }
    }
}
