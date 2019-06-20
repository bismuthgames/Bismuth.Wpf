using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace Bismuth.Wpf.Controls
{
    public class MultiSelectTreeViewItem : TreeViewItem
    {
        private static readonly MethodInfo _selectMethod =
            typeof(TreeViewItem).GetMethod("Select", BindingFlags.Instance | BindingFlags.NonPublic);

        internal void Select(bool selected)
        {
            _selectMethod.Invoke(this, new object[] { selected });
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new MultiSelectTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is MultiSelectTreeViewItem;
        }

        protected override void OnSelected(RoutedEventArgs e)
        {
            ParentTreeView.Add(this);
            base.OnSelected(e);
        }

        protected override void OnUnselected(RoutedEventArgs e)
        {
            ParentTreeView.Remove(this);
            base.OnUnselected(e);
        }

        protected override void OnCollapsed(RoutedEventArgs e)
        {
            UnselectRecursive();
            base.OnCollapsed(e);
        }

        internal void UnselectRecursive()
        {
            for (int i = 0; i < ItemContainerGenerator.Items.Count; i++)
            {
                var treeViewItem = ItemContainerGenerator.ContainerFromIndex(i) as MultiSelectTreeViewItem;
                if (treeViewItem != null)
                {
                    treeViewItem.IsSelected = false;
                    treeViewItem.UnselectRecursive();
                }
            }
        }

        protected override void OnKeyDown(KeyEventArgs e)
        {
            if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                e.Handled = true;
            }

            base.OnKeyDown(e);
        }

        protected override void OnMouseLeftButtonUp(MouseButtonEventArgs e)
        {
            if (!e.Handled &&
                IsSelected &&
                !Keyboard.IsKeyDown(Key.LeftCtrl) &&
                !Keyboard.IsKeyDown(Key.RightCtrl) &&
                !Keyboard.IsKeyDown(Key.LeftShift) &&
                !Keyboard.IsKeyDown(Key.RightShift))
            {
                ParentTreeView.Clear();
                Select(true);

                e.Handled = true;
            }

            base.OnMouseLeftButtonUp(e);
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            var parentTreeView = ParentTreeView;

            if (Keyboard.IsKeyDown(Key.LeftCtrl) || Keyboard.IsKeyDown(Key.RightCtrl))
            {
                parentTreeView.MultiSelect(this);

                e.Handled = true;
            }
            else if (Keyboard.IsKeyDown(Key.LeftShift) || Keyboard.IsKeyDown(Key.RightShift))
            {
                parentTreeView.MultiSelectRange(this);

                e.Handled = true;
            }
            else if (IsSelected)
            {
                // NOTE: By setting e.Handled to 'true',
                // we can no longer expand/collapse TreeViewItems by double clicking.
                e.Handled = true;
            }
            else
            {
                parentTreeView.Clear();
            }

            base.OnMouseLeftButtonDown(e);
        }

        internal ItemsControl ParentItemsControl
        {
            get { return ItemsControlFromItemContainer(this); }
        }

        private MultiSelectTreeView ParentTreeView
        {
            get
            {
                var itemsControl = ParentItemsControl;
                while (itemsControl != null)
                {
                    if (itemsControl is MultiSelectTreeView treeView) return treeView;

                    itemsControl = ItemsControlFromItemContainer(itemsControl);
                }

                return null;
            }
        }
    }
}
