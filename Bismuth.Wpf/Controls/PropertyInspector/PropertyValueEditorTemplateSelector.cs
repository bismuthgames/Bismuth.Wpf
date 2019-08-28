using System;
using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class PropertyValueEditorTemplateSelector : DataTemplateSelector
    {
        public ResourceDictionary ValueEditorTemplates { get; set; }

        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            var propertyItemGroup = item as PropertyItemGroup;
            if (propertyItemGroup == null) return null;

            var element = container as FrameworkElement;
            if (element == null) return null;

            var key = new DataTemplateKey(GetDataType(propertyItemGroup));

            if (ValueEditorTemplates != null &&
                ValueEditorTemplates[key] is DataTemplate localTemplate)
                return localTemplate;

            if (element.TryFindResource(key) is DataTemplate template)
                return template;

            return null;
        }

        private Type GetDataType(PropertyItemGroup propertyItemGroup)
        {
            if (propertyItemGroup.Type.IsEnum) return typeof(Enum);
            return propertyItemGroup.Type;
        }
    }
}
