using System;
using System.Collections.ObjectModel;
using System.Windows;

namespace Bismuth.Wpf.Controls
{
    public class PropertyItemGroup : DependencyObject
    {
        public PropertyItemGroup(string name, Type type)
        {
            Name = name ?? throw new ArgumentNullException(nameof(name));
            Type = type ?? throw new ArgumentNullException(nameof(type));
        }

        public string Name { get; }
        public Type Type { get; }

        public bool IsReadOnly
        {
            get { return (bool)GetValue(IsReadOnlyProperty); }
            private set { SetValue(_isReadOnlyPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _isReadOnlyPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsReadOnly), typeof(bool), typeof(PropertyItemGroup), new PropertyMetadata());
        public static readonly DependencyProperty IsReadOnlyProperty = _isReadOnlyPropertyKey.DependencyProperty;

        public bool IsAdvanced
        {
            get { return (bool)GetValue(IsAdvancedProperty); }
            private set { SetValue(_isAdvancedPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _isAdvancedPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsAdvanced), typeof(bool), typeof(PropertyItemGroup), new PropertyMetadata());
        public static readonly DependencyProperty IsAdvancedProperty = _isAdvancedPropertyKey.DependencyProperty;

        public string Category
        {
            get { return (string)GetValue(CategoryProperty); }
            private set { SetValue(_categoryPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _categoryPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Category), typeof(string), typeof(PropertyItemGroup), new PropertyMetadata());
        public static readonly DependencyProperty CategoryProperty = _categoryPropertyKey.DependencyProperty;

        public string DisplayName
        {
            get { return (string)GetValue(DisplayNameProperty); }
            private set { SetValue(_displayNamePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _displayNamePropertyKey = DependencyProperty.RegisterReadOnly(nameof(DisplayName), typeof(string), typeof(PropertyItemGroup), new PropertyMetadata());
        public static readonly DependencyProperty DisplayNameProperty = _displayNamePropertyKey.DependencyProperty;

        public string Description
        {
            get { return (string)GetValue(DescriptionProperty); }
            private set { SetValue(_descriptionPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _descriptionPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Description), typeof(string), typeof(PropertyItemGroup), new PropertyMetadata());
        public static readonly DependencyProperty DescriptionProperty = _descriptionPropertyKey.DependencyProperty;

        public object DefaultValue
        {
            get { return GetValue(DefaultValueProperty); }
            private set { SetValue(_defaultValuePropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _defaultValuePropertyKey = DependencyProperty.RegisterReadOnly(nameof(DefaultValue), typeof(object), typeof(PropertyItemGroup), new PropertyMetadata());
        public static readonly DependencyProperty DefaultValueProperty = _defaultValuePropertyKey.DependencyProperty;

        public string SourceTypeNames
        {
            get { return (string)GetValue(SourceTypeNamesProperty); }
            private set { SetValue(_sourceTypeNamesPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _sourceTypeNamesPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SourceTypeNames), typeof(string), typeof(PropertyItemGroup), new PropertyMetadata(0));
        public static readonly DependencyProperty SourceTypeNamesProperty = _sourceTypeNamesPropertyKey.DependencyProperty;

        public ReadOnlyCollection<PropertyItem> Items
        {
            get { return (ReadOnlyCollection<PropertyItem>)GetValue(ItemsProperty); }
            private set { SetValue(_itemsPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _itemsPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Items), typeof(ReadOnlyCollection<PropertyItem>), typeof(PropertyItemGroup), new PropertyMetadata(Array.AsReadOnly(new PropertyItem[0])));
        public static readonly DependencyProperty ItemsProperty = _itemsPropertyKey.DependencyProperty;

        public ReadOnlyCollection<object> Values
        {
            get { return (ReadOnlyCollection<object>)GetValue(ValuesProperty); }
            private set { SetValue(_valuesPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _valuesPropertyKey = DependencyProperty.RegisterReadOnly(nameof(Values), typeof(ReadOnlyCollection<object>), typeof(PropertyItemGroup), new PropertyMetadata(Array.AsReadOnly(new object[0])));
        public static readonly DependencyProperty ValuesProperty = _valuesPropertyKey.DependencyProperty;

        public Type ConcreteType
        {
            get { return (Type)GetValue(ConcreteTypeProperty); }
            set { SetValue(ConcreteTypeProperty, value); }
        }

        public static readonly DependencyProperty ConcreteTypeProperty = DependencyProperty.Register(nameof(ConcreteType), typeof(Type), typeof(PropertyItemGroup),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((PropertyItemGroup)d).OnConcreteTypeChanged()));

        public object Value
        {
            get { return GetValue(ValueProperty); }
            set { SetValue(ValueProperty, value); }
        }

        public static readonly DependencyProperty ValueProperty = DependencyProperty.Register(nameof(Value), typeof(object), typeof(PropertyItemGroup),
            new FrameworkPropertyMetadata(null, FrameworkPropertyMetadataOptions.BindsTwoWayByDefault, (d, e) => ((PropertyItemGroup)d).OnValueChanged()));

        private void OnConcreteTypeChanged()
        {

        }

        private void OnValueChanged()
        {

        }
    }
}
