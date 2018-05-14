using System;
using System.Windows;
using System.ComponentModel;
using VideoSearch.ViewModel.Base;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using VideoSearch.Database;
using System.Threading.Tasks;

namespace VideoSearch.Model
{
    public delegate void NotifyCommandHandler(DataItemBase sender, String command);

    public class DataItemBase : ObservableCollection<DataItemBase>, INotifyPropertyChanged, IDisposable
    {
        #region Constructor & Init & Destructor
        public DataItemBase()
        {
            DataItemCommand = new RelayCommandEx(OnCommand);
        }

        public virtual void SetItem(DataItemBase copyItem)
        {
            ID = copyItem.ID;
            Name = copyItem.Name;
            Parent = copyItem.Parent;
        }

        public DataItemBase FindFriendItem(String find_id)
        {
            if (Parent == null)
                return null;

            foreach (DataItemBase item in Parent.Children)
                if (item.ID == find_id)
                    return item;

            return null;
        }

        public void Dispose()
        {
            DisposeItem();
        }

        public virtual void DisposeItem()
        {
            foreach(DataItemBase item in Items)
                item.Dispose();
        }
        #endregion

        #region Property

        #region Backend property
        private bool _isEnabled = true;
        public bool IsEnabled
        {
            get { return _isEnabled; }
            set
            {
                if (_isEnabled != value)
                {
                    _isEnabled = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsEnabled"));
                }
            }
        }

        private UInt16 _level = 0;
        public UInt16 Level
        {
            get { return _level; }
            private set
            {
                _level = value;

                OnPropertyChanged(new PropertyChangedEventArgs("Level"));
            }
        }

        private String _itemNamePrefix = "";
        public String ItemNamePrefix
        {
            get { return _itemNamePrefix; }
            set { _itemNamePrefix = value; }
        }

        private DataItemBase _parent = null;
        public DataItemBase Parent
        {
            get { return _parent; }

            set
            {
                _parent = value;

                OnPropertyChanged(new PropertyChangedEventArgs("Parent"));
            }
        }

        private bool _isChecked = false;
        public bool IsChecked
        {
            get { return _isChecked; }
            set
            {
                Console.WriteLine("Check ===");
                if (_isChecked != value)
                {
                    _isChecked = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsChecked"));
                    SelectorVisibility = _isChecked ? Visibility.Visible : Visibility.Hidden;
                }
            }
        }

        private bool _isSelected = false;
        public bool IsSelected
        {
            get { return _isSelected; }
            set
            {
                if (_isSelected != value)
                {
                    _isSelected = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsSelected"));
                }
            }
        }

        private bool _isExpanded = false;
        public bool IsExpanded
        {
            get { return _isExpanded; }
            set
            {
                if (_isExpanded != value)
                {
                    _isExpanded = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IsExpanded"));
                }
            }
        }

        private Visibility _selectorVisibility = Visibility.Hidden;
        public Visibility SelectorVisibility
        {
            get { return _selectorVisibility; }
            set
            {
                if (_selectorVisibility != value)
                {
                    _selectorVisibility = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("SelectorVisibility"));
                }
            }
        }

        private Visibility _visibility = Visibility.Visible;
        public Visibility Visibility
        {
            get { return _visibility; }
            set
            {
                if (_visibility != value)
                {
                    _visibility = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Visibility"));
                }
            }
        }

        public bool HasCheckedItem
        {
            get
            {
                foreach (DataItemBase item in Items)
                    if (item.IsChecked)
                        return true;

                return false;
            }
        }

