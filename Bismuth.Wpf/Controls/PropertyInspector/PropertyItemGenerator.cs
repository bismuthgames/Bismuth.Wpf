using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Windows.Data;

namespace Bismuth.Wpf.Controls
{
    public class PropertyItemGenerator
    {
        public PropertyItemGenerator()
        {
            Items = new ReadOnlyObservableCollection<PropertyItemGroup>(_items);
        }

        private readonly ObservableCollection<PropertyItemGroup> _items = new ObservableCollection<PropertyItemGroup>();
        public ReadOnlyObservableCollection<PropertyItemGroup> Items { get; }

        internal void Add(object obj)
        {
            foreach (var item in CreateItems(obj))
                GetOrCreateGroup(item.Name, item.Type).Add(item);
        }

        internal void Remove(object obj)
        {
            var items = _items
                .SelectMany(g => g.Items)
                .Where(i => i.Source == obj)
                .ToArray();

            foreach (var item in items)
            {
                item.Group.Remove(item);
                if (item.Group.Items.Count == 0)
                    _items.Remove(item.Group);
            }
        }

        private PropertyItemGroup GetOrCreateGroup(string name, Type type)
        {
            var group = _items.FirstOrDefault(g => g.Name == name && g.Type == type);
            if (group == null)
            {
                group = new PropertyItemGroup(name, type);
                _items.Add(group);
            }
            return group;
        }

        private IEnumerable<PropertyItem> CreateItems(object obj)
        {
            if (obj == null) throw new ArgumentNullException(nameof(obj));

            return obj
                .GetType()
                .GetProperties()
                .Where(IsBrowsable)
                .Select(p => CreateItem(p, obj));
        }

        private bool IsBrowsable(PropertyInfo property)
        {
            return property.GetCustomAttribute<BrowsableAttribute>()?.Browsable ?? GetEditorBrowsableState(property) != EditorBrowsableState.Never;
        }

        private bool IsAdvanced(PropertyInfo property)
        {
            return GetEditorBrowsableState(property) == EditorBrowsableState.Advanced;
        }

        private EditorBrowsableState GetEditorBrowsableState(PropertyInfo property)
        {
            return property.GetCustomAttribute<EditorBrowsableAttribute>()?.State ?? EditorBrowsableState.Always;
        }

        private string GetDisplayName(PropertyInfo property)
        {
            return property.GetCustomAttribute<DisplayNameAttribute>()?.DisplayName ?? property.Name;
        }

        private string GetDescription(PropertyInfo property)
        {
            return property.GetCustomAttribute<DescriptionAttribute>()?.Description;
        }

        private string GetCategory(PropertyInfo property)
        {
            return property.GetCustomAttribute<CategoryAttribute>()?.Category ?? "Common";
        }

        private object GetDefaultValue(PropertyInfo property)
        {
            return property.GetCustomAttribute<DefaultValueAttribute>()?.Value ?? CreateDefaultValue(property.PropertyType);
        }

        private object CreateDefaultValue(Type type)
        {
            return type.IsValueType ? Activator.CreateInstance(type) : null;
        }

        private PropertyItem CreateItem(PropertyInfo property, object obj)
        {
            // Retrieving the PropertyDescriptor to get the IsReadOnly flag.
            // The PropertyInfo.CanWrite is not always false on read only properties.
            // E.g. public int MyProperty { get; private set; }
            var pd = TypeDescriptor.GetProperties(obj).Find(property.Name, false);

            var item = new PropertyItem
            (
                source: obj,
                name: property.Name,
                type: property.PropertyType,
                isReadOnly: pd.IsReadOnly,
                isAdvanced: IsAdvanced(property),
                category: GetCategory(property),
                displayName: GetDisplayName(property),
                description: GetDescription(property),
                defaultValue: GetDefaultValue(property)
            );

            var binding = new Binding(property.Name)
            {
                Source = obj,
                Mode = pd.IsReadOnly ? BindingMode.OneWay : BindingMode.TwoWay
            };

            BindingOperations.SetBinding(item, PropertyItem.ValueProperty, binding);

            return item;
        }
    }
}
