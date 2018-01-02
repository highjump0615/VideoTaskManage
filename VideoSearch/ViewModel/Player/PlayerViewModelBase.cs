using System;
using System.ComponentModel;
using System.IO;
using System.Windows;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel.Player
{
    public class PlayerViewModelBase : INotifyPropertyChanged
    {
        public event PropertyChangedEventHandler PropertyChanged;

        protected void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyChangedEventArgs);
        }

        private String _title;
        public String Title
        {
            get { return _title; }
            set
            {
                if (_title != value)
                {
                    _title = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Title"));
                }
            }
        }

        private String _source = null;
        public String Source
        {
            get { return _source; }
            set
            {
                if(_source != value)
                {
                    _source = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Source"));
                }
            }
        }

        public PlayerViewModelBase(String name, String source)
        {
            Title = name;
            Source = source;
        }
    }
}
