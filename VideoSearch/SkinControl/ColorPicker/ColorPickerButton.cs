using System.ComponentModel;
using System.Windows.Controls;
using System.Windows.Media;

namespace VideoSearch.SkinControl.ColorPicker
{
    public class ColorPickerButton : Button
    {
        public ColorPickerButton()
        {
            if (DesignerProperties.GetIsInDesignMode(this))
            {
                Background = Brushes.Red;
            }
        }
    }
}
