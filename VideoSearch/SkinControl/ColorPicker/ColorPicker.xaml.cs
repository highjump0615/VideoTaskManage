using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;


namespace VideoSearch.SkinControl.ColorPicker
{
    /// <summary>
    /// Interaction logic for ColorPicker.xaml
    /// </summary>
    public partial class ColorPicker : UserControl
    {
        public ColorPicker()
        {
            InitializeComponent();
        }

        public static readonly DependencyProperty SelectedColorProperty = DependencyProperty.Register("SelectedColor", typeof(Color), typeof(ColorPicker), new FrameworkPropertyMetadata(null));
        public Color SelectedColor
        {
            get { return (Color)base.GetValue(SelectedColorProperty); }
            set { base.SetValue(SelectedColorProperty, value); }
        }

        public static readonly DependencyProperty SelectedBrushProperty = DependencyProperty.Register("SelectedBrush", typeof(SolidColorBrush), typeof(ColorPicker), new FrameworkPropertyMetadata(null));
        public SolidColorBrush SelectedBrush
        {
            get { return (SolidColorBrush)base.GetValue(SelectedBrushProperty); }
            set { base.SetValue(SelectedBrushProperty, value); }
        }

        private void OnSelectColor(object sender, RoutedEventArgs e)
        {
            SelectedColor = (Color)((Button)sender).Tag;
            SelectedBrush = new SolidColorBrush(SelectedColor);
        }
    }
}
