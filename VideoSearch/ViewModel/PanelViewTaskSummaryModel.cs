using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class PanelViewTaskSummaryModel : ObservableObject
    {
        private MovieTaskSummaryItem _owner = null;

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

        public PanelViewTaskSummaryModel(DataItemBase item)
        {
            if (item != null && item.GetType() == typeof(MovieTaskSummaryItem))
            {
                MovieTaskSummaryItem summaryItem = (MovieTaskSummaryItem)item;

                _owner = summaryItem;

                DisplayType = _owner.DisplayType;
                ItemSizeIndex = _owner.ItemSizeIndex;

                var task = InitTaskResult();
             }
        }

        public async Task InitTaskResult()
        {
            await _owner.InitFromServer();

            Snapshots = _owner.Snapshots;
            Title = _snapshots.Count > 0 ? String.Format("{0}张图片", _snapshots.Count) : "";
        }

        #region Property

        private List<TaskSnapshot> _snapshots = null;
        public List<TaskSnapshot> Snapshots
        {
            get { return _snapshots; }
            set
            {
                _snapshots = value;
                PropertyChanging("Snapshots");
            }
        }

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
                if(_displayType != value && _snapshots != null)
                {
                    _displayType = value;

                    foreach(TaskSnapshot snapshot in _snapshots)
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
    }

}
