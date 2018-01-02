using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Globalization;
using System.ComponentModel;

namespace VideoSearch.SkinControl
{

    public delegate void FrameStateChangedHandler(bool isMaximized);

    /// <summary>
    /// Interaction logic for SkinFrame.xaml
    /// </summary>
    public partial class SkinFrame : Grid
    {
        private enum MouseState
        {
            Normal = 0,
            Move_Window,
            Resize_Right,
            Resize_Bottom,
            Resize_RightBottom,
        }

        public FrameStateChangedHandler FrameStateChanged = null;

        private const int SM_CYCAPTION = 4;
        private const int SM_CXFULLSCREEN = 16;
        private const int SM_CYFULLSCREEN = 17;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        internal static extern int GetSystemMetrics(int Point);

        private const double Resize_Area_Size = 6.0;
        private Thickness UnInitializedMargin = new Thickness(-10000.0);

        private bool m_isCanResize = true;
        private bool m_hasTitlebar = true;
        private bool m_isMaximized = false;
        private bool m_isDuringUpdateLayout = false;

        private double m_titlebarHeight;
        private double m_cornerRadius = 4.0;

        private double m_win_left;
        private double m_win_top;
        private double m_win_width;
        private double m_win_height;

        private Point m_startPos;
        private Window m_parentWindow = null;
        private Grid m_contentView = null;
        private Thickness m_contentMargin = new Thickness(-10000.0);

        private Color m_titlebarColor = Colors.Black;
        private Color m_bkColor = Colors.DarkGray;
        private Color m_borderColor = Colors.Black;
        private double m_borderSize = 0.0;

        private MouseState m_mouseState = MouseState.Normal;

        // shadow
        private DropShadowEffect m_shadow = null;

        // for draw, move
        private Rect m_rtRestore;   // for restore window
        private Rect m_rtDraw;      // for drawing contents
        private Rect m_rtTitlebar;  // for drawing titlebar
        private Rect m_rtIcon;      // for drawing icon
        private Rect m_rtMove;      // for moving parent window

        // icon & text
        private BitmapSource m_extraIcon;
        private Thickness m_titleMargin;
        private TextAlignment m_titleAlignment = TextAlignment.Center;
        private Typeface m_titleTF = new Typeface("Arial");
        private double m_titleFontSize = 12.0;
        private Color m_titleColor = Colors.Black;

        // for hitTest ...
        private Rect m_rtRight;
        private Rect m_rtBottom;
        private Rect m_rtRightBottom;

        public SkinFrame()
        {
            InitializeComponent();

            // Create Drop Shadow ...
            m_shadow = new DropShadowEffect();
            m_shadow.Color = Colors.Black;
            m_shadow.Direction = 0.0;
            m_shadow.Opacity = 0.8;
            m_shadow.ShadowDepth = 0.0;
            m_shadow.BlurRadius = 12;

            Effect = m_shadow;

            // Titlebar ...
            m_titlebarHeight = 25; // (double)GetSystemMetrics(SM_CYCAPTION);
        }

        public void minimize()
        {
            if (m_parentWindow != null)
                m_parentWindow.WindowState = WindowState.Minimized;
        }

        public void close()
        {
            if (m_parentWindow != null)
                m_parentWindow.Close();
        }

        protected void updateWindowState(bool needsRefresh = true)
        {
            if (m_parentWindow != null)
            {
                m_isDuringUpdateLayout = true;

                if (m_isMaximized)
                {
                    m_rtRestore = new Rect(m_parentWindow.Left,
                                            m_parentWindow.Top,
                                            m_parentWindow.Width,
                                            m_parentWindow.Height);

                    m_parentWindow.Left = 0;
                    m_parentWindow.Top = 0;
                    m_parentWindow.Width = GetSystemMetrics(SM_CXFULLSCREEN);
                    m_parentWindow.Height = GetSystemMetrics(SM_CYFULLSCREEN) + GetSystemMetrics(SM_CYCAPTION);

                    Effect = null;
                }
                else
                {
                    m_parentWindow.Left = m_rtRestore.Left;
                    m_parentWindow.Top = m_rtRestore.Top;
                    m_parentWindow.Width = m_rtRestore.Width;
                    m_parentWindow.Height = m_rtRestore.Height;

                    Effect = m_shadow;
                }

                m_isDuringUpdateLayout = false;

                if(needsRefresh)
                    updateAndRefresh();
            }
        }


