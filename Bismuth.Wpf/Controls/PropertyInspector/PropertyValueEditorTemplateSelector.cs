using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class PropertyValueEditorTemplateSelector : DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            return base.SelectTemplate(item, container);
        }
    }
}
