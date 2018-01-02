using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace VideoSearch.SkinFrame
{
    /// <summary>
    /// Interaction logic for DialogFrame.xaml
    /// </summary>
    public partial class DialogFrame : UserControl
    {
        public DialogFrame()
        {
            InitializeComponent();
        }


        [Bindable(true)]
        [Category("DialogFrame")]
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
        [Category("DialogFrame")]
        public int ShadowSize
        {
            get
            {
                return (int)mainFrame.ShadowSize;
            }
            set
            {
                mainFrame.ShadowSize = value;
            }
        }

        [Bindable(true)]
        [Category("DialogFrame")]
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
        [Category("DialogFrame")]
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
        [Category("DialogFrame")]
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
        [Category("DialogFrame")]
        public Color TitleColor
        {
            get
            {
                return mainFrame.TitleColor;
            }
            set
            {
                mainFrame.TitleColor = value;
            }
        }

        [Bindable(true)]
        [Category("DialogFrame")]
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
        [Category("DialogFrame")]
        public int CornerRadius
        {
            get
            {
                return mainFrame.CornerRadius;
            }
            set
            {
                mainFrame.CornerRadius = value;
            }
        }

        [Bindable(true)]
        [Category("DialogFrame")]
        public int BorderSize
        {
            get
            {
                return mainFrame.BorderSize;
            }
            set
            {
                mainFrame.BorderSize = value;
            }
        }

        [Bindable(true)]
        [Category("DialogFrame")]
        public Color BorderColor
        {
            get
            {
                return mainFrame.BorderColor;
            }
            set
            {
                mainFrame.BorderColor = value;
            }
        }

        [Bindable(true)]
        [Category("DialogFrame")]
        public Thickness TitleMargin
        {
            get
            {
                return mainFrame.TitleMargin;
            }
            set
            {
                mainFrame.TitleMargin = value;
            }
        }

        [Bindable(true)]
        [Category("DialogFrame")]
        public TextAlignment TitleAlignment
        {
            get
            {
                return mainFrame.TitleAlignment;
            }
            set
            {
                mainFrame.TitleAlignment = value;
            }
        }

        [Bindable(true)]
        [Category("DialogFrame")]
        public int TitleFontSize
        {
            get
            {
                return mainFrame.TitleFontSize;
            }
            set
            {
                mainFrame.TitleFontSize = value;
            }
        }
    }
}
