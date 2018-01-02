using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VideoSearch.SearchListView
{
    /// <summary>
    /// Interaction logic for SearchListViewItemContent.xaml
    /// </summary>
    public partial class SearchListViewItemContent : UserControl
    {
        private FormattedText _contentText = null;

        public FormattedText Text
        {
            set
            {
                _contentText = value;
            }
        }

        public SearchListViewItemContent()
        {

        }

        public SearchListViewItemContent(FormattedText fmtText)
        {
            InitializeComponent();

            Text = fmtText;
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            if(_contentText != null)
            {
                double text_height = _contentText.Height;

                drawingContext.DrawText(_contentText, new Point(0, -8));
            }

        }
    }
}
