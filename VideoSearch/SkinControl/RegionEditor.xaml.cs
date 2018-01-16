using System;
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
        private List<Point> _alarmPosList;
        private List<Point> _regionPosList;

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
                if (_regionPosList.Count == 2)
                {
                    Rect region = new Rect(_regionPosList[0], _regionPosList[1]);
                    region.Scale(_rate, _rate);

                    return region;
                }
                return new Rect();
            }
        }

        public String AlarmInfo
        {
            get
            {
                String alarmInfo = "";

                if(_direction >= 0 && _alarmPosList.Count >= 2)
                {
                    Point leftPos = _alarmPosList[0];
                    Point rightPos = _alarmPosList[1];

                    if (_alarmPosList[0].X > _alarmPosList[1].X)
                    {
                        leftPos = _alarmPosList[1];
                        rightPos = _alarmPosList[0];
                    }

                    alarmInfo = String.Format("{0},{1},{2},{3},{4}", (int)leftPos.X, (int)leftPos.Y, (int)rightPos.X, (int)rightPos.Y, _direction);
                }

                return alarmInfo;
            }
        }
        #endregion

        public RegionEditor()
        {
            InitializeComponent();

            _regionPosList = new List<Point>();
            _alarmPosList = new List<Point>();
        }


        #region Functions
        public void SetPoint(Point pos, bool isRegion = true)
        {
            if (_rtImage.Width == 0 || _rtImage.Height == 0 || !_rtImage.Contains(pos))
                return;

            List<Point> _posList = isRegion ? _regionPosList : _alarmPosList;

            if (!isRegion && _posList.Count == 0)
                _direction = 2;

            if (_posList.Count < 4)
            {
                if (_posList.Count == 3)
                    _posList[2] = pos;
                else
                    _posList.Add(pos);
            }
        }

        public void UpdatePoint(Point pos, bool isRegion = true)
        {
            if (_rtImage.Width == 0 || _rtImage.Height == 0 || !_rtImage.Contains(pos))
                return;

            List<Point> _posList = isRegion ? _regionPosList : _alarmPosList;

            if (_posList.Count < 2)
                _posList.Add(pos);
            else
            {
                if (_posList.Count == 3)
                {
                    Point startPos = _posList[2];

                    _direction = 2;

                    _posList[0] = _posList[2];
                    _posList.RemoveAt(2);
                }
                _posList[1] = pos;
            }

            InvalidateVisual();
        }

        public void UpdateDirection(Point pos)
        {
            if (_rtImage.Width == 0 || _rtImage.Height == 0 || !_rtImage.Contains(pos))
                return;

            if (_alarmPosList.Count == 3)
            {
                _direction = (_direction + 1) % 3;
            }
            InvalidateVisual();
        }

        public void ResetAlarm()
        {
            _alarmPosList.Clear();
            _direction = -1;
            InvalidateVisual();
        }

        public void Reset()
        {
            _regionPosList.Clear();
            InvalidateVisual();
        }
        #endregion

        #region Drawing
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
                                        _rtImage.Top + (_rtImage.Height - ih) / 2,
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
            if (_regionPosList.Count == 1)
            {
                drawingContext.DrawEllipse(Brushes.Red, shapeOutlinePen, _regionPosList[0], 1, 1);
            }
            else if(_regionPosList.Count == 2)
            {
                drawingContext.DrawRectangle(Brushes.Transparent, shapeOutlinePen, new Rect(_regionPosList[0], _regionPosList[1]));
            }

            // draw direction
            if (_direction >= 0 && _alarmPosList.Count > 1)
            {
                Pen dirPen = new Pen(Brushes.Blue, 1.0);
                Point left = _alarmPosList[0];
                Point right = _alarmPosList[1];
                Rect rtBounds = new Rect(left, right);
                double angle = Math.Atan(rtBounds.Height / rtBounds.Width);

                if (_alarmPosList[0].X > _alarmPosList[1].X)
                {
                    left = _alarmPosList[1];
                    right = _alarmPosList[0];

                    angle *= -1;
                }

                if (_alarmPosList[0].Y < _alarmPosList[1].Y)
                {
                    angle *= -1;
                }

                if (rtBounds.Width > 20 || rtBounds.Height > 20)
                {
                    Point center = new Point(rtBounds.Left + rtBounds.Width / 2, rtBounds.Top + rtBounds.Height / 2);

                    drawingContext.DrawLine(dirPen, left, right);
                    drawingContext.DrawEllipse(Brushes.White, dirPen, center, 4, 4);

                    if (_direction == 0)
                    {
                        drawArrowAtPoint(drawingContext, dirPen, right, false, angle);

                    }
                    else if (_direction == 1)
                    {
                        drawArrowAtPoint(drawingContext, dirPen, left, true, angle);
                    }
                    else
                    {
                        drawArrowAtPoint(drawingContext, dirPen, left, true, angle);
                        drawArrowAtPoint(drawingContext, dirPen, right, false, angle);
                    }
                }
            }
        }

        protected void drawArrowAtPoint(DrawingContext drawingContext, Pen pen, Point pos, bool isLeft, double rotate)
        {
            Point pos1, pos2;
            double size = 16.0;

            double angle = 30 * Math.PI / 180;

            if (isLeft)
            {
                pos1 = new Point(pos.X + size * Math.Cos(angle + rotate), pos.Y - size * Math.Sin(angle + rotate));
                pos2 = new Point(pos.X + size * Math.Cos(angle - rotate), pos.Y + size * Math.Sin(angle - rotate));
            }
            else
            {
                pos1 = new Point(pos.X - size * Math.Cos(angle - rotate), pos.Y - size * Math.Sin(angle - rotate));
                pos2 = new Point(pos.X - size * Math.Cos(angle + rotate), pos.Y + size * Math.Sin(angle + rotate));
            }

            drawingContext.DrawLine(pen, pos, pos1);
            drawingContext.DrawLine(pen, pos, pos2);
        }
        #endregion
    }
}
