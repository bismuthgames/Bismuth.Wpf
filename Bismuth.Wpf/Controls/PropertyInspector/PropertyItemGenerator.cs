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

        public void Add(object obj)
        {

        }

        public void Remove(object obj)
        {

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
