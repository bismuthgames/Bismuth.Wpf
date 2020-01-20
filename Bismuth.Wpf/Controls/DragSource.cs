using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Bismuth.Wpf.Extensions;

namespace Bismuth.Wpf.Controls
{
    public class DragSource : Decorator
    {
        public object DragData
        {
            get { return GetValue(DragDataProperty); }
            set { SetValue(DragDataProperty, value); }
        }

        public static readonly DependencyProperty DragDataProperty = DependencyProperty.Register(nameof(DragData), typeof(object), typeof(DragSource));

        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.FindVisualParent<DragDropScope>()?.BeginDrag(DragData ?? DataContext);
        }
    }
}
