using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;

namespace Bismuth.Wpf.Controls
{
    public class DropTarget : ContentControl
    {
        static DropTarget()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DropTarget), new FrameworkPropertyMetadata(typeof(DropTarget)));
        }

        public bool IsCurrentTarget
        {
            get { return (bool)GetValue(IsCurrentTargetProperty); }
            set { SetValue(IsCurrentTargetProperty, value); }
        }

        public static readonly DependencyProperty IsCurrentTargetProperty = DependencyProperty.Register(nameof(IsCurrentTarget), typeof(bool), typeof(DropTarget));

        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register("DropCommand", typeof(ICommand), typeof(DropTarget));

        public int DropZIndex
        {
            get { return (int)GetValue(DropZIndexProperty); }
            set { SetValue(DropZIndexProperty, value); }
        }

        public static readonly DependencyProperty DropZIndexProperty = DependencyProperty.Register(nameof(DropZIndex), typeof(int), typeof(DropTarget));

        // NOTE: Overrides the HitTestCore so that a HitTestResult is always returned, even though the control is hidden.
        protected override HitTestResult HitTestCore(PointHitTestParameters hitTestParameters)
        {
            if (IsEnabled && IsVisible && DragDropScope.IsInDragDropState)
                return new PointHitTestResult(this, hitTestParameters.HitPoint);

            return null;
        }

        public virtual void DropItem(object item)
        {
            if (DropCommand != null && DropCommand.CanExecute(item))
            {
                DropCommand.Execute(item);
            }

            if (ItemDropped != null)
            {
                ItemDropped(this, new ItemDroppedEventArgs(item));
            }
        }

        public event EventHandler<ItemDroppedEventArgs> ItemDropped;
    }

    public class ItemDroppedEventArgs : EventArgs
    {
        public ItemDroppedEventArgs(object item)
        {
            Item = item;
        }

        public object Item { get; }
    }
}
