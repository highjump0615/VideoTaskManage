using System;
using System.Collections.Generic;
using VideoSearch.ViewModel.Player;

namespace VideoSearch.ViewModel.PlayView
{
    public class PlayViewSmallModel : MultiPlayerModel
    {
         public PlayViewSmallModel(List<String> nameList, List<String> movieList)
            : base(nameList, movieList, 4)
        {
        }
    }
}
