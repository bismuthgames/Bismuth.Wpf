using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;

namespace Bismuth.Wpf.Extensions
{
    public static class LogicalTree
    {
        public static T FindLogicalParent<T>(this DependencyObject current) where T : DependencyObject
        {
            return current
                .EnumerateLogicalParents()
                .OfType<T>()
                .FirstOrDefault();
        }

        public static T FindLogicalParentByName<T>(this DependencyObject current, string name) where T : DependencyObject
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return current
                .EnumerateLogicalParents()
                .OfType<T>()
                .FirstOrDefault(parent => parent.MatchOnName(name));
        }

        public static T FindLogicalChildren<T>(this DependencyObject current) where T : DependencyObject
        {
            return current
                .EnumerateLogicalChildren()
                .OfType<T>()
                .FirstOrDefault();
        }

        public static T FindLogicalChildByName<T>(this DependencyObject current, string name) where T : DependencyObject
        {
            if (name == null) throw new ArgumentNullException(nameof(name));

            return current
                .EnumerateLogicalParents()
                .OfType<T>()
                .FirstOrDefault(child => child.MatchOnName(name));
        }

        public static IEnumerable<DependencyObject> EnumerateLogicalParents(this DependencyObject current)
        {
            var parent = LogicalTreeHelper.GetParent(current);
            while (parent != null)
            {
                yield return parent;
                parent = LogicalTreeHelper.GetParent(parent);
            }
        }

        public static IEnumerable<DependencyObject> EnumerateLogicalChildren(this DependencyObject current)
        {
            foreach (DependencyObject child in LogicalTreeHelper.GetChildren(current))
            {
                yield return child;
                foreach (var childOfChild in child.EnumerateLogicalChildren())
                    yield return childOfChild;
            }
        }

        private static bool MatchOnName(this DependencyObject obj, string name)
        {
            return obj is FrameworkElement element && name.Equals(element.Name, StringComparison.OrdinalIgnoreCase);
        }
    }
}
