using System;
using Microsoft.Windows.Design.Metadata;
using Microsoft.Windows.Design.PropertyEditing;

namespace VideoSearch.SkinControl.ColorPicker
{
    internal class Metadata : IProvideAttributeTable
    {
        public AttributeTable AttributeTable
        {
            get
            {
                AttributeTableBuilder builder = new AttributeTableBuilder();

                builder.AddCustomAttributes(
                    typeof(ColorPickerButton), "Background", 
                    PropertyValueEditor.CreateEditorAttribute(typeof(BrushExtendedEditor)));

                return builder.CreateTable();
            }
        }
    }
}
