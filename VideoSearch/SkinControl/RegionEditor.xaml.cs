using System.Collections.Generic;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoSearch.SkinControl
{

    public partial class RegionEditor : UserControl
    {
        #region Property

        private int _direction = -1;
        private List<Point> _posList;

        private BitmapImage _thumbnail = null;
        Rect _rtImage = new Rect();
        Rect _bounds = new Rect();
        double _rate = 1.0;

        public BitmapImage Thumbnail
        {
            set
            {
                _thumbnail = value;
                _rtImage = new Rect(0, 0, 0, 0);
            }
        }

        public Rect Region
        {
            get
            {
                if (_posList.Count == 2)
                {
                    Rect region = new Rect(_posList[0], _posList[1]);
                    region.Scale(_rate, _rate);

                    return region;
                }
                return new Rect();
            }
        }

        #endregion

        public RegionEditor()
        {
            InitializeComponent();

            _posList = new List<Point>();
        }


        #region Functions
        public void SetPoint(Point pos)
        {
            if (_rtImage.Width == 0 || _rtImage.Height == 0 || !_rtImage.Contains(pos))
                return;

            if (_posList.Count >= 2)
                Reset();

            _posList.Add(pos);

            InvalidateVisual();
        }

        public void UpdatePoint(Point pos)
        {
            if (_rtImage.Width == 0 || _rtImage.Height == 0 || !_rtImage.Contains(pos))
                return;

            if (_posList.Count < 2)
                _posList.Add(pos);
            else
                _posList[1] = pos;

            InvalidateVisual();
        }

        public void UpdateDirection(Point pos)
        {
            if (_rtImage.Width == 0 || _rtImage.Height == 0 || !_rtImage.Contains(pos))
                return;

            _direction = (_direction + 1) % 3;
            InvalidateVisual();
        }

        public void ResetDirection()
        {
            _direction = -1;
            InvalidateVisual();
        }

        public void Reset()
        {
            _posList.Clear();
            InvalidateVisual();
        }
        #endregion

        #region Drawing
        protected void drawArrowAtPoint(DrawingContext drawingContext, Pen pen, Point pos, bool isLeft)
        {
            Point pos1, pos2;
            double x = 12, y = 4;

            if (isLeft)
            {
                pos1 = new Point(pos.X + x, pos.Y - y);
                pos2 = new Point(pos.X + x, pos.Y + y);
            }
            else
            {
                pos1 = new Point(pos.X - x, pos.Y - y);
                pos2 = new Point(pos.X - x, pos.Y + y);
            }

            drawingContext.DrawLine(pen, pos, pos1);
            drawingContext.DrawLine(pen, pos, pos2);
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            double w = ActualWidth;
            double h = ActualHeight;

            if(_thumbnail != null)
            {
                Rect rt = new Rect(0, 0, w, h);
                Pen borderPen = new Pen(Brushes.White, 1.0);
                drawingContext.DrawRectangle(Brushes.Black, new Pen(), rt);


                if(_bounds != rt)
                {
                    rt.Inflate(-2, -2);

                    _rtImage = rt;
                    double r1 = rt.Width / rt.Height;
                    double r2 = _thumbnail.Width / _thumbnail.Height;
                    double iw, ih;
                    if (r1 > r2)
                    {
                        ih = rt.Height;
                        iw = ih * r2;
                    }
                    else
                    {
                        iw = rt.Width;
                        ih = iw / r2;
                    }
                    _rtImage = new Rect(_rtImage.Left + (_rtImage.Width - iw) / 2,
                                         (_rtImage.Height - ih) / 2,
                                        iw, ih);

                    _rate = _thumbnail.Width / _rtImage.Width;
                    rt = _rtImage;
                    rt.Inflate(1, 1);
                }

                // draw bounds
                drawingContext.DrawRectangle(Brushes.Transparent, borderPen, rt);
                drawingContext.DrawImage(_thumbnail, _rtImage);
            }

            Pen shapeOutlinePen = new Pen(Brushes.Red, 1);
            shapeOutlinePen.Freeze();

            // draw outline
            if (_posList.Count == 1)
            {
                drawingContext.DrawEllipse(Brushes.Red, shapeOutlinePen, _posList[0], 1, 1);
            }
            else if(_posList.Count == 2)
            {
                drawingContext.DrawRectangle(Brushes.Transparent, shapeOutlinePen, new Rect(_posList[0], _posList[1]));
            }

            // draw direction
            if (_direction >= 0)
            {
                double margin = 60;
                Pen dirPen = new Pen(Brushes.Green, 1.0);
                Point left = new Point(margin, h / 2);
                Point center = new Point(w / 2, h / 2);
                Point right = new Point(w - margin, h / 2);


                if (_direction == 0)
                {
                    drawingContext.DrawLine(dirPen, center, right);
                     drawArrowAtPoint(drawingContext, dirPen, right, false);

                }
                else if (_direction == 1)
                {
                    drawingContext.DrawLine(dirPen, left, center);
                    drawArrowAtPoint(drawingContext, dirPen, left, true);
                }
                else
                {
                    drawingContext.DrawLine(dirPen, left, right);
                    drawArrowAtPoint(drawingContext, dirPen, left, true);
                    drawArrowAtPoint(drawingContext, dirPen, right, false);
                }

                drawingContext.DrawEllipse(Brushes.White, dirPen, center, 4, 4);
            }
        }
        #endregion
    }
}
