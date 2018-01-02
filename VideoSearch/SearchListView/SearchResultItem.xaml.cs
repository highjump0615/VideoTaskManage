using System.Windows.Controls;
using System.Windows.Media;


namespace VideoSearch.SearchListView
{
    /// <summary>
    /// Interaction logic for SearchResultItem.xaml
    /// </summary>
    public partial class SearchResultItem : ListViewItem
    {
        #region Property
        private FormattedText _itemText = null;
        public FormattedText ItemText
        {
            get { return _itemText; }
        }


        #endregion

        public SearchResultItem(FormattedText fmtText, double height = 24)
        {
            InitializeComponent();

            Height = height;

            Content = new SearchListViewItemContent(fmtText);
        }
    }
}
