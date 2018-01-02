﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.Windows;

namespace VideoSearch.ViewModel.Base
{
    public class ListViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

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
                    OnPropertyChanged("Visibility");
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

        public virtual DataItemBase EmptyItem()
        {
            DataItemBase item = new DataItemBase();
            item.Visibility = Visibility.Hidden;

            return item;
        }

        #region utility fuction
        public virtual void AddNewItem()
        {
        }

        public virtual void DeleteSelectedItems()
        {
            if (_owner == null || !_owner.HasCheckedItem)
                return;

            ConfirmDeleteWindow deleteDlg = new ConfirmDeleteWindow();

            Nullable<bool> result = deleteDlg.ShowDialog();

            if (result == true)
            {
                _owner.DeleteSelectedItem();
            }
        }
        #endregion
    }
}