        protected Window getParentWindow(DependencyObject obj)
        {
            if (obj == null)
                return null;

            Window parentWindow = null;

            DependencyObjectType type = obj.DependencyObjectType;

            DependencyObjectType uiType = DependencyObjectType.FromSystemType(typeof(FrameworkElement));
            DependencyObjectType winType = DependencyObjectType.FromSystemType(typeof(Window));
            DependencyObjectType gridType = DependencyObjectType.FromSystemType(typeof(Grid));

            if (!type.IsSubclassOf(uiType))
                return parentWindow;

            if (type.IsSubclassOf(winType))
                parentWindow = (Window)obj;
            else
            {
                parentWindow = (Window)getParentWindow(((FrameworkElement)obj).Parent);

                if (parentWindow != null && m_contentView == null && gridType.IsInstanceOfType(obj))
                    m_contentView = (Grid)obj;
            }

            return parentWindow;
        }

        protected void updateCursor(Point pos)
        {
            if (m_rtRight.Contains(pos))
                Cursor = Cursors.SizeWE;
            else if (m_rtBottom.Contains(pos))
                Cursor = Cursors.SizeNS;
            else if (m_rtRightBottom.Contains(pos))
                Cursor = Cursors.SizeNWSE;
            else
                Cursor = Cursors.Arrow;
        }

        protected void updateMouseState(Point pos)
        {
            if (m_isCanResize)
            {
                if (m_rtRight.Contains(pos))
                    m_mouseState = MouseState.Resize_Right;
                else if (m_rtBottom.Contains(pos))
                    m_mouseState = MouseState.Resize_Bottom;
                else if (m_rtRightBottom.Contains(pos))
                    m_mouseState = MouseState.Resize_RightBottom;
                else if (m_rtMove.Contains(pos))
                    m_mouseState = MouseState.Move_Window;
                else
                    m_mouseState = MouseState.Normal;
            }
            else
            {
                if (m_rtMove.Contains(pos))
                    m_mouseState = MouseState.Move_Window;
                else
                    m_mouseState = MouseState.Normal;
            }
        }

        protected void updateAndRefresh()
        {
            if (m_parentWindow == null || m_isDuringUpdateLayout)
                return;

            bool isInitialized = (m_rtDraw.Width > 0 && m_rtDraw.Height > 0);

            double shadowSize = m_isMaximized ? 0 : m_shadow.BlurRadius;

            // set margin for shadow ...
            UIElementCollection children = m_contentView.Children;

            foreach (UIElement element in children)
            {
                DependencyObjectType gridType = DependencyObjectType.FromSystemType(typeof(Grid));

                if (gridType.IsInstanceOfType(element))
                {
                    Grid gridElement = (Grid)element;

                    if (m_contentMargin == UnInitializedMargin)
                        m_contentMargin = gridElement.Margin;

                    gridElement.Margin = new Thickness(m_contentMargin.Left + shadowSize,
                                                        m_contentMargin.Top + shadowSize,
                                                        m_contentMargin.Right + shadowSize,
                                                        m_contentMargin.Bottom + shadowSize);
                    String name = ((Grid)element).Name;
                    break;
                }
            }

            ((FrameworkElement)Parent).Margin = new Thickness(shadowSize, shadowSize, shadowSize, shadowSize);

            if (!isInitialized && Maximized)
                updateWindowState(false);

            Size newSize = new Size(m_parentWindow.Width - shadowSize * 2, m_parentWindow.Height - shadowSize * 2);

            // other process
            if (isInitialized)
            {
                double min_w = Math.Max((Resize_Area_Size + shadowSize) * 2, 0);
                double min_h = Math.Max((Resize_Area_Size + shadowSize) * 2, 0);
                if (newSize.Width <= min_w || newSize.Height <= min_h)
                    return;
            }

            m_rtDraw = new Rect(0, 0, newSize.Width, newSize.Height);

            m_rtTitlebar = new Rect(0, 0, newSize.Width, m_titlebarHeight);
            m_rtIcon = new Rect(0, 0, m_titlebarHeight - 1, m_titlebarHeight - 1);

            m_rtMove = HasTitlebar ? m_rtTitlebar : m_rtDraw;


            m_rtRight = new Rect(newSize.Width - Resize_Area_Size, 0, Resize_Area_Size, newSize.Height - Resize_Area_Size);
            m_rtBottom = new Rect(0, newSize.Height - Resize_Area_Size, newSize.Width - Resize_Area_Size, Resize_Area_Size);
            m_rtRightBottom = new Rect(newSize.Width - Resize_Area_Size, newSize.Height - Resize_Area_Size, Resize_Area_Size, Resize_Area_Size);

            InvalidateVisual();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            if (m_parentWindow == null)
            {
                m_parentWindow = getParentWindow(this);
            }

            updateAndRefresh();
        }

