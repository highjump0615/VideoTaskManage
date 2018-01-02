

using System;
using System.Collections.Generic;

namespace VideoSearch.ViewModel.PlayView
{
    public class PlayViewMainModel : MultiPlayerModel
    {
        public PlayViewMainModel(String name, String moviePath)
        {
        }

        public PlayViewMainModel(List<String> nameList, List<String> movieList)
            : base(nameList, movieList)
        {
        }

    }
}
