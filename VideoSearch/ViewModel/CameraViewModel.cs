using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;
using VideoSearch.Windows;

namespace VideoSearch.ViewModel
{
    public class CameraViewModel : HostViewModel
    {
        public CameraViewModel(DataItemBase owner)
            : base(owner)
        {
            Contents = new CameraViewListModel(owner, this);
        }

        #region utility function
        protected void updateList()
        {
            if (Contents == null || Contents.GetType() == typeof(CameraViewMapModel))
            {
                Contents = new CameraViewListModel(Owner, this);
            }
        }

        public void AddNewItem()
        {
            if (Owner == null)
                return;

            CreateCameraWindow createDlg = new CreateCameraWindow();
            createDlg.CameraID = Owner.AutoID;
            createDlg.CameraDisplayID = Owner.GetItemDisplayID(createDlg.CameraID);
            createDlg.CameraName = Owner.AutoName;
            createDlg.CameraEventPos = Owner.ID;

            Nullable<bool> result = createDlg.ShowDialog();
            if (result == true)
            {
                updateList();

                CameraItem item = new CameraItem(createDlg.NewCamera);

                if(item.EventPos == Owner.ID)
                    Owner.AddItem(item);
                else
                {
                    DataItemBase ReParent = Owner.FindFriendItem(item.EventPos);

                    if (ReParent != null)
                    {
                        if(Owner.IsSelected)
                        {
                            Owner.IsSelected = false;
                            ReParent.IsSelected = true;
                        }
                        if (Owner.IsExpanded)
                        {
                            Owner.IsExpanded = false;
                            ReParent.IsExpanded = true;
                        }

                        item.DisplayID = ReParent.GetItemDisplayID(item.ID);
                        item.Name = ReParent.AutoName;
                        item.IsChecked = false;
                        item.IsSelected = false;

                        ReParent.AddItem(item);
                    }
                }
            }
        }

        public void DeleteSelectedItems()
        {
            if (Owner == null)
                return;

            if (Owner == null || !Owner.HasCheckedItem)
                return;

            ConfirmDeleteWindow deleteDlg = new ConfirmDeleteWindow();

            Nullable<bool> result = deleteDlg.ShowDialog();

            if (result == true)
            {
                updateList();
                Owner.DeleteSelectedItem();
            }
        }

        public void ShowCameraMap()
        {
            Contents = new CameraViewMapModel();
        }

        public void ShowCameraMap(double longitude, double latitude)
        {
            Contents = new CameraViewMapModel(longitude, latitude);
        }

        public void ShowCameraDetailList()
        {
            Contents = new CameraViewDetailListModel(Owner, this);
        }
        #endregion
    }
}