        protected override void OnRender(DrawingContext dc)
        {
            if (m_parentWindow == null)
            {
                base.OnRender(dc);
                return;
            }
            titlebar.Visibility = Visibility.Hidden;

            Brush brBk = new SolidColorBrush(m_bkColor);
            Pen pen = new Pen(new SolidColorBrush(m_borderColor), m_borderSize);
            Pen transPen = new Pen();
            double radius = (m_isMaximized) ? 0 : m_cornerRadius;

            // draw background
            dc.DrawRoundedRectangle(brBk, pen, m_rtDraw, radius, radius);
            dc.DrawRoundedRectangle(brBk, transPen, m_rtDraw, radius, radius);

            // draw titlebar
            if (HasTitlebar)
            {
                // draw background & border
                Brush brTitle = new SolidColorBrush(m_titlebarColor);
                dc.DrawRoundedRectangle(brTitle, pen, m_rtTitlebar, radius, radius);
                if (radius > 0.0)
                {
                    Rect rtBottom = Rect.Offset(m_rtTitlebar, 0, m_cornerRadius);
                    rtBottom.Height = m_titlebarHeight - m_cornerRadius;
                    dc.DrawRectangle(brTitle, pen, rtBottom);
                }

                // draw background
                dc.DrawRoundedRectangle(brTitle, transPen, m_rtTitlebar, radius, radius);
                if (radius > 0.0)
                {
                    Rect rtBottom = Rect.Offset(m_rtTitlebar, 0, m_cornerRadius);
                    rtBottom.Height = m_titlebarHeight - m_cornerRadius;
                    dc.DrawRectangle(brTitle, transPen, rtBottom);
                }


                Rect rtIcon = new Rect(0, 0, 0, 0);
                // draw icon
                if (m_parentWindow.Icon != null)
                {
                    ImageSource icon = m_parentWindow.Icon;

                    double rate = icon.Width / icon.Height;
                    double height = m_titlebarHeight;
                    rtIcon = new Rect(8, 0, height * rate, height);

                    if (rtIcon.Height > icon.Height)
                    {
                        rtIcon = new Rect(8, (height - icon.Height) / 2, icon.Width, icon.Height);
                    }


                    dc.DrawImage(icon, rtIcon);
                }

                // draw extra icon
                if (m_extraIcon != null)
                {
                    double rate = m_extraIcon.Width / m_extraIcon.Height;
                    double height = m_titlebarHeight;

                    rtIcon = new Rect(rtIcon.Right, (height - m_extraIcon.Height) / 2, m_extraIcon.Width, m_extraIcon.Height);

                    dc.DrawImage(m_extraIcon, rtIcon);
                }


                // draw title
                if (m_parentWindow.Title != null && m_parentWindow.Title.Length > 0)
                {
                    DependencyProperty titleProperty = Window.TitleProperty;
                    FormattedText ft = new FormattedText(m_parentWindow.Title,
                                                CultureInfo.GetCultureInfo("en-us"),
                                                FlowDirection.LeftToRight,
                                                m_titleTF,
                                                m_titleFontSize, new SolidColorBrush(m_titleColor));

                    ft.MaxLineCount = 1;
                    ft.MaxTextWidth = Math.Max(10, m_rtTitlebar.Width - m_titleMargin.Left - m_titleMargin.Right);
                    ft.Trimming = TextTrimming.CharacterEllipsis;
                    ft.TextAlignment = m_titleAlignment;

                    Point titlePos = new Point(m_titleMargin.Left + m_shadow.BlurRadius,
                                                m_titleMargin.Top + m_shadow.BlurRadius);
                    titlePos.Y += ((m_titleMargin.Bottom - m_titleMargin.Top) - ft.Height) / 2;

                    dc.DrawText(ft, titlePos);
                }
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (m_parentWindow == null)
                return;

            Point pos = e.GetPosition(this);

            updateMouseState(pos);

            if (m_isMaximized)
            {
                if (CanResize && m_mouseState == MouseState.Move_Window && e.ClickCount == 2)
                {
                    Maximized = false;
                    m_mouseState = MouseState.Normal;

                    return;
                }
            }
            else
            {
                if (CanResize && m_mouseState == MouseState.Move_Window && e.ClickCount == 2)
                {
                    Maximized = true;
                    m_mouseState = MouseState.Normal;

                    return;
                }

                Point locaoPos = new Point(m_parentWindow.Left, m_parentWindow.Top);
                Point screenPos = PointToScreen(locaoPos);

                m_win_left = m_parentWindow.Left;
                m_win_top = m_parentWindow.Top;

                m_win_width = m_parentWindow.Width;
                m_win_height = m_parentWindow.Height;

                m_startPos = PointToScreen(pos);

                CaptureMouse();
            }
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (IsMouseCaptured)
            {
                ReleaseMouseCapture();
            }

            Point pos = e.GetPosition(this);

            m_mouseState = MouseState.Normal;

            if (m_isCanResize && !m_isMaximized)
                updateCursor(pos);
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            Point pos = e.GetPosition(this);

            if (m_isCanResize && !m_isMaximized && m_mouseState == MouseState.Normal)
                updateCursor(pos);

            if (m_mouseState != MouseState.Normal && !m_isMaximized)
            {
                Point curPos = PointToScreen(pos);
                double xDiff = curPos.X - m_startPos.X;
                double yDiff = curPos.Y - m_startPos.Y;

                if (m_mouseState == MouseState.Move_Window)
                {
                    m_parentWindow.Left = m_win_left + xDiff;
                    m_parentWindow.Top = m_win_top + yDiff;
                }
                else
                {
                    double w = m_win_width;
                    double h = m_win_height;
                    if (m_mouseState == MouseState.Resize_Right)
                    {
                        w += xDiff;
                    }
                    else if (m_mouseState == MouseState.Resize_Bottom)
                    {
                        h += yDiff;
                    }
                    else if (m_mouseState == MouseState.Resize_RightBottom)
                    {
                        w += xDiff;
                        h += yDiff;
                    }

                    if (w > 0)
                        m_parentWindow.Width = w;
                    if (h > 0)
                        m_parentWindow.Height = h;
                }
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public bool CanResize
        {
            get
            {
                return m_isCanResize;
            }
            set
            {
                m_isCanResize = value;
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public bool HasTitlebar
        {
            get
            {
                return m_hasTitlebar;
            }
            set
            {
                m_hasTitlebar = value;

                if (m_hasTitlebar)
                    titlebar.Visibility = Visibility.Visible;
                else
                    titlebar.Visibility = Visibility.Hidden;
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public int TitlebarHeight
        {
            get
            {
                return (int)m_titlebarHeight;
            }
            set
            {
                m_titlebarHeight = (double)value;

                titlebar.Height = m_titlebarHeight;
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public Color TitlebarColor
        {
            get
            {
                return m_titlebarColor;
            }
            set
            {
                m_titlebarColor = value;
                titlebar.Background = new SolidColorBrush(m_titlebarColor);
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public Color TitleColor
        {
            get
            {
                return m_titleColor;
            }
            set
            {
                m_titleColor = value;
                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public Color BkColor
        {
            get
            {
                return m_bkColor;
            }
            set
            {
                m_bkColor = value;
                Background = new SolidColorBrush(m_bkColor);
                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public int CornerRadius
        {
            get
            {
                return (int)m_cornerRadius;
            }
            set
            {
                m_cornerRadius = Math.Min((double)value, (int)(m_titlebarHeight / 2));

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public int BorderSize
        {
            get
            {
                return (int)m_borderSize;
            }
            set
            {
                m_borderSize = Math.Min((double)value, 4.0);

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public Color BorderColor
        {
            get
            {
                return m_borderColor;
            }
            set
            {
                m_borderColor = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public Thickness TitleMargin
        {
            get
            {
                return m_titleMargin;
            }
            set
            {
                m_titleMargin = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public TextAlignment TitleAlignment
        {
            get
            {
                return m_titleAlignment;
            }
            set
            {
                m_titleAlignment = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public int TitleFontSize
        {
            get
            {
                return (int)m_titleFontSize;
            }
            set
            {
                m_titleFontSize = value;

                updateAndRefresh();
            }
        }

        [Bindable(true)]
        [Category("SkinFrame")]
        public BitmapSource ExtraIcon
        {
            get
            {
                return m_extraIcon;
            }
            set
            {
                m_extraIcon = value;

                updateAndRefresh();
            }
        }

        public int ShadowDirection
        {
            get
            {
                return (int)m_shadow.Direction;
            }
            set
            {
                m_shadow.Direction = value;
                Effect = m_shadow;

                updateAndRefresh();
            }
        }

        public int ShadowDepth
        {
            get
            {
                return (int)m_shadow.ShadowDepth;
            }
            set
            {
                m_shadow.ShadowDepth = (double)Math.Min(2, value);
                Effect = m_shadow;

                updateAndRefresh();
            }
        }

        public int ShadowSize
        {
            get
            {
                return (int)m_shadow.BlurRadius;
            }
            set
            {
                m_shadow.BlurRadius = (double)Math.Min(24, value);
                Effect = m_shadow;

                updateAndRefresh();
            }
        }

        public bool Maximized
        {
            get
            {
                return m_isMaximized;
            }
            set
            {
                if(m_isMaximized != value)
                {
                    m_isMaximized = value;
                    if (FrameStateChanged != null)
                        FrameStateChanged(m_isMaximized);
                    updateWindowState();
                }
            }
        }

    }
}
