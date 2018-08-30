using Microsoft.Win32;
using System;
using System.IO;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace VideoSearch.Windows
{
    public partial class ExtractPuppetWindow : Window
    {
        public ExtractPuppetWindow(BitmapImage image, PointCollection path)
        {
            InitializeComponent();

            _isStarted = false;
            ResetButton.IsEnabled = false;

            if (image != null)
            {
                _thumbnail = image;
                _extractorMargin = ExtractorMargin;
                ResetButton.IsEnabled = true;
            }

            if (path != null)
            {
                Extractor.Outline.Points = new PointCollection(path);
                ResetButton.IsEnabled = true;
                SaveButton.IsEnabled = true;
            }
        }

        #region Property

        private bool _isStarted = false;

        public Rect Bounds
        {
            get
            {
                return Extractor.Bounds;
            }
        }

        private BitmapImage _thumbnail = null;
        public BitmapImage Thumbnail
        {
            get
            {
                return _thumbnail;
            }
        }

        public BitmapSource ClippedThumbnail
        {
            get
            {
                return Extractor.ClippedThumbnail;
            }
        }

        public PointCollection Path
        {
            get
            {
                return Extractor.Path;
            }
        }

        public string BackgroundImagePath;

        private Thickness _extractorMargin;
        public Thickness ExtractorMargin
        {
            get
            {
                Thickness margin = new Thickness();

                if(Thumbnail != null)
                {
                    Rect rt = new Rect(0, 0, ExtractorContainer.ActualWidth, ExtractorContainer.ActualHeight);
                    double r1 = rt.Width / rt.Height;
                    double r2 = _thumbnail.Width / _thumbnail.Height;
                    double iw, ih, mx, my;
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

                    mx = (rt.Width - iw) / 2;
                    my = (rt.Height - ih) / 2;

                    margin = new Thickness(mx, my, mx, my);
                }

                if (_extractorMargin != margin)
                    _extractorMargin = margin;
                return _extractorMargin;
            }
        }

        #endregion

        #region Handler

        private void OnWindowLoaded(object sender, RoutedEventArgs e)
        {
            Extractor.Thumbnail = _thumbnail;
            Extractor.Margin = ExtractorMargin;
            Console.WriteLine(" === ExtractPuppet Window Road :{0}", Extractor.Margin);
        }

        private void OnMouseUp(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(Extractor);

            if (_isStarted &&
                e.ClickCount == 1 && pos.X >= 0 && pos.Y >= 0 &&
               pos.X < Extractor.ActualWidth &&
               pos.Y < Extractor.ActualHeight)
            {
                Extractor.AddPoint(pos);
                SaveButton.IsEnabled = true;
            }
        }

        private void OnOpenImage(object sender, RoutedEventArgs e)
        {
            OpenFileDialog dlg = new OpenFileDialog();
            dlg.Multiselect = false;
            dlg.FileName = "Open Image";
            dlg.DefaultExt = ".jpg";
            dlg.Filter = "Bitmap files (*.bmp)|*.BMP|Jpeg files (*.jpg)|*.JPG|Png files (*.png)|*.PNG|All Image Files(*.bmp;*.jpg;*.png)|*.BMP;*.JPG;*.PNG";
            dlg.FilterIndex = 9;
            var result = dlg.ShowDialog();

            if (result == true)
            {
                String[] filenames = dlg.FileNames;
                if(filenames.Length > 0)
                {
                    BackgroundImagePath = filenames[0];
                    FileInfo info = new FileInfo(filenames[0]);

                    if(info.Length > 1048576 * 7)
                    {
                        MessageBox.Show("Maximum image size is 7M!");
                        return;
                    }

                    _thumbnail = new BitmapImage(new Uri(filenames[0]));

                    Extractor.Margin = ExtractorMargin;
                    Extractor.Thumbnail = _thumbnail;

                    ResetButton.Content = "开始勾画";
                    _isStarted = false;

                    Extractor.Reset();
                    ResetButton.IsEnabled = true;
                    SaveButton.IsEnabled = false;
                }
            }
        }

        private void OnReset(object sender, RoutedEventArgs e)
        {
            if (_isStarted)
            {
                Extractor.Reset();
                SaveButton.IsEnabled = false;
            }
            else
            {
                _isStarted = true;
                ResetButton.Content = "重新勾画";
            }
        }

        private void OnSave(object sender, RoutedEventArgs e)
        {
            var isValid = Extractor.IsValid;

            if (isValid)
            {
                DialogResult = isValid;
                return;
            }

            MessageBox.Show("Extract result is invalid!");
            OnReset(sender, e);
        }
        
        private void OnMinimize(object sender, RoutedEventArgs e)
        {
            WindowState = WindowState.Minimized;
        }

        private void OnClose(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }

        #endregion
    }
}
