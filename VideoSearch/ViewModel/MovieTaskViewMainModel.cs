using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VideoSearch.Model;

namespace VideoSearch.ViewModel
{
    public class ClipInfo
    {
        public ClipInfo(long frame, int x0, int y0, int x1, int y1)
        {
            Frame = frame;
            X0 = x0;
            Y0 = y0;
            X1 = x1;
            Y1 = y1;
        }

        public long Frame
        {
            get;
            set;
        }

        public int X0
        {
            get;
            set;
        }

        public int Y0
        {
            get;
            set;
        }

        public int X1
        {
            get;
            set;
        }

        public int Y1
        {
            get;
            set;
        }
    }

    public class MovieTaskViewMainModel : INotifyPropertyChanged
    {
        #region Constructor
        public MovieTaskViewMainModel(DataItemBase item, Object parentViewModel = null)
        {
            if (item != null && item.GetType() == typeof(MovieItem))
            {
                MovieItem movie = (MovieItem)item;

                MovieTitle = movie.Name;
                MoviePath = movie.PlayPath;
                MovieID = movie.VideoId.ToString();
            }

            _parentViewModel = parentViewModel;
        }
        #endregion

        #region Property
        private Object _parentViewModel = null;

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
                if(_moviePath != value)
                {
                    _moviePath = value;
                    OnPropertyChanged("MoviePath");
                }
            }
        }

        private String _movieID= "";
        public String MovieID
        {
            get { return _movieID; }
            set
            {
                if (_movieID != value)
                {
                    _movieID = value;
                    OnPropertyChanged("MovieID");
                }
            }
        }

        private ObservableCollection<ClipInfo> _markList = new ObservableCollection<ClipInfo>();
        public ObservableCollection<ClipInfo> MarkList
        {
            get
            {
                return _markList;
            }
            set
            {
                _markList = value;
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
