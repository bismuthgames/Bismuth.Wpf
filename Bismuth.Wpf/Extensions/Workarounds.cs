using System.Reflection;
using System.Windows.Controls;

namespace Bismuth.Wpf.Extensions
{
    internal static class Workarounds
    {
        private static readonly PropertyInfo _itemsHostProperty =
            typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

        private static readonly MethodInfo _ensureGeneratorMethod =
            typeof(Panel).GetMethod("EnsureGenerator", BindingFlags.Instance | BindingFlags.NonPublic);

        public static Panel GetItemsHost(this ItemsControl itemsControl) => (Panel)_itemsHostProperty.GetValue(itemsControl);
        public static void EnsureGenerator(this Panel panel) => _ensureGeneratorMethod.Invoke(panel, null);
    }
}
