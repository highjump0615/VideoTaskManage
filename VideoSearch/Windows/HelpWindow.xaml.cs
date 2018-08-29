using System.Windows;


namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();

            Owner = MainWindow.VideoSearchMainWindow;
            txtDescription.Text = "软件开发说明, 使用说明, 反馈说明。软件开发说明, 使用说明, 反馈说明。软件开发说明, \n\n使用说明, 反馈说明。软件开发说明, 使用说明, \n\n反馈说明。软件开发说明, 使用说明, 反馈说明。软件开发说明, 使用说明, 反馈说明。\n\n软件开发说明, 使用说明, 反馈说明。软件开发说明, 使用说明, \n\n反馈说明。软件开发说明, 使用说明, 反馈说明。软件开发说明, 使用说明, \n\n反馈说明。软件开发说明, 使用说明, 反馈说明。\n\n软件开发说明, 使用说明, 反馈说明。软件开发说明, 使用说明, 反馈说明。软件开发说明。";

            // 到期时间
            var mainVM = Globals.Instance.MainVM;
            txtExpire.Text = "（至" + mainVM.mstrExpireDate + "）";
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
