using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class PanelViewSummaryBaseModel : ObservableObject
    {
        protected MovieTaskListItemBase _owner = null;

        private List<TaskSnapshot> _snapshots = null;
        public List<TaskSnapshot> Snapshots
        {
            get { return _snapshots; }
            set
            {
                _snapshots = value;
                PropertyChanging("Snapshots");

                // 更新visible
                PropertyChanging("GridVisible");
                PropertyChanging("NoticeVisible");
            }
        }

        #region Property

        private String _title = "";

        public String Title
        {
            get { return _title; }
            set
            {
                _title = value;
                PropertyChanging("Title");
            }
        }

        private int _displayType = 0;
        public int DisplayType
        {
            get { return _displayType; }

            set
            {
                if (_displayType != value && _snapshots != null)
                {
                    _displayType = value;

                    foreach (TaskSnapshot snapshot in _snapshots)
                    {
                        snapshot.DisplayType = _displayType;
                    }

                    if (_owner != null)
                        _owner.DisplayType = _displayType;
                }
            }
        }

        private int _itemSizeIndex = 0;
        public int ItemSizeIndex
        {
            get { return _itemSizeIndex; }
            set
            {
                if (_itemSizeIndex != value)
                {
                    _itemSizeIndex = value;

                    foreach (TaskSnapshot snapshot in _snapshots)
                    {
                        snapshot.ItemSizeIndex = _itemSizeIndex;
                    }

                    if (_owner != null)
                        _owner.ItemSizeIndex = _itemSizeIndex;
                }
            }
        }
        #endregion

        /// <summary>
        /// 结果列表是否显示
        /// </summary>
        public Visibility GridVisible
        {
            get
            {
                if (Snapshots == null || Snapshots.Count == 0)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        /// <summary>
        /// 无结果提示是否显示
        /// </summary>
        public Visibility NoticeVisible
        {
            get
            {
                if (Snapshots == null || Snapshots.Count > 0)
                {
                    return Visibility.Collapsed;
                }
                return Visibility.Visible;
            }
        }

        public async Task InitTaskResult()
        {
            await _owner.InitFromServer();

            Snapshots = _owner.Snapshots;
            Title = Snapshots.Count > 0 ? String.Format("{0}张图片", Snapshots.Count) : "";
        }
    }
}
