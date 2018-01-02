using System;
using System.ComponentModel;
using VideoSearch.Model;

namespace VideoSearch.ViewModel
{
    public class PanelViewTaskCompressModel : INotifyPropertyChanged
    {
        public PanelViewTaskCompressModel(DataItemBase item)
        {
            if (item != null && item.GetType() == typeof(MovieTaskCompressItem))
            {
                MovieTaskCompressItem compressedMovie = (MovieTaskCompressItem)item;

                MovieTitle = compressedMovie.Name;
                MoviePath = compressedMovie.CompressedPlayPath;
            }
        }

        #region Property

        private String _movieTitle = "";
        public String MovieTitle
        {
            get { return _movieTitle; }
            set
            {
                if (_movieTitle != value)
                {
                    _movieTitle = value;
                    OnPropertyChanged("MovieTitle");
                }
            }
        }

        private String _moviePath = "";

        public String MoviePath
        {
            get { return _moviePath; }
            set
            {
                if (_moviePath != value)
                {
                    _moviePath = value;
                    OnPropertyChanged("MoviePath");
                }
            }
        }

        #endregion

        #region Notify
        public event PropertyChangedEventHandler PropertyChanged;

        protected virtual void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        #endregion
    }
}
