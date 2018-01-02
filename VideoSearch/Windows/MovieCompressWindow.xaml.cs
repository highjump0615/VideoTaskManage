using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media.Imaging;
using VideoSearch.Model;

namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for MovieCompressWindow.xaml
    /// </summary>
    public partial class MovieCompressWindow : Window
    {
        #region Property
        private double _width = 0;
        private double _height = 0;

        public int Thickness
        {
            get { return (int)ThicknessSlider.Value; }
        }

        private Rect _region;
        public Rect Region
        {
            get { return _region; }
        }

        public String TaskName
        {
            get { return TaskNameEditor.Text; }
        }

        public String Sensitivity
        {
            get
            {
                return String.Format("{0}", 2 - CboSensitivity.SelectedIndex);
            }
        }

        public int RegionType
        {
            get
            {
                int nRegionType = 0;

                if ((bool)ChkInclude.IsChecked)
                    nRegionType = 1;
                else if ((bool)ChkExclude.IsChecked)
                    nRegionType = 2;

                return nRegionType;
            }
        }

        #endregion

        #region Contructor & Init
        public MovieCompressWindow(MovieItem movie)
        {
            InitializeComponent();

            LoadInfo(movie);
        }


        private void LoadInfo(MovieItem movie)
        {
            BitmapImage img = new BitmapImage(new Uri(movie.ThumbnailPath));

            if (img != null)
            {
                _width = img.Width;
                _height = img.Height;

                RegionEditor.Thumbnail = img;
            }
        }
        #endregion

        #region Handler
        private void OnApply(object sender, RoutedEventArgs e)
        {
            Rect region = RegionEditor.Region;

            if (_width == 0 || _height == 0)
            {
                MessageBox.Show("You can't set region!");
                Close();
                return;
            }

            if (((bool)ChkExclude.IsChecked || (bool)ChkInclude.IsChecked) &&
                (region.Width == 0 || region.Height == 0))
            {
                MessageBox.Show("You must set two points!");
                return;
            }

            DialogResult = true;

            double rate_x = _width / RegionEditor.ActualWidth;
            double rate_y = _height / RegionEditor.ActualHeight;

            _region = region;

            Close();
        }

        private void OnSettingRegion(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(RegionEditor);

            if (e.ClickCount == 1 && pos.X >= 0 && pos.Y >= 0 &&
               pos.X < RegionEditor.ActualWidth &&
               pos.Y < RegionEditor.ActualHeight)
                RegionEditor.SetPoint(pos);
        }

        private void OnUpdateRegion(object sender, MouseEventArgs e)
        {
            Point pos = e.GetPosition(RegionEditor);

            bool IsUpdatingRegion = e.LeftButton == MouseButtonState.Pressed;
            bool IsUpdatingDirection = e.RightButton == MouseButtonState.Pressed;

            if (IsUpdatingRegion && IsUpdatingDirection)
                return;

            if (IsUpdatingRegion || IsUpdatingDirection)
            {
                if (pos.X >= 0 && pos.Y >= 0 &&
                    pos.X < RegionEditor.ActualWidth &&
                    pos.Y < RegionEditor.ActualHeight)
                {
                    if (IsUpdatingRegion)
                        RegionEditor.UpdatePoint(pos);
                }
            }

        }

        private void OnResetRegion(object sender, RoutedEventArgs e)
        {
            RegionEditor.Reset();
        }

        private void OnSelectRegionType(object sender, RoutedEventArgs e)
        {
            if (ChkExclude == null || ChkInclude == null)
                return;

            if (sender == ChkInclude)
                ChkExclude.IsChecked = !ChkInclude.IsChecked;
            else
                ChkInclude.IsChecked = !ChkExclude.IsChecked;
        }
        #endregion

    }
}
