﻿using System;
using System.ComponentModel;
using System.Windows;
using VideoSearch.Database;
using VideoSearch.Windows;

namespace VideoSearch.Model
{
    public class EventItem : DataItemBase
    {
        #region Property

        private String _date = "";
        public String Date
        {
            get { return _date; }
            set
            {
                if (_date != value)
                {
                    _date = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Date"));
                }
            }
        }

        private String _remark = "";
        public String Remark
        {
            get { return _remark; }
            set
            {
                if (_remark != value)
                {
                    _remark = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Remark"));
                }
            }
        }
        #endregion

        #region Constructor & Init
        public EventItem() : base()
        {
            SetLevel(1);
            ItemNamePrefix = "Camera";
            IconPath = "Resources/Images/View/TreeView/tree_icon_Event.png";
            Margin = new Thickness(0, 4, 0, 0);

            Table = EventTable.Table;
            ItemsTable = CameraTable.Table;
        }

        public EventItem(String id, String display_id, String name, String date, String remark, bool isSelected)
            : this()
        {
            ID = id;
            DisplayID = display_id;
            Name = name;
            Date = date;
            Remark = remark;
            IsSelected = isSelected;
        }

        public EventItem(String id, String display_id, String name, String date, String remark)
            : this(id, display_id, name, date, remark, false)
        {
        }

        public EventItem(EventItem item)
            : this(item.ID, item.DisplayID, item.Name, item.Date, item.Remark, item.IsSelected)
        {            
        }

        public override void SetItem(DataItemBase copyItem)
        {
            base.SetItem(copyItem);

            EventItem item = (EventItem)copyItem;

            ID = item.ID;
            DisplayID = item.DisplayID;
            Name = item.Name;
            Date = item.Date;
            Remark = item.Remark;
        }
        #endregion

        #region Override & Update
        protected override void OnCommand(Object parameter)
        {
            if (parameter != null && parameter.GetType() == typeof(String) && (String)parameter == UpdateCommand)
            {
                UpdateItem();
            }
            else
                base.OnCommand(parameter);
        }

        protected void UpdateItem()
        {
            CreateEventWindow createDlg = new CreateEventWindow(this);

            Nullable<bool> result = createDlg.ShowDialog();
            if (result == true)
            {
                EventItem newItem = createDlg.NewEvent;

                if(Table != null && Table.Update(newItem) != 0)
                {
                    SetItem(newItem);
                }
            }
        }

        public override bool ContainsText(String text)
        {
            if (Name.Contains(text) || DisplayID.Contains(text) || Date.Contains(text) || Remark.Contains(text))
                return true;
            return false;
        }

        public override String GetDisplaySearchText()
        {
            String strSearch = "";

            strSearch += "●  案件名称 : " + Name;
            strSearch += " ; 案件编号 : " + DisplayID;
            strSearch += " ; 创建日期 : " + Date;
            strSearch += " ; 备注 : " + Remark;

            return strSearch;
        }
        #endregion
    }
}