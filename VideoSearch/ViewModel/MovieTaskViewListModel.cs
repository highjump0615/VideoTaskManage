using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    class MovieTaskViewListModel : ListViewModel
    {
        public MovieTaskViewListModel(DataItemBase owner) : base(owner)
        {
            var movieItem = (MovieItem)owner;

            foreach (MovieTaskItem taskItem in movieItem.Children)
            {
                var task = taskItem.InitFromServer();
            }
        }
    }
}