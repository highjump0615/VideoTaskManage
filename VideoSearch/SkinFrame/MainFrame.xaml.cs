using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.ComponentModel;

namespace VideoSearch.SkinFrame
{
    /// <summary>
    /// Interaction logic for MainFrame.xaml
    /// </summary>
    public partial class MainFrame : UserControl
    {
        public MainFrame()
        {
            InitializeComponent();
        }

        protected override void OnInitialized(EventArgs e)
        {
            base.OnInitialized(e);
            mainFrame.FrameStateChanged = StateChanged;
        }

        void StateChanged(bool isMaximized)
        {
            btRestore.IsAltState = isMaximized;
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public bool CanResize
        {
            get
            {
                return mainFrame.CanResize;
            }
            set
            {
                mainFrame.CanResize = value;
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public bool HasTitlebar
        {
            get
            {
                return mainFrame.HasTitlebar;
            }
            set
            {
                mainFrame.HasTitlebar = value;
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public Color TitlebarColor
        {
            get
            {
                return mainFrame.TitlebarColor;
            }
            set
            {
                mainFrame.TitlebarColor = value;
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public int TitlebarHeight
        {
            get
            {
                return mainFrame.TitlebarHeight;
            }
            set
            {
                mainFrame.TitlebarHeight = value;
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public Color BkColor
        {
            get
            {
                return mainFrame.BkColor;
            }
            set
            {
                mainFrame.BkColor = value;
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public int ShadowSize
        {
            get
            {
                return (int)mainFrame.ShadowSize;
            }
            set
            {
                mainFrame.ShadowSize = value;


                titleBar.Margin = new Thickness(mainFrame.ShadowSize,
                                        mainFrame.ShadowSize,
                                        mainFrame.ShadowSize,
                                        0);
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public int CornerRadius
        {
            get
            {
                return (int)mainFrame.CornerRadius;
            }
            set
            {
                mainFrame.CornerRadius = value;
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public BitmapSource ExtraIcon
        {
            get
            {
                return mainFrame.ExtraIcon;
            }
            set
            {
                mainFrame.ExtraIcon = value;
            }
        }

        [Bindable(true)]
        [Category("MainFrame")]
        public bool Maximized
        {
            get
            {
                return mainFrame.Maximized;
            }
            set
            {
                mainFrame.Maximized = value;
            }
        }

        // system buttons pressed ...
        private void OnClose(object sender, RoutedEventArgs e)
        {
            mainFrame.close();
        }

        private void OnRestore(object sender, RoutedEventArgs e)
        {
            mainFrame.Maximized = !mainFrame.Maximized;
        }

        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            mainFrame.minimize();
        }
    }
}
