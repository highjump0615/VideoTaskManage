﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace VideoSearch.SkinControl
{
    /// <summary>
    /// Interaction logic for TimeRangeMarker.xaml
    /// </summary>
    public partial class TimeRangeMarker : UserControl
    {
        private List<FormattedText> _times;

        public TimeRangeMarker()
        {
            InitializeComponent();

            _times = new List<FormattedText>();
            calcDisplayTimes(0);

        }

        [Bindable(true)]
        [Category("TimeRangeMarker")]
        public TimeSpan Duration
        {
            set
            {
                calcDisplayTimes(value.Ticks);
            }
        }

        protected void calcDisplayTimes(long duration)
        {
            long step = duration / 11;
            Brush timeBrush = new SolidColorBrush(Color.FromRgb(150, 150, 150));

            _times.Clear();

            for (int i = 1; i < 11; i++)
            {
                TimeSpan t = new TimeSpan(step * i);
                FormattedText fmtText = new FormattedText(t.ToString(@"mm\:ss\:ff"),
                            CultureInfo.GetCultureInfo("en-us"),
                            FlowDirection.LeftToRight,
                            new Typeface("Arial"), 12, timeBrush);


                _times.Add(fmtText);
                InvalidateVisual();
            }
        }

        protected override void OnRender(DrawingContext drawingContext)
        {
            Brush br = new SolidColorBrush(Color.FromRgb(100, 100, 100));
            Pen pen = new Pen(br, 2);

            double w = ActualWidth;
            double h = ActualHeight;
            double step = w / 53;

            Point start_large = new Point(0, 0);
            Point start = new Point(-1, h - 10);
            Point end = new Point(-1, h - 4);
            Point textPos = new Point(0, h - 30);
            int nIndex = 0;
            bool bDrawText = (_times.Count == 10) ? true : false;

                for (int i=0; i<55; i++)
            {
                if (i == 0)
                    continue;

                if(i % 5 == 0)
                {
                    drawingContext.DrawLine(pen, start_large, end);

                    if(bDrawText)
                    {
                        FormattedText text = _times[nIndex++];
                        textPos.X = start_large.X - text.Width/2;
                        drawingContext.DrawText(text, textPos);
                    }
                }
                else
                    drawingContext.DrawLine(pen, start, end);

                start_large.X += step;
                start.X += step;
                end.X += step;
            }
        }
    }
}