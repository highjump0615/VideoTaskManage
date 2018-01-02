using System;
using System.Collections.Generic;
using System.ComponentModel;
using VideoSearch.ViewModel.PlayView;

namespace VideoSearch.ViewModel
{
    public class MovieViewPlayModel : INotifyPropertyChanged
    {
        #region Delegate & Handler
        public event PropertyChangedEventHandler PropertyChanged;
        protected void OnPropertyChanged(PropertyChangedEventArgs propertyChangedEventArgs)
        {
            if (PropertyChanged != null)
                PropertyChanged(this, propertyChangedEventArgs);
        }
        #endregion

        #region Property
        private Object _contents;
        public Object Contents
        {
            get { return _contents; }
            set
            {
                if (_contents == null || _contents.GetType() != value.GetType())
                {
                    _contents = value;
                    OnPropertyChanged(new PropertyChangedEventArgs("Contents"));
                }
            }
        }
        #endregion

        #region Constructor
        public MovieViewPlayModel(String name, String moviePath)
        {
            List<String> namelist = new List<String>();
            List<String> movielist = new List<String>();

            namelist.Add(name);
            movielist.Add(moviePath);

            Contents = new PlayViewMainModel(namelist, movielist);
        }

        public MovieViewPlayModel(List<String> nameList, List<String> movieList)
        {
            if (nameList == null || nameList.Count == 0)
                return;

            if (movieList == null || movieList.Count == 0)
                return;

            if (nameList.Count != movieList.Count)
                return;

            if (movieList.Count == 1)
                Contents = new PlayViewMainModel(nameList, movieList);
            else if (movieList.Count < 5)
                Contents = new PlayViewSmallModel(nameList, movieList);
            else if (movieList.Count < 10)
                Contents = new PlayViewMediumModel(nameList, movieList);
            else
                Contents = new PlayViewLargeModel(nameList, movieList);
        }

        #endregion

    }
}
