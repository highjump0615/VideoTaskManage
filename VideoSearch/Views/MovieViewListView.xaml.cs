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

            Unloaded += (s, e) => ((ViewModelBase)this.DataContext).Dispose();
        }
    }
}
