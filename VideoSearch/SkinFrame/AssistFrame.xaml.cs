using System.Windows.Controls;
using System.Windows.Media;
using System.ComponentModel;

namespace VideoSearch.SkinFrame
{
    /// <summary>
    /// Interaction logic for AssistFrame.xaml
    /// </summary>
    public partial class AssistFrame : UserControl
    {
        public AssistFrame()
        {
            InitializeComponent();
        }

        [Bindable(true)]
        [Category("AssistFrame")]
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
        [Category("AssistFrame")]
        public int ShadowDirection
        {
            get
            {
                return (int)mainFrame.ShadowDirection;
            }
            set
            {
                mainFrame.ShadowDirection = value;
            }
        }

        [Bindable(true)]
        [Category("AssistFrame")]
        public int ShadowDepth
        {
            get
            {
                return (int)mainFrame.ShadowDepth;
            }
            set
            {
                mainFrame.ShadowDepth = value;
            }
        }


        [Bindable(true)]
        [Category("AssistFrame")]
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
        [Category("AssistFrame")]
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
        [Category("AssistFrame")]
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
        [Category("AssistFrame")]
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
    }
}
