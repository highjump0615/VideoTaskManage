using System;
using System.Threading.Tasks;
using System.Windows;
using VideoSearch.Model;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.ViewModel
{
    public class MovieViewListModel : ListViewModel
    {
        public MovieViewListModel(DataItemBase owner, Object parentViewModel = null) : base(owner, parentViewModel)
        {
            // query each video
            var taskQuery = QueryVideoIfNeeded();
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

        private async Task QueryVideoIfNeeded()
        {
            foreach (MovieItem item in Owner)
            {
                await item.InitFromServer();
            }
        }

        protected override void OnDispose()
        {
            base.OnDispose();

            foreach (MovieItem item in Owner)
            {
                item.DisposeItem();
            }
        }
    }
}
