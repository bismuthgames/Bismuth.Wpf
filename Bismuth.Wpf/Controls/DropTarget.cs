using System;
using System.Collections.Generic;
using System.Linq;
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

        public event ItemDroppedEventHandler ItemDropped
        {
            add { AddHandler(ItemDroppedEvent, value); }
            remove { RemoveHandler(ItemDroppedEvent, value); }
        }

        public static readonly RoutedEvent ItemDroppedEvent = EventManager.RegisterRoutedEvent(nameof(ItemDropped), RoutingStrategy.Bubble, typeof(ItemDroppedEventHandler), typeof(DropTarget));

        public bool IsCurrentTarget
        {
            get { return (bool)GetValue(IsCurrentTargetProperty); }
            internal set { SetValue(_isCurrentTargetPropertyKey, value); }
        }

        private static readonly DependencyPropertyKey _isCurrentTargetPropertyKey = DependencyProperty.RegisterReadOnly(nameof(IsCurrentTarget), typeof(bool), typeof(DropTarget), new PropertyMetadata(false));
        public static readonly DependencyProperty IsCurrentTargetProperty = _isCurrentTargetPropertyKey.DependencyProperty;

        public Func<object, bool> AcceptFunction
        {
            get { return (Func<object, bool>)GetValue(AcceptFunctionProperty); }
            set { SetValue(AcceptFunctionProperty, value); }
        }

        public static readonly DependencyProperty AcceptFunctionProperty = DependencyProperty.Register(nameof(AcceptFunction), typeof(Func<object, bool>), typeof(DropTarget));

        public IEnumerable<Type> AcceptTypes
        {
            get { return (IEnumerable<Type>)GetValue(AcceptTypesProperty); }
            set { SetValue(AcceptTypesProperty, value); }
        }

        public static readonly DependencyProperty AcceptTypesProperty = DependencyProperty.Register(nameof(AcceptTypes), typeof(IEnumerable<Type>), typeof(DropTarget));

        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register(nameof(DropCommand), typeof(ICommand), typeof(DropTarget));

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

        public virtual bool AcceptItem(object item)
        {
            if (AcceptFunction != null) return AcceptFunction(item);
            if (AcceptTypes != null) return AcceptTypes.Any(t => t.IsInstanceOfType(item));
            return false;
        }

        public virtual void DropItem(object item)
        {
            if (DropCommand != null &&
                DropCommand.CanExecute(item))
                DropCommand.Execute(item);

            RaiseEvent(new ItemDroppedEventArgs(ItemDroppedEvent, this, item));
        }
    }

    public delegate void ItemDroppedEventHandler(object sender, ItemDroppedEventArgs e);

    public class ItemDroppedEventArgs : RoutedEventArgs
    {
        public ItemDroppedEventArgs(RoutedEvent routedEvent, object source, object item) : base(routedEvent, source)
        {
            Item = item;
        }

        public object Item { get; }
    }
}
