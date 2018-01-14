using System;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class MovieViewListModel : ListViewModel
    {
        public MovieViewListModel(DataItemBase owner, Object parentViewModel = null) : base(owner, parentViewModel)
        {
        }

        public void MovieImport(object parameter)
        {
            if (parameter != null)
            {
                if (parameter.GetType() == typeof(MovieItem))
                {
                    MovieItem item = (MovieItem)parameter;

                    item.Import();
                }
            }
        }
    }
}
