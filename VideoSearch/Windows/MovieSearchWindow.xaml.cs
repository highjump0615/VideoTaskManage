using System;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using VideoSearch.Model;

namespace VideoSearch.Windows
{
    /// <summary>
    /// Interaction logic for MovieSearchWindow.xaml
    /// </summary>
    public partial class MovieSearchWindow : Window
    {
        #region Property
        private double _width = 0;
        private double _height = 0;

        private Rect _region;
        public Rect Region
        {
            get { return _region; }
        }

        public String TaskName
        {
            get { return TaskNameEditor.Text; }
        }

        public String ObjectType
        {
            get
            {
                String strType = CboObjectType.Text;

                if (strType == "全部")
                    return "4";
                if (strType == "人")
                    return "1";
                if (strType == "人形")
                    return "5";
                if (strType == "车")
                    return "2";
                if (strType == "其他")
                    return "3";

                return "0";
            }
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

        #region Constructor & Init
        public MovieSearchWindow(MovieItem movie)
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

            if(IsUpdatingRegion || IsUpdatingDirection)
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

        private void OnSettingDirection(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(RegionEditor);

            if (pos.X >= 0 && pos.Y >= 0 &&
               pos.X < RegionEditor.ActualWidth &&
               pos.Y < RegionEditor.ActualHeight)

                RegionEditor.UpdateDirection(pos);
        }

        private void OnResetRegion(object sender, RoutedEventArgs e)
        {
            RegionEditor.Reset();
        }

        private void OnResetDirection(object sender, RoutedEventArgs e)
        {
            RegionEditor.ResetDirection();
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

        private void OnPicker1(object sender, RoutedEventArgs e)
        {
        }

        /// <summary>
        /// 点击上半身颜色按钮
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void OnButColor1(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();
            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // need to convert from the ColorDialog GDI colorspace to the WPF colorspace
                var wpfColor = Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);

                var brush = new SolidColorBrush(wpfColor);
                this.ButColor1.Background = brush;
            }
        }
    }
}
