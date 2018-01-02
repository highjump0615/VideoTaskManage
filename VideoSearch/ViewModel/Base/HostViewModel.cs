using System;
using System.ComponentModel;

using VideoSearch.Model;

namespace VideoSearch.ViewModel.Base
{
    public class HostViewModel : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
        {
            PropertyChanged?.Invoke(this, propertyChangedEventArgs);
        }

        private DataItemBase _owner;
        public DataItemBase Owner
        {
            get { return _owner; }
        }

        public HostViewModel(DataItemBase owner)
        {
            _owner = owner;
        }

        private Object _contents;
        public Object Contents
        {
            get { return _contents; }
            set
            {
                if(_contents == null || _contents.GetType() != value.GetType())
                {
                    _contents = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Contents"));
                }
            }
        }
    }
}
