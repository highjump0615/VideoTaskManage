using System;
using System.Collections.Generic;
using VideoSearch.Model;

namespace VideoSearch.ViewModel
{
    public class PanelViewTaskSummaryModel
    {
        private MovieTaskSummaryItem _owner = null;

        public PanelViewTaskSummaryModel(DataItemBase item)
        {
            if (item != null && item.GetType() == typeof(MovieTaskSummaryItem))
            {
                MovieTaskSummaryItem summaryItem = (MovieTaskSummaryItem)item;

                _snapshots = summaryItem.Snapshots;
                _title = _snapshots.Count > 0 ? String.Format("{0}张图片", _snapshots.Count) : "";
                _owner = summaryItem;

                DisplayType = _owner.DisplayType;
                ItemSizeIndex = _owner.ItemSizeIndex;
             }
        }
        #region Property

        private List<TaskSnapshot> _snapshots = null;
        public List<TaskSnapshot> Snapshots
        {
            get { return _snapshots; }
        }

        private String _title = "";

        public String Title
        {
            get { return _title; }            
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
