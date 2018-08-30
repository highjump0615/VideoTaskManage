using System;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;
using VideoSearch.Windows;
using VideoSearch.Utils;
using System.Collections.Generic;

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

            var bHasChildren = false;
            foreach (CameraItem cm in Owner)
            {
                if (cm.IsChecked && cm.Children.Count > 0)
                {
                    bHasChildren = true;
                    break;
                }
            }

            if (bHasChildren)
            {
                MessageBox.Show(Globals.Instance.MainVM.View as MainWindow,
                    "此摄像头内含视频，无法删除有内容的摄像头",
                    "提示",
                    MessageBoxButton.OK,
                    MessageBoxImage.Stop);
                return;
            }

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
            ShowCameraMap(null);
        }

        public void ShowCameraMap(CameraItem item)
        {
            if (AppUtils.CheckForInternetConnection())
            {
                var c = item ?? this.Owner;
                Contents = new CameraViewMapModel(c);
            }
            else
            {
                // 提示
                MessageBox.Show(Globals.Instance.MainVM.View as MainWindow,
                    "连接不到网络，无法显示地图", 
                    "请链接网络",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }
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
        public void ShowLabelTracking(List<ArticleItem> articles)
        {
            if (AppUtils.CheckForInternetConnection())
            {
                Contents = new PanelViewPathModel(Owner, this, articles);
            }
            else
            {
                // 提示
                MessageBox.Show(Globals.Instance.MainVM.View as MainWindow,
                    "连接不到网络，无法显示地图",
                    "请链接网络",
                    MessageBoxButton.OK, MessageBoxImage.Stop);
            }            
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
            viewMain.ToolbarPanelShowPath.IsEnabled = false;
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
