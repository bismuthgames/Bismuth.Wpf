using System;
using System.Windows;

namespace Bismuth.Wpf.Controls
{
    public class PropertyItem : DependencyObject
    {
        private bool _suppressOnChanged = false;

        public PropertyItem(object source, string name, Type type, bool isReadOnly, bool isAdvanced, string category, string displayName, string description, object defaultValue)
        {
            Source = source ?? throw new ArgumentNullException(nameof(source));
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
            IsReadOnly = isReadOnly;
            IsAdvanced = isAdvanced;
            Category = category ?? throw new ArgumentNullException(nameof(category));
            DisplayName = displayName ?? throw new ArgumentNullException(nameof(displayName));
            Description = description;
            DefaultValue = defaultValue;
        }

        public PropertyItemGroup Group { get; internal set; }
        public object Source { get; }
        public string Name { get; }
        public Type Type { get; }
        public bool IsReadOnly { get; }
        public bool IsAdvanced { get; }
        public string Category { get; }
        public string DisplayName { get; }
        public string Description { get; }
        public object DefaultValue { get; }

        public Type ConcreteType
        {
            get { return (Type)GetValue(ConcreteTypeProperty); }
            set { SetValue(ConcreteTypeProperty, value); }
        }

        public static readonly DependencyProperty ConcreteTypeProperty = DependencyProperty.Register(nameof(ConcreteType), typeof(Type), typeof(PropertyItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((PropertyItem)d).OnConcreteTypeChanged()));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(PropertyItem),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((PropertyItem)d).OnValueChanged()));

        private void OnConcreteTypeChanged()
        {
            if (_suppressOnChanged) return;

            _suppressOnChanged = true;
            Value = CreateValue();
            _suppressOnChanged = false;

            Group?.Update();
        }

        private void OnValueChanged()
        {
            if (_suppressOnChanged) return;

            _suppressOnChanged = true;
            ConcreteType = Value?.GetType();
            _suppressOnChanged = false;

            Group?.Update();
        }

        private object CreateValue()
        {
            if (ConcreteType == null) return null;
            return Activator.CreateInstance(ConcreteType);
        }
    }
}
