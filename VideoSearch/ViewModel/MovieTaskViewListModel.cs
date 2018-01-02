using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    class MovieTaskViewListModel : ListViewModel
    {
        public MovieTaskViewListModel(DataItemBase owner) : base(owner)
        {
        }

        public override DataItemBase EmptyItem()
        {
            MovieTaskItem item = new MovieTaskItem();
            item.Visibility = Visibility.Hidden;

            return item;
        }
    }
}