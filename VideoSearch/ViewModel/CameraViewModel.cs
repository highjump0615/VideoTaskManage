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
            Contents = new CameraViewDetailListModel(owner, this);
        }

        #region utility function
        protected void updateList()
        {
            if (Contents == null || Contents.GetType() == typeof(CameraViewMapModel))
            {
                Contents = new CameraViewDetailListModel(Owner, this);
            }
        }

        public async void AddNewItemAsync()
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
                    await Owner.AddItemAsync(item);
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

                        await ReParent.AddItemAsync(item);
                    }
                }

                // update tree
                Globals.Instance.MainVM.updateTreeList();
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
                Globals.Instance.ShowWaitCursor(true);
                Owner.DeleteSelectedItem();
                Globals.Instance.ShowWaitCursor(false);

                // update tree
                Globals.Instance.MainVM.updateTreeList();
            }
        }

        public void ShowCameraMap()
        {
            Contents = new CameraViewMapModel(this.Owner);
        }

        public void ShowCameraMap(CameraItem item)
        {
            Contents = new CameraViewMapModel(item);
        }

        /// <summary>
        /// 打开摄像头列表
        /// </summary>
        public void ShowCameraDetailList()
        {
            Contents = new CameraViewDetailListModel(Owner, this);
        }

        /// <summary>
        /// 打开标注列表
        /// </summary>
        public void ShowLabelList()
        {
            Contents = new PanelViewListModel(Owner, this);
        }

        /// <summary>
        /// 打开轨迹查询
        /// </summary>
        public void ShowLabelTracking()
        {
            Contents = new PanelViewPathModel(Owner, this);
        }

        /// <summary>
        /// 更新工具栏
        /// </summary>
        private void updateToolbar()
        {
            var viewMain = Globals.Instance.MainVM.View as MainWindow;

            // 导出&删除
            viewMain.ToolbarPanelExport.IsEnabled = false;
            viewMain.ToolbarMarkDelete.IsEnabled = false;

            // 标注列表
            if (Contents is PanelViewListModel)
            {
                viewMain.ToolbarPanelExport.IsEnabled = true;
            }
        }

        public override void contentChanged()
        {
            base.contentChanged();

            // 更新工具栏
            updateToolbar();
        }

        #endregion
    }
}
