using System;
using System.Collections.Generic;
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
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.ComponentModel;
using System.Globalization;

namespace VideoSearch.SkinControl
{
    public enum TextRenderMode
    {
        None = 0,
        Left,
        Right,
        Top,
        Bottom,
        Center
    }

    /// <summary>
    /// Interaction logic for SkinButton.xaml
    /// </summary>
    public partial class SkinButton : UserControl
    {
        private bool m_isDown = false;
        private bool m_isOver = false;
        private bool m_isAltState = false;
        private bool m_isSelected = false;

        private BitmapSource m_OverImage = null;
        private BitmapSource m_NormalImage = null;
        private BitmapSource m_PressedImage = null;
        private BitmapSource m_DisabledImage = null;
        private BitmapSource m_SelectedImage = null;

        private BitmapSource m_AltOverImage = null;
        private BitmapSource m_AltNormalImage = null;
        private BitmapSource m_AltPressedImage = null;
        private BitmapSource m_AltDisabledImage = null;

        private FormattedText m_fmtText = null;
        private String m_text;
        private double m_txtOffsetWithImage = 4.0;
        private double m_txtOffsetX = 0;
        private double m_txtOffsetY = 0;
        private TextRenderMode m_textRenderMode = TextRenderMode.None;
        private Color m_txtColor = Colors.White;

        private Color m_normalColor = Colors.Transparent;
        private Color m_overColor = Colors.Transparent;
        private Color m_PressedColor = Colors.Transparent;
        private Color m_disabledColor = Colors.Transparent;

        [Category("Behavior")]
        public event RoutedEventHandler Click = null;

        private Rect m_rtImage = new Rect();
        private Point m_textPos = new Point();

        public SkinButton()
        {
            InitializeComponent();

            FontFamily = new FontFamily("Arial");
            FontSize = 12.0;
            Padding = new Thickness(4.0);

            this.IsEnabledChanged += SkinButton_IsEnabledChanged;
        }

        protected bool hitTest(Point pos)
        {
            Rect bounds = new Rect(0, 0, Width, Height);

            if (bounds.Contains(pos))
                return true;

            return false;
        }

        protected Size getImageRenderingSize(Rect bounds, ImageSource image, bool isAll_fill = false)
        {
            Size size = new Size();

            if (image != null)
            {
                size = new Size(image.Width, image.Height);

                double r1 = bounds.Width / bounds.Height;
                double r2 = image.Width / image.Height;

                if (!isAll_fill && bounds.Width >= image.Width && bounds.Height >= image.Height)
                    size = new Size(image.Width, image.Height);
                else if (r1 > r2)
                    size = new Size(bounds.Height * r2, bounds.Height);
                else
                    size = new Size(bounds.Width, bounds.Width / r2);
            }

            return size;
        }

        protected void prepareDrawingInfo()
        {
            double x, y;
            Size imgSize = new Size(0, 0);
            Size txtSize = new Size(0, 0);
            Rect bounds = new Rect(Padding.Left,
                       Padding.Top,
                       Width - Padding.Left - Padding.Right,
                       Height - Padding.Top - Padding.Bottom);


            if (m_text != null && m_text.Length > 0)
            {
                m_fmtText = new FormattedText(m_text,
                CultureInfo.GetCultureInfo("en-us"),
                FlowDirection.LeftToRight,
                new Typeface(FontFamily.Source),
                FontSize, new SolidColorBrush(m_txtColor));

                m_fmtText.MaxLineCount = 1;
                m_fmtText.Trimming = TextTrimming.CharacterEllipsis;
                m_fmtText.TextAlignment = TextAlignment.Left;

                txtSize = new Size(m_fmtText.Width, m_fmtText.Height);
            }
            else
                m_fmtText = null;

            if (m_textRenderMode == TextRenderMode.None)
            {
                m_textPos = new Point();
            }
            else if (m_textRenderMode == TextRenderMode.Center)
            {
                m_textPos = new Point(bounds.Left + (bounds.Width - txtSize.Width) / 2 + m_txtOffsetX,
                                     bounds.Top + (bounds.Height - txtSize.Height) / 2 + m_txtOffsetY);

            }
            else if (m_textRenderMode == TextRenderMode.Left)
            {
                m_textPos = new Point(bounds.Left + m_txtOffsetX,
                     bounds.Top + (bounds.Height - txtSize.Height) / 2 + m_txtOffsetY);

                x = m_textPos.X + txtSize.Width + m_txtOffsetWithImage;
                bounds.Offset(x, 0);
                bounds.Width -= x;
            }
            else if (m_textRenderMode == TextRenderMode.Top)
            {
                m_textPos = new Point(bounds.Left + (bounds.Width - txtSize.Width) / 2 + m_txtOffsetX,
                     bounds.Top + m_txtOffsetY);

                y = m_textPos.Y + txtSize.Height + m_txtOffsetWithImage;
                bounds.Offset(0, y);
                bounds.Height -= y;
            }
            else if (m_textRenderMode == TextRenderMode.Right)
            {
                x = txtSize.Width + m_txtOffsetX;

                m_textPos = new Point(bounds.Left + bounds.Width - x,
                     bounds.Top + (bounds.Height - txtSize.Height) / 2 + m_txtOffsetY);

                bounds.Width -= x + m_txtOffsetWithImage;
            }
            else if (m_textRenderMode == TextRenderMode.Bottom)
            {
                y = txtSize.Height + m_txtOffsetY;

                m_textPos = new Point(bounds.Left + (bounds.Width - txtSize.Width) / 2 + m_txtOffsetX,
                     bounds.Top + bounds.Height - y);

                bounds.Height -= y + m_txtOffsetWithImage;
            }

            if (m_NormalImage != null)
            {
#if false
                imgSize = getImageRenderingSize(bounds, m_NormalImage);
#else
                imgSize = new Size(m_NormalImage.Width, m_NormalImage.Height);
#endif
            }
            m_rtImage = new Rect(bounds.Left + (bounds.Width - imgSize.Width) / 2,
                                 bounds.Top + (bounds.Height - imgSize.Height) / 2,
                                 imgSize.Width, imgSize.Height);


            if (m_fmtText != null && m_textRenderMode == TextRenderMode.Right)
            {
                m_rtImage.X = bounds.Left;
                m_textPos.X = m_rtImage.Right + m_txtOffsetWithImage;
            }
        }

