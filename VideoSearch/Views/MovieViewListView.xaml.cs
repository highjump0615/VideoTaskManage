using System.Windows;
using System.Windows.Controls;
using VideoSearch.ViewModel.Base;

namespace VideoSearch.Views
{
    /// <summary>
    /// Interaction logic for MovieViewListView.xaml
    /// </summary>
    public partial class MovieViewListView : UserControl
    {
        public MovieViewListView()
        {
            InitializeComponent();

            if (this.DataContext != null)
            {
                Unloaded += (s, e1) => ((ViewModelBase)this.DataContext).Dispose();
            }
        }
    }
}
