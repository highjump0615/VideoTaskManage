using System.Windows;

namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for ConfirmDeleteWindow.xaml
    /// </summary>
    public partial class ConfirmDeleteWindow : Window
    {
        public ConfirmDeleteWindow()
        {
            InitializeComponent();

            Owner = MainWindow.VideoSearchMainWindow;
        }

        private void btnOk_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        public void setMessage(string text, bool showIcon = false)
        {
            Msg.Text = text;
            if (!showIcon)
            {
                // 隐藏图标
                Icon.Visibility = Visibility.Collapsed;
            }
        }
    }
}
