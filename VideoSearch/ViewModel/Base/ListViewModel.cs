using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.Windows;

namespace VideoSearch.ViewModel.Base
{
    public class ListViewModel : ViewModelBase
    {

        protected virtual void CommandHandler(DataItemBase sender, String command)
        {
        }

        protected virtual void CollectionChangedEventHandler(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Remove)
            {
                foreach (DataItemBase item in ChildItems)
                    item.Order = item.Order;
            }
        }

        private Object _parentViewModel;
        private DataItemBase _owner;

        public ListViewModel(DataItemBase owner, Object parentViewModel = null)
        {
            _owner = owner;
            _owner.CollectionChanged += CollectionChangedEventHandler;
            _owner.Command += CommandHandler;

            _parentViewModel = parentViewModel;
        }


        public Object ParentViewModel
        {
            get { return _parentViewModel; }
        }


        public DataItemBase Owner
        {
            get { return _owner; }
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
                    PropertyChanging("Visibility");
                }
            }
        }

        public IList<DataItemBase> ChildItems
        {
            get
            {
                return Owner.Children;
            }
        }

        #region utility fuction
        public virtual async void AddNewItemAsync()
        {
            await Task.FromResult(0);
        }

        public virtual void DeleteSelectedItems()
        {
            if (_owner == null || !_owner.HasCheckedItem)
                return;

            ConfirmDeleteWindow deleteDlg = new ConfirmDeleteWindow();

            Nullable<bool> result = deleteDlg.ShowDialog();

            if (result == true)
            {
                Globals.Instance.ShowWaitCursor(true);
                _owner.DeleteSelectedItem();
                Globals.Instance.ShowWaitCursor(false);

                // update tree
                Globals.Instance.MainVM.updateTreeList();
            }
        }
        #endregion
    }
}
