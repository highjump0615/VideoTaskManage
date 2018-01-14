using System;
using System.Windows;
using System.Windows.Controls;
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
        private bool _isSelectedColor1 = false;
        private bool _isSelectedColor2 = false;

        private Color _color1, _color2;

        private Rect _region;
        public Rect Region
        {
            get { return _region; }
        }

        public String TaskName
        {
            get { return TaskNameEditor.Text; }
        }

        public String Colors
        {
            get
            {
                String Colors = "";

                if (ColorPickerButton1.IsEnabled && _isSelectedColor1)
                    Colors = String.Format("{0},{1},{2}", _color1.R, _color1.G, _color1.B);
                Colors += ";";
                if (ColorPickerButton2.IsEnabled && _isSelectedColor2)
                    Colors += String.Format("{0},{1},{2}", _color2.R, _color2.G, _color2.B);
                Colors += ";";

                return Colors;
            }
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

        public String AlarmInfo
        {
            get
            {
                return RegionEditor.AlarmInfo;
            }
        }
        #endregion

        #region Constructor & Init
        public MovieSearchWindow(MovieItem movie)
        {
            InitializeComponent();

            Owner = MainWindow.VideoSearchMainWindow;

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

        private void OnUpdateRegionAndDirection(object sender, MouseEventArgs e)
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
                    else
                        RegionEditor.UpdatePoint(pos, false);
                }
            }
        }

        private void OnSettingAlarmPos(object sender, MouseButtonEventArgs e)
        {
            Point pos = e.GetPosition(RegionEditor);

            if (pos.X >= 0 && pos.Y >= 0 &&
               pos.X < RegionEditor.ActualWidth &&
               pos.Y < RegionEditor.ActualHeight)

                RegionEditor.SetPoint(pos, false);
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
            RegionEditor.ResetAlarm();
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

        private void CboObjectType_SelectionChanged(object sender, System.Windows.Controls.SelectionChangedEventArgs e)
        {
            if (ColorPickerButton1 == null || ColorPickerButton2 == null)
                return;

            ComboBoxItem selectedItem = ((sender as ComboBox).SelectedItem as ComboBoxItem);

            String strType = selectedItem.Content.ToString();

            if (strType == "人")
            {
                ColorPickerButton1.IsEnabled = true;
                ColorPickerButton2.IsEnabled = true;
            }
            else if (strType == "人形")
            {
                ColorPickerButton1.IsEnabled = false;
                ColorPickerButton2.IsEnabled = false;
            }
            else
            {
                ColorPickerButton1.IsEnabled = true;
                ColorPickerButton2.IsEnabled = false;
            }
        }

        private void OnColorPicker(object sender, RoutedEventArgs e)
        {
            var dialog = new System.Windows.Forms.ColorDialog();

            if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                // need to convert from the ColorDialog GDI colorspace to the WPF colorspace
                var wpfColor = Color.FromArgb(dialog.Color.A, dialog.Color.R, dialog.Color.G, dialog.Color.B);

                var brush = new SolidColorBrush(wpfColor);

                if(sender == ColorPickerButton1)
                {
                    this.SelectedColor1.Background = brush;
                    _isSelectedColor1 = true;
                    _color1 = wpfColor;
                }
                else
                {
                    this.SelectedColor2.Background = brush;
                    _isSelectedColor2 = true;
                    _color2 = wpfColor;
                }
            }
        }
        #endregion
    }
}
