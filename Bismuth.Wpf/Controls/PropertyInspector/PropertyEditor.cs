using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class PropertyEditor : Control
    {
        static PropertyEditor()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyEditor), new FrameworkPropertyMetadata(typeof(PropertyEditor)));
        }

        public object Header
        {
            get { return GetValue(HeaderProperty); }
            set { SetValue(HeaderProperty, value); }
        }

        public static readonly DependencyProperty HeaderProperty = DependencyProperty.Register(nameof(Header), typeof(object), typeof(PropertyEditor));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(PropertyEditor));

        public object DefaultValue
        {
            get { return GetValue(DefaultValueProperty); }
            set { SetValue(DefaultValueProperty, value); }
        }

        public static readonly DependencyProperty DefaultValueProperty = DependencyProperty.Register(nameof(DefaultValue), typeof(object), typeof(PropertyEditor));

        public DataTemplateSelector ValueEditorTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(ValueEditorTemplateSelectorProperty); }
            set { SetValue(ValueEditorTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty ValueEditorTemplateSelectorProperty = DependencyProperty.Register(nameof(ValueEditorTemplateSelector), typeof(DataTemplateSelector), typeof(PropertyEditor),
            new PropertyMetadata(new PropertyValueEditorTemplateSelector()));

        public object ValueEditor
        {
            get { return GetValue(ValueEditorProperty); }
            set { SetValue(ValueEditorProperty, value); }
        }

        public static readonly DependencyProperty ValueEditorProperty = DependencyProperty.Register(nameof(ValueEditor), typeof(object), typeof(PropertyEditor));

        public object ExtendedEditor
        {
            get { return GetValue(ExtendedEditorProperty); }
            set { SetValue(ExtendedEditorProperty, value); }
        }

        public static readonly DependencyProperty ExtendedEditorProperty = DependencyProperty.Register(nameof(ExtendedEditor), typeof(object), typeof(PropertyEditor));
    }
}
