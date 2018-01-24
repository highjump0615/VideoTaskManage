using System;
using System.ComponentModel;
using VideoSearch.Database;
using VideoSearch.Windows;

namespace VideoSearch.Model
{
    public class CameraItem : DataItemBase
    {
        #region Property

        private String _eventPos = "";
        public String EventPos
        {
            get { return _eventPos; }
            set
            {
                if (_eventPos != value)
                {
                    _eventPos = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("EventPos"));
                }
            }
        }

        private String _address = "";
        public String Address
        {
            get { return _address; }
            set
            {
                if (_address != value)
                {
                    _address = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Address"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressDesc"));
                }
            }
        }

        /// <summary>
        /// 地址 + 经纬度
        /// </summary>
        public String AddressDesc
        {
            get
            {
                string strAddr = this.Address;
                if (!String.IsNullOrEmpty(strAddr))
                {
                    strAddr += "\n";
                }
                strAddr += this.Coordinate;

                return strAddr;
            }
        }

        public String Coordinate
        {
            get
            {
                return String.Format("({0:0.00000}, {1:0.00000})", this.Longitude, this.Latitude);
            }
        }

        private double _longitude;
        public double Longitude
        {
            get { return _longitude; }
            set
            {
                if (_longitude != value)
                {
                    _longitude = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Longitude"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Coordinate"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressDesc"));
                }
            }
        }

        private double _latitude;
        public double Latitude
        {
            get { return _latitude; }
            set
            {
                if (_latitude != value)
                {
                    _latitude = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Latitude"));
                    OnPropertyChanged(new PropertyChangedEventArgs("Coordinate"));
                    OnPropertyChanged(new PropertyChangedEventArgs("AddressDesc"));
                }
            }
        }

        private String _type = "";
        public String Type
        {
            get { return _type; }
            set
            {
                if (_type != value)
                {
                    _type = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Type"));
                }
            }
        }

        private String _source = "";
        public String Source
        {
            get { return _source; }
            set
            {
                if (_source != value)
                {
                    _source = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Source"));
                }
            }
        }

        private String _portCount = "";
        public String PortCount
        {
            get { return _portCount; }
            set
            {
                if (_portCount != value)
                {
                    _portCount = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("PortCount"));
                }
            }
        }
        #endregion

        #region Command
        public String ShowMapCommand
        {
            get { return "ShowMapCommand"; }
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
            // 
            // 弹出摄像头添加对话框
            //
            CreateCameraWindow createDlg = new CreateCameraWindow(this);

            Nullable<bool> result = createDlg.ShowDialog();
            if (result == true)
            {
                CameraItem newItem = createDlg.NewCamera;

                if( newItem.EventPos == EventPos)
                {
                    if (Table != null && Table.Update(newItem) != 0)
                        SetItem(newItem);
                }
                else if(Parent != null)
                {
                    DataItemBase ReParent = Parent.FindFriendItem(newItem.EventPos);

                    if (ReParent != null && Parent.DeleteItem(this))
                    {
                        if (Parent.IsSelected)
                        {
                            Parent.IsSelected = false;
                            ReParent.IsSelected = true;
                        }
                        if (Parent.IsExpanded)
                        {
                            Parent.IsExpanded = false;
                            ReParent.IsExpanded = true;
                        }

                        newItem.DisplayID = ReParent.GetItemDisplayID(newItem.ID);
                        newItem.Name = ReParent.AutoName;
                        newItem.IsChecked = false;
                        newItem.IsSelected = false;

                        ReParent.AddItem(new CameraItem(newItem));
                    }
                }
             }
        }

        public override bool ContainsText(String text)
        {
            if (Name.Contains(text) || DisplayID.Contains(text) || Type.Contains(text))
                return true;
            return false;
        }

        public override String GetDisplaySearchText()
        {
            String strSearch = "";

            strSearch += "●  摄像头名称 : " + Name;
            strSearch += " ; 摄像头编号 : " + DisplayID;
            strSearch += " ; 摄像头类型 : " + Type;

            return strSearch;
        }
        #endregion

        #region Constructor & Init
        public CameraItem(DataItemBase parent = null) : base()
        {
            Parent = parent;

            SetLevel(2);
            ItemNamePrefix = "Movie";
            IconPath = "Resources/Images/View/TreeView/tree_icon_Camera.png";

            Table = CameraTable.Table;
            ItemsTable = MovieTable.Table;
        }

        public CameraItem(DataItemBase parent, String eventPos, String id, String display_id, String name, String address, double longitude, double latitude, String type, String source, String portCount, bool isSelected = false)
           : this(parent)
        {
            EventPos = eventPos;
            ID = id;
            DisplayID = display_id;
            Name = name;
            Address = address;
            Longitude = longitude;
            Latitude = latitude;
            Type = type;
            Source = source;
            PortCount = portCount;
            IsSelected = isSelected;
        }

        public CameraItem(CameraItem item)
            : this(item.Parent, item.EventPos, item.ID, item.DisplayID, item.Name, item.Address, item.Longitude, item.Latitude, item.Type, item.Source, item.PortCount)
        {
        }

        public override void SetItem(DataItemBase copyItem)
        {
            base.SetItem(copyItem);

            CameraItem item = (CameraItem)copyItem;
            EventPos = item.EventPos;
            Address = item.Address;
            Longitude = item.Longitude;
            Latitude = item.Latitude;
            Type = item.Type;
            Source = item.Source;
            PortCount = item.PortCount;
            IsSelected = item.IsSelected;
        }

        #endregion
    }
}
