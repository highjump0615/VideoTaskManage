using Microsoft.Windows.Design.PropertyEditing;
using System.Windows;

namespace VideoSearch.SkinControl.ColorPicker
{
    public class BrushExtendedEditor : ExtendedPropertyValueEditor
    {
        private EditorResources res = new EditorResources();

        public BrushExtendedEditor()
        {
            this.ExtendedEditorTemplate = res["BrushExtendedEditorTemplate"] as DataTemplate;
            this.InlineEditorTemplate = res["BrushInlineEditorTemplate"] as DataTemplate;
        }
    }
}
