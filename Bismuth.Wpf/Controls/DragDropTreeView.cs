using System.Windows;

namespace Bismuth.Wpf.Controls
{
    public class DragDropTreeView : MultiSelectTreeView
    {
        static DragDropTreeView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragDropTreeView), new FrameworkPropertyMetadata(typeof(DragDropTreeView)));
        }

        public DataTemplate ItemDragTemplate
        {
            get { return (DataTemplate)GetValue(ItemDragTemplateProperty); }
            set { SetValue(ItemDragTemplateProperty, value); }
        }

        public static readonly DependencyProperty ItemDragTemplateProperty = DependencyProperty.Register(nameof(ItemDragTemplate), typeof(DataTemplate), typeof(DragDropTreeView));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DragDropTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DragDropTreeViewItem;
        }
    }
}
