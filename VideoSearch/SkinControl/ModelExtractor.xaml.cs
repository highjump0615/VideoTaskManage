using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoSearch.SkinControl
{
    /// <summary>
    /// Interaction logic for ModelExtractor.xaml
    /// </summary>
    public partial class ModelExtractor : UserControl
    {
        public ModelExtractor()
        {
            InitializeComponent();
        }

        #region Property

        private BitmapImage _thumbnail = null;

        public BitmapImage Thumbnail
        {
            set
            {
                _thumbnail = value;
                InvalidateVisual();
            }
        }

        public BitmapSource ClippedThumbnail
        {
            get
            {
                Geometry geometry = Outline.RenderedGeometry.Clone();
                Rect bounds = geometry.Bounds;
                Int32Rect rtBounds = new Int32Rect((int)bounds.Left, (int)bounds.Top, (int)bounds.Width, (int)bounds.Height);

                double scaleX = ActualWidth / _thumbnail.Width;
                double scaleY = ActualHeight / _thumbnail.Height;

                TransformedBitmap scaleBmp = new TransformedBitmap(_thumbnail, new ScaleTransform(scaleX, scaleY));
                CroppedBitmap src = new CroppedBitmap(scaleBmp, rtBounds);
                ImageBrush brImage = new ImageBrush(src);
     
                DrawingVisual drawingVisual = new DrawingVisual();
                DrawingContext drawingContext = drawingVisual.RenderOpen();

                geometry.Transform = new TranslateTransform(-rtBounds.X, -rtBounds.Y);
                drawingContext.DrawGeometry(brImage, new Pen(), geometry); ;
                drawingContext.Close();


                RenderTargetBitmap frame = new RenderTargetBitmap(rtBounds.Width, rtBounds.Height, _thumbnail.DpiX, _thumbnail.DpiY, PixelFormats.Pbgra32);
                frame.Render(drawingVisual);

                return frame;
            }
        }

        public Rect Bounds
        {
            get
            {
                if (Outline.Points.Count > 2)
                {
                    Rect bounds = Outline.RenderedGeometry.Bounds;
                    return bounds;
                }

                return new Rect();
            }
        }

        public bool IsValid
        {
            get
            {
                return IsValidModel();
            }
        }

        public PointCollection Path
        {
            get
            {
                return IsValid ? Outline.Points : null;
            }
        }
        #endregion

        #region Method
        public void AddPoint(Point pos)
        {
            if (_thumbnail == null)
                return;

            Outline.Points.Add(pos);
        }

        public void Reset()
        {
            Outline.Points.Clear();
        }

        private bool IsValidModel()
        {
            PointCollection points = Outline.Points;
            if (points.Count < 2) return false;

            if (points.Count == 3)
                return true;

            for (int i=1; i<points.Count; i++)
            {
                for (int j=i+1; j<points.Count; j++)
                {
                    if (IsCrossedLines(points[i - 1], points[i],
                                      points[j - 1], points[j]))
                        return false;
                }
            }

            return true;
        }

        private bool IsCrossedLines(Point start1, Point end1, Point start2, Point end2)
        {
#if false
            Point temp;
            if(start1.X > end1.X)
            {
                temp = end1;
                end1 = start1;
                start1 = end1;
            }

            if (start2.X > end2.X)
            {
                temp = end2;
                end2 = start2;
                start2 = end2;
            }
#endif
            double a1, b1, a2, b2, x, y;

            a1 = (start1.Y - end1.Y) / (start1.X - end1.X);
            b1 = start1.Y - a1 * start1.X;

            a2 = (start2.Y - end2.Y) / (start2.X - end2.X);
            b2 = start2.Y - a2 * start2.X;


            if (a1 == a2)
                return false;

            x = (b2 - b1) / (a1 - a2);
            y = a1 * x + b1;


            if (ContainsPoint(start1, end1, x, y) &&
                ContainsPoint(start2, end2, x, y))
            {
                return true;
            }


            return false;
        }

        private bool ContainsPoint(Point start, Point end, double x, double y)
        {
            if (((x > start.X && x < end.X) || 
                 (x > end.X && x < start.X)) &&
                ((y > start.Y && y < end.Y) ||
                 (y > end.Y && y < start.Y)))
                return true;

            return false;
        }
        private double GetAngle(Point start, Point end)
        {
            double angle = Math.Atan((end.Y-start.Y) / (start.X - end.X));

            if (start.X > end.X)
            {
                angle += Math.PI;
            }
            else if (start.Y < end.Y)
                angle += Math.PI * 2;


            return angle * 180 / Math.PI;
        }
#endregion

#region Render
        protected override void OnRender(DrawingContext drawingContext)
        {
            if(_thumbnail != null)
                drawingContext.DrawImage(_thumbnail, new Rect(0, 0, ActualWidth, ActualHeight));
        }
#endregion
    }
}