        protected void Refresh()
        {
            prepareDrawingInfo();
            InvalidateVisual();
        }

        protected override void OnRenderSizeChanged(SizeChangedInfo sizeInfo)
        {
            base.OnRenderSizeChanged(sizeInfo);


        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Rect bounds = new Rect(0, 0, Width, Height);
            Rect rtImage = m_rtImage;

            Pen borderPen = new Pen();
            SolidColorBrush bkBrush;

            if (IsEnabled)
            {
                if (m_isDown)
                {
                    if (m_isOver)
                    {
                        bkBrush = new SolidColorBrush(m_PressedColor);
                        drawingContext.DrawRectangle(bkBrush, borderPen, bounds);

                        if (m_isAltState && m_AltPressedImage != null)
                            drawingContext.DrawImage(m_AltPressedImage, rtImage);
                        else if (m_PressedImage != null)
                            drawingContext.DrawImage(m_PressedImage, rtImage);
                        else if (m_NormalImage != null)
                            drawingContext.DrawImage(m_NormalImage, rtImage);
                    }
                    else
                    {
                        bkBrush = new SolidColorBrush(m_normalColor);
                        drawingContext.DrawRectangle(bkBrush, borderPen, bounds);

                        if (m_isAltState && m_AltNormalImage != null)
                            drawingContext.DrawImage(m_AltNormalImage, rtImage);
                        else if (m_NormalImage != null)
                            drawingContext.DrawImage(m_NormalImage, rtImage);
                    }
                }
                else
                {
                    if (m_isOver)
                    {
                        bkBrush = new SolidColorBrush(m_overColor);
                        drawingContext.DrawRectangle(bkBrush, borderPen, bounds);

                        if (m_isAltState && m_AltOverImage != null)
                            drawingContext.DrawImage(m_AltOverImage, rtImage);
                        else if (m_OverImage != null)
                            drawingContext.DrawImage(m_OverImage, rtImage);
                        else if (m_NormalImage != null)
                            drawingContext.DrawImage(m_NormalImage, rtImage);
                    }
                    else
                    {
                        bkBrush = new SolidColorBrush(m_normalColor);
                        drawingContext.DrawRectangle(bkBrush, borderPen, bounds);

                        if (m_isAltState && m_AltNormalImage != null)
                            drawingContext.DrawImage(m_AltNormalImage, rtImage);
                        else if (m_NormalImage != null)
                            drawingContext.DrawImage(m_NormalImage, rtImage);
                    }
                }

                if (m_isSelected)
                {
                    drawingContext.DrawImage(m_SelectedImage, rtImage);
                }
            }
            else
            {
                bkBrush = new SolidColorBrush(m_disabledColor);
                drawingContext.DrawRectangle(bkBrush, borderPen, bounds);

                if (m_isAltState && m_AltDisabledImage != null)
                    drawingContext.DrawImage(m_AltDisabledImage, rtImage);
                else if (m_isAltState && m_AltNormalImage != null)
                    drawingContext.DrawImage(m_AltNormalImage, rtImage);
                else if (m_DisabledImage != null)
                    drawingContext.DrawImage(m_DisabledImage, rtImage);
                else if (m_NormalImage != null)
                    drawingContext.DrawImage(m_NormalImage, rtImage);
            }

            if (m_fmtText != null)
            {
                drawingContext.DrawText(m_fmtText, m_textPos);
            }
        }

