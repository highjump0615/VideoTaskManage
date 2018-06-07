using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.ComponentModel;
using System.Collections.ObjectModel;
using System.Collections.Specialized;

namespace VideoSearch.SkinControl
{
    /// <summary>
    /// Interaction logic for SkinButtonGroup.xaml
    /// </summary>
    public partial class SkinButtonGroup : UserControl
    {
        private bool m_isSelected = false;
        private Brush m_normal = new SolidColorBrush(Color.FromRgb(49, 47, 41));
        private Brush m_selected = new SolidColorBrush(Color.FromRgb(97, 97, 97));
        private Brush m_bottomNormal = new SolidColorBrush(Color.FromRgb(41, 39, 35));
        private Brush m_bottomSelected = new SolidColorBrush(Color.FromRgb(74, 74, 72));
        private Brush m_sepNormal = new SolidColorBrush(Color.FromRgb(97, 97, 97));
        private Brush m_sepSelected = Brushes.Transparent;

        private ObservableCollection<Control> m_children = null;
        private ObservableCollection<bool> mchildrenEnabled = new ObservableCollection<bool>();

        public event RoutedEventHandler GroupSelected = null;

        public SkinButtonGroup()
        {
            InitializeComponent();

            Children.CollectionChanged += OnChildrenChanged;
        }

        private void OnChildrenChanged(object sender, NotifyCollectionChangedEventArgs e)
        {
            if (e.Action == NotifyCollectionChangedAction.Add)
            {
                // save control enabled status
                foreach (Control item in e.NewItems)
                {
                    mchildrenEnabled.Add(item.IsEnabled);
                }                
            }
        }

        public ObservableCollection<Control> Children
        {
            get
            {
                if (m_children == null)
                {
                    m_children = new ObservableCollection<Control>();
                }
                return m_children;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (GroupSelected != null)
                GroupSelected(this, e);
        }

        protected void updateAndRefresh()
        {
            if (m_isSelected)
            {
                Background = m_selected;
                bottom_bar.Background = m_bottomSelected;
                separator.Background = m_sepSelected;
                Caption.Opacity = 0.5;
            }
            else
            {
                Background = m_normal;
                bottom_bar.Background = m_bottomNormal;
                separator.Background = m_sepNormal;
                Caption.Opacity = 0.2;
            }
        }

        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }
            set
            {
                m_isSelected = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinGroup")]
        public String Title
        {
            get
            {
                return Caption.Text;
            }
            set
            {
                Caption.Text = value;
                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinGroup")]
        public Brush Normal
        {
            get
            {
                return m_normal;
            }
            set
            {
                m_normal = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinGroup")]
        public Brush Selected
        {
            get
            {
                return m_selected;
            }
            set
            {
                m_selected = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinGroup")]
        public Brush Bottom_Normal
        {
            get
            {
                return m_bottomNormal;
            }
            set
            {
                m_bottomNormal = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinGroup")]
        public Brush Bottom_Selected
        {
            get
            {
                return m_bottomSelected;
            }
            set
            {
                m_bottomSelected = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinGroup")]
        public Brush Separator_Normal
        {
            get
            {
                return m_sepNormal;
            }
            set
            {
                m_sepNormal = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinGroup")]
        public Brush Separator_Selected
        {
            get
            {
                return m_sepSelected;
            }
            set
            {
                m_sepSelected = value;

                updateAndRefresh();
            }
        }

        private void OnIsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            for (var i = 0; i < Children.Count; i++)
            {
                var control = Children[i];
                if (IsEnabled)
                {
                    control.IsEnabled = mchildrenEnabled[i];
                }
                else
                {
                    control.IsEnabled = false;
                }
            }
        }
    }
}
