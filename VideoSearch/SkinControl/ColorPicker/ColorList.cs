using System;
using System.Collections.ObjectModel;
using System.Reflection;
using System.Windows.Media;

namespace VideoSearch.SkinControl.ColorPicker
{
    public class ColorList : ObservableCollection<Color>
    {
        public ColorList()
        {
            Type type = typeof(Colors);
            foreach(PropertyInfo propertyInfo in type.GetProperties(BindingFlags.Public | BindingFlags.Static))
            {
                if (propertyInfo.PropertyType == typeof(Color))
                    Add((Color)propertyInfo.GetValue(null, null));
            }
        }
    }
}
