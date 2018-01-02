using System;
using System.Collections.Generic;

namespace VideoSearch.ViewModel.PlayView
{
    public class PlayViewLargeModel : MultiPlayerModel
    {
        public PlayViewLargeModel(List<String> nameList, List<String> movieList)
            : base(nameList, movieList, 16)
        {
        }
    }
}
