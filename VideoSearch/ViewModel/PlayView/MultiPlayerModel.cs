using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using VideoSearch.ViewModel.Player;

namespace VideoSearch.ViewModel.PlayView
{
    public class MultiPlayerModel : ObservableCollection<MiniPlayerViewModel>, INotifyPropertyChanged
    {
        public MultiPlayerModel(List<String> nameList = null, List < String> movieList = null, int maxCount = 16)
        {
            int count = (movieList == null) ? 0 : movieList.Count;

            String name, moviePath;
            for (int index = 0; index < count; index++)
            {
                if (index < count)
                {
                    name = nameList[index];
                    moviePath = movieList[index];
                }
                else
                {
                    name = "";
                    moviePath = "";
                }

                Add(new MiniPlayerViewModel(name, moviePath));
            }
        }

        public IList<MiniPlayerViewModel> PlayerList
        {
            get { return Items; }
        }
    }
}
