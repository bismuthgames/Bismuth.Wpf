using System.Windows.Controls;
using System.Windows.Input;
using Bismuth.Wpf.Extensions;

namespace Bismuth.Wpf.Controls
{
    public class DragSource : Decorator
    {
        protected override void OnPreviewMouseDown(MouseButtonEventArgs e)
        {
            base.OnPreviewMouseDown(e);
            this.FindVisualParent<DragDropScope>()?.BeginDrag(DataContext);
        }
    }
}
