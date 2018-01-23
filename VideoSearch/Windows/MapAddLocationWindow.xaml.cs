using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for MapAddLocationWindow.xaml
    /// </summary>
    public partial class MapAddLocationWindow : Window
    {
        public MapAddLocationWindow()
        {
            InitializeComponent();

            WebBrowserOverlay wbo = new WebBrowserOverlay(bdMap);

            wbo.webBrowser.Navigate(Path.Combine(Environment.CurrentDirectory, "Map\\addLocation.html"));
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Application curApp = Application.Current;
            Window mainWnd = curApp.MainWindow;
            this.Left = mainWnd.Left + (mainWnd.Width - this.ActualWidth) / 2;
            this.Top = mainWnd.Top + (mainWnd.Height - this.ActualHeight) / 2;
        }

        /// <summary>
        /// 点击确定按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButOk(object sender, RoutedEventArgs e)
        {

        }
    }
}
