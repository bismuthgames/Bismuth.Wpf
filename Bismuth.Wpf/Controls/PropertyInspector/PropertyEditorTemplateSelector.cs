using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class PropertyEditorTemplateSelector : DataTemplateSelector
    {
        public DataTemplate DefaultTemplate { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var propertyItemGroup = item as PropertyItemGroup;
            if (propertyItemGroup == null) return null;

            var element = container as FrameworkElement;
            if (element == null) return null;

            var template = element.TryFindResource(new DataTemplateKey(propertyItemGroup.Type)) as DataTemplate;
            if (template != null) return template;

            return DefaultTemplate;
        }
    }
}
