using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Media;

namespace Bismuth.Wpf.Extensions
{
    public static class VisualTree
    {
        public static T FindVisualParent<T>(this DependencyObject current) where T : DependencyObject
        {
            return current
                .EnumerateVisualParents()
                .OfType<T>()
                .FirstOrDefault();
        }

        public static T FindVisualParentByName<T>(this DependencyObject current, string name) where T : DependencyObject
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return current
                .EnumerateVisualParents()
                .OfType<T>()
                .FirstOrDefault(parent => parent.MatchOnName(name));
        }

        public static T FindVisualChild<T>(this DependencyObject current) where T : DependencyObject
        {
            return current
                .EnumerateVisualChildren()
                .OfType<T>()
                .FirstOrDefault();
        }

        public static T FindVisualChildByName<T>(this DependencyObject current, string name) where T : DependencyObject
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return current
                .EnumerateVisualParents()
                .OfType<T>()
                .FirstOrDefault(child => child.MatchOnName(name));
        }

        public static IEnumerable<T> FindVisualChildren<T>(this DependencyObject current) where T : DependencyObject
        {
            return current
                .EnumerateVisualChildren()
                .OfType<T>();
        }

        public static IEnumerable<DependencyObject> EnumerateVisualParents(this DependencyObject current)
        {
            var parent = VisualTreeHelper.GetParent(current);
            while (parent != null)
            {
                yield return parent;
                parent = VisualTreeHelper.GetParent(parent);
            }
        }

        public static IEnumerable<DependencyObject> EnumerateVisualChildren(this DependencyObject current)
        {
            int childrenCount = VisualTreeHelper.GetChildrenCount(current);
            for (int i = 0; i < childrenCount; i++)
            {
                var child = VisualTreeHelper.GetChild(current, i);
                yield return child;
                foreach (var childOfChild in child.EnumerateVisualChildren())
                    yield return childOfChild;
            }
        }

        private static bool MatchOnName(this DependencyObject obj, string name)
        {
            return obj is FrameworkElement element && name.Equals(element.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
