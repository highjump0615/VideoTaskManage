using System;
using System.Collections.Generic;

namespace VideoSearch.ViewModel.PlayView
{
    public class PlayViewMediumModel : MultiPlayerModel
    {
        public PlayViewMediumModel(List<String> nameList, List<String> movieList)
            : base(nameList, movieList, 9)
        {
        }
    }
}
