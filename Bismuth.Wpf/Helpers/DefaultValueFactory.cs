using System;
using System.Reflection;
using System.Windows;

namespace Bismuth.Wpf.Helpers
{
    public static class DefaultValueFactory
    {
        public static object Create(Freezable defaultValue)
        {
            return typeof(DependencyProperty)
                .Assembly
                .GetType("MS.Internal.FreezableDefaultValueFactory")
                .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new[] { typeof(Freezable) }, null)
                .Invoke(new[] { defaultValue });
        }

        public static object CreateObservableCollection<T>()
        {
            return typeof(DependencyProperty)
                .Assembly
                .GetType("MS.Internal.ObservableCollectionDefaultValueFactory`1")
                .MakeGenericType(typeof(T))
                .GetConstructor(BindingFlags.Instance | BindingFlags.NonPublic, null, new Type[0], null)
                .Invoke(null);
        }
    }
}
