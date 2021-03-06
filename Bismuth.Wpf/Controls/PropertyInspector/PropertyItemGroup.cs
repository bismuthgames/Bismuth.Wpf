﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;

namespace Bismuth.Wpf.Controls
{
    public class PropertyItemGroup : DependencyObject
    {
        private readonly List<PropertyItem> _items = new List<PropertyItem>();
        private bool _suppressOnChanged = false;
        private bool _suppressUpdate = false;

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

        private static readonly DependencyPropertyKey _sourceTypeNamesPropertyKey = DependencyProperty.RegisterReadOnly(nameof(SourceTypeNames), typeof(string), typeof(PropertyItemGroup), new PropertyMetadata());
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
            if (_suppressOnChanged) return;

            _suppressUpdate = true;

            foreach (var item in _items)
                item.ConcreteType = ConcreteType;

            _suppressUpdate = false;

            Update();
        }

        private void OnValueChanged()
        {
            if (_suppressOnChanged) return;

            _suppressUpdate = true;

            foreach (var item in _items)
                item.Value = Value;

            _suppressUpdate = false;

            Update();
        }

        internal void Add(PropertyItem item)
        {
            if (item.Group != null) throw new ArgumentException("PropertyItem is already member of another group.", nameof(item));
            item.Group = this;
            _items.Add(item);

            Update();
        }

        internal void Remove(PropertyItem item)
        {
            if (item.Group != this) throw new ArgumentException("PropertyItem is not member of this group.", nameof(item));
            item.Group = null;
            _items.Remove(item);

            Update();
        }

        internal void Update()
        {
            if (_suppressUpdate) return;

            _suppressOnChanged = true;

            ConcreteType = GetSharedOrDefault(i => i.ConcreteType);
            Value = GetSharedOrDefault(i => i.Value);

            _suppressOnChanged = false;

            IsReadOnly = GetSharedOrDefault(i => i.IsReadOnly, true);
            IsAdvanced = GetSharedOrDefault(i => i.IsAdvanced, false);
            Category = GetSharedOrDefault(i => i.Category, "Common");
            DisplayName = GetSharedOrDefault(i => i.DisplayName, Name);
            Description = GetSharedOrDefault(i => i.Description);
            DefaultValue = GetSharedOrDefault(i => i.DefaultValue);
            SourceTypeNames = GetSourceTypeNames();

            Items = Array.AsReadOnly(_items.ToArray());
            Values = Array.AsReadOnly(_items.Select(i => i.Value).ToArray());
        }

        private string GetSourceTypeNames()
        {
            return string.Join(", ", _items.Select(i => i.Source.GetType().Name).Distinct().OrderBy(i => i));
        }

        private T GetSharedOrDefault<T>(Func<PropertyItem, T> propertySelector, T defaultValue = default(T))
        {
            if (_items.Count == 0) return defaultValue;

            var value = propertySelector(_items[0]);
            for (int i = 1; i < _items.Count; i++)
            {
                var otherValue = propertySelector(_items[i]);
                if (!EqualityComparer<T>.Default.Equals(value, otherValue)) return defaultValue;
            }

            return value;
        }
    }
}