        private String _iconPath = "";
        public String IconPath
        {
            get { return _iconPath; }
            set
            {
                if (_iconPath != value)
                {
                    _iconPath = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("IconPath"));
                }
            }
        }

        private Thickness _margin = new Thickness(-32, 4, 0, 0);
        public Thickness Margin
        {
            get { return _margin; }
            set
            {
                if (_margin != value)
                {
                    _margin = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Margin"));
                }
            }
        }

        public IList<DataItemBase> Children
        {
            get { return Items; }
        }

        #region Auto Value
        public String AutoID
        {
            get
            {
                DateTime today = DateTime.Now;
                String autoID = String.Format("{0}", today.ToString("yyyyMMddHHmmssfffffff"));

                return autoID;
            }
        }

        public String AutoName
        {
            get
            {
                String autoName;
                Decimal no = Items.Count + 1;

                autoName = ItemNamePrefix + String.Format("{0}", no);

                return autoName;
            }
        }
        #endregion

        protected TableManager _table = null;
        public TableManager Table
        {
            get { return _table; }
            set { _table = value; }
        }

        protected TableManager _itemsTable = null;
        public TableManager ItemsTable
        {
            get { return _itemsTable; }
            set
            {
                _itemsTable = value;
                LoadItems();
            }
        }

        #endregion

        #region for DB & Table's common member

        private String _order = "";
        public String Order
        {
            get
            {
                if (Parent == null)
                    return "";

                int index = Parent.Children.IndexOf(this);

                if (index < 0)
                    return "";

                return String.Format("{0}", (index + 1));
            }

            set
            {
                if(_order != value)
                {
                    _order = value;

                    OnPropertyChanged(new PropertyChangedEventArgs("Order"));
                    OnPropertyChanged(new PropertyChangedEventArgs("CheckerName"));
                }
            }
        }

        public virtual String CheckerName
        {
            get
            {
                return "";
            }
        }

        public virtual double CheckerWidth
        {
            get { return 16.0; }
        }

        private String _id = "";
        public String ID
        {
            get { return _id; }
            set
            {
                if (_id != value)
                {
                    _id = value;
                    LoadItems();
                    OnPropertyChanged(new PropertyChangedEventArgs("ID"));
                }
            }
        }

        private String _displayID = "";
        public String DisplayID
        {
            get
            {
                if (Parent != null && _displayID.Length == 0)
                {
                    String displayID = ID.Substring(0, 8);
                    uint nIndex = 1;
                    foreach (DataItemBase item in Parent.Children)
                    {
                        if (item.ID == ID)
                            break;

                        if (item.ID.Contains(displayID))
                            nIndex++;
                    }

                    displayID += String.Format("{0,3:d3}", Math.Min(999, Decimal.ToInt32(nIndex)));

                    _displayID = displayID;
                }

                return _displayID;
            }

            set
            {
                if (_displayID != value)
                {
                    _displayID = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("DisplayID"));
                }
            }
        }

        private String _name = "";
        public String Name
        {
            get { return _name; }
            set
            {
                if (_name != value)
                {
                    _name = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Name"));
                }
            }
        }
        #endregion

        #endregion

        #region Command Reference

        public String UpdateCommand
        {
            get { return "UpdateCommand"; }
        }

        public NotifyCommandHandler Command = null;
        protected virtual void OnCommand(Object parameter)
        {
            if (parameter != null && parameter.GetType() == typeof(String))
            {
                if(Parent != null && Parent.Command != null)
                    Parent.Command(this, (String)parameter);
            }
        }

        public RelayCommandEx DataItemCommand
        {
            get;
            private set;
        }

        #endregion

        #region Notify Reference
        protected override void OnCollectionChanged(NotifyCollectionChangedEventArgs e)
        {
            base.OnCollectionChanged(e);

            SendCollectionChangedNotification(this);
        }

        public void SendCollectionChangedNotification(DataItemBase item)
        {
            if(item != this)
            {
                int index = IndexOf(item);

                if (index >= 0)
                {
                    Remove(item);
                    Insert(index, item);
                }
            }
                    
            if (Parent != null)
                Parent.SendCollectionChangedNotification(this);
        }
        #endregion

        #region Common function

        public String GetItemDisplayID(String itemID)
        {
            String displayID = itemID.Substring(0, 8);
            uint nIndex = 1;
            foreach (DataItemBase item in Items)
            {
                if (item.ID == itemID)
                    break;

                if (item.ID.Contains(displayID))
                    nIndex++;
            }

            displayID += String.Format("{0,3:d3}", Math.Min(999, Decimal.ToInt32(nIndex)));

            return displayID;
        }

        protected void SetLevel(UInt16 level)
        {
            Level = level;
        }

        public DataItemBase GetItem(String id)
        {
            foreach(DataItemBase item in Items)
            {
                if (item.ID == id)
                    return item;
            }

            return null;
        }

        public bool AddItem(DataItemBase newItem, bool needsUpdateDB = true)
        {
            if (newItem == null)
                return false;

            if (GetItem(newItem.ID) != null)
                return false;

            if (needsUpdateDB && (ItemsTable == null || ItemsTable.Add(newItem) == 0))
                return false;

            newItem.Parent = this;
            Add(newItem);

            return true;
        }

        public virtual bool DeleteItem(DataItemBase item)
        {
            if (ItemsTable != null && ItemsTable.Remove(item) == 1)
            {
                Remove(item);
                item.ClearFromDB();

                return true;
            }

            return false;
        }

        public virtual bool DeleteSelectedItem()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                DataItemBase item = Items[i];

                if (item.IsChecked)
                {
                    if (ItemsTable != null && ItemsTable.Remove(item) == 1)
                    {
                        RemoveAt(i);
                        item.ClearFromDB();
                    }
                }
            }

            return true;
        }

        public virtual async Task<bool> DeleteSelectedItemAsync()
        {
            for (int i = Items.Count - 1; i >= 0; i--)
            {
                DataItemBase item = Items[i];

                if (item.IsChecked)
                {
                    if (ItemsTable != null && ItemsTable.Remove(item) == 1)
                    {
                        RemoveAt(i);
                        await item.ClearFromDBAsync();
                    }
                }
            }

            return true;
        }
        #endregion

        #region Utility for DataTable (LoadItems, ClearFromDB)

        protected virtual void LoadItems()
        {
            if (ItemsTable == null)
                return;

            ClearFromDB();
            ItemsTable.Load(this);
        }

        public virtual bool ClearFromDB()
        {
            foreach (DataItemBase item in Items)
            {
                if (item.ClearFromDB() && Table != null)
                    Table.Remove(item);
                else
                    return false;
            }
            return true;
        }

        public virtual async Task<bool> ClearFromDBAsync()
        {
            foreach (DataItemBase item in Items)
            {
                if (await item.ClearFromDBAsync() && Table != null)
                    Table.Remove(item);
                else
                    return false;
            }
            return true;
        }

        #endregion

        #region Utility for Search (GetSearchText)

        public virtual bool ContainsText(String text)
        {
            return false;
        }

        public virtual String GetDisplaySearchText()
        {
            return "";
        }
        #endregion
    }
}