        protected void SkinButton_IsEnabledChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsEnabled)
            {
                IsHitTestVisible = true;
                Opacity = 1.0;
            }
            else
            {
                IsHitTestVisible = false;

                if (m_DisabledImage == null)
                    Opacity = 0.4;
            }
        }

        protected override void OnMouseEnter(MouseEventArgs e)
        {
            base.OnMouseEnter(e);

            if (!IsEnabled)
                return;

            m_isOver = true;

            InvalidateVisual();
        }

        protected override void OnMouseLeave(MouseEventArgs e)
        {
            base.OnMouseLeave(e);

            if (!IsEnabled || m_isDown)
                return;

            m_isOver = false;

            InvalidateVisual();
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            base.OnMouseMove(e);

            if (m_isDown)
            {
                if (hitTest(e.GetPosition(this)))
                {
                    if (!m_isOver)
                    {
                        m_isOver = true;
                    }
                }
                else
                {
                    if (m_isOver)
                    {
                        m_isOver = false;
                    }
                }

                InvalidateVisual();
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (!IsEnabled)
                return;

            m_isDown = true;
            m_isOver = true;

            InvalidateVisual();

            CaptureMouse();
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonUp(e);

            if (!IsEnabled)
                return;

            m_isDown = false;
            m_isOver = (hitTest(e.GetPosition(this))) ? true : false;

            InvalidateVisual();

            if (IsMouseCaptured)
                ReleaseMouseCapture();

            if (Click != null && hitTest(e.GetPosition(this)))
                Click(this, e);
        }

        public bool IsShow
        {
            get
            {
                return this.Visibility == Visibility.Visible;
            }
            set
            {
                this.Visibility = value ? Visibility.Visible : Visibility.Hidden;
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public String Text
        {
            get
            {
                return m_text;
            }
            set
            {
                m_text = value;
                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public Color TextColor
        {
            get
            {
                return m_txtColor;
            }
            set
            {
                m_txtColor = value;

                InvalidateVisual();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public double OffsetX
        {
            get
            {
                return m_txtOffsetX;
            }
            set
            {
                m_txtOffsetX = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public double OffsetWithImage
        {
            get
            {
                return m_txtOffsetWithImage;
            }
            set
            {
                m_txtOffsetWithImage = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public double OffsetY
        {
            get
            {
                return m_txtOffsetY;
            }
            set
            {
                m_txtOffsetY = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public TextRenderMode TextMode
        {
            get
            {
                return m_textRenderMode;
            }
            set
            {
                m_textRenderMode = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource NormalImage
        {
            get
            {
                return m_NormalImage;
            }
            set
            {
                m_NormalImage = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource SelectedImage
        {
            get
            {
                return m_SelectedImage;
            }
            set
            {
                m_SelectedImage = value;
                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource OverImage
        {
            get
            {
                return m_OverImage;
            }
            set
            {
                m_OverImage = value;
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource PressedImage
        {
            get
            {
                return m_PressedImage;
            }
            set
            {
                m_PressedImage = value;
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource DisabledImage
        {
            get
            {
                return m_DisabledImage;
            }
            set
            {
                m_DisabledImage = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource AlternateNormalImage
        {
            get
            {
                return m_AltNormalImage;
            }
            set
            {
                m_AltNormalImage = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource AlternateOverImage
        {
            get
            {
                return m_AltOverImage;
            }
            set
            {
                m_AltOverImage = value;
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource AlternatePressedImage
        {
            get
            {
                return m_AltPressedImage;
            }
            set
            {
                m_AltPressedImage = value;
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public BitmapSource AlternateDisabledImage
        {
            get
            {
                return m_AltDisabledImage;
            }
            set
            {
                m_AltDisabledImage = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public Color NormalColor
        {
            get
            {
                return m_normalColor;
            }
            set
            {
                m_normalColor = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public Color OverColor
        {
            get
            {
                return m_overColor;
            }
            set
            {
                m_overColor = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public Color PressedColor
        {
            get
            {
                return m_PressedColor;
            }
            set
            {
                m_PressedColor = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public Color DisabledColor
        {
            get
            {
                return m_disabledColor;
            }
            set
            {
                m_disabledColor = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public bool IsAltState
        {
            get
            {
                return m_isAltState;
            }
            set
            {
                m_isAltState = value;

                Refresh();
            }
        }

        [Bindable(true)]
        [Category("SkinButton")]
        public bool IsSelected
        {
            get
            {
                return m_isSelected;
            }
            set
            {
                m_isSelected = value;
                Refresh();
            }
        }
    }
}
