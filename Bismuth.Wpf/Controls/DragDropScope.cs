using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Threading;
using Bismuth.Wpf.Helpers;

namespace Bismuth.Wpf.Controls
{
    public enum DragDropOperation
    {
        None,
        Add,
        Move,
        Invalid
    }

    [TemplatePart(Name = "PART_DragPopup", Type = typeof(Popup))]
    public class DragDropScope : ContentControl
    {
        static DragDropScope()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragDropScope), new FrameworkPropertyMetadata(typeof(DragDropScope)));
        }

        public static bool IsInDragDropState { get; private set; }

        public object DragContent
        {
            get { return GetValue(DragContentProperty); }
            set { SetValue(DragContentProperty, value); }
        }

        public static readonly DependencyProperty DragContentProperty = DependencyProperty.Register(nameof(DragContent), typeof(object), typeof(DragDropScope));

        public DataTemplate DragContentTemplate
        {
            get { return (DataTemplate)GetValue(DragContentTemplateProperty); }
            set { SetValue(DragContentTemplateProperty, value); }
        }

        public static readonly DependencyProperty DragContentTemplateProperty = DependencyProperty.Register(nameof(DragContentTemplate), typeof(DataTemplate), typeof(DragDropScope));

        public DragDropOperation Operation
        {
            get { return (DragDropOperation)GetValue(OperationProperty); }
            set { SetValue(OperationProperty, value); }
        }

        public static readonly DependencyProperty OperationProperty = DependencyProperty.Register(nameof(Operation), typeof(DragDropOperation), typeof(DragDropScope), new PropertyMetadata(DragDropOperation.None));

        private readonly DispatcherTimer _timer = new DispatcherTimer();

        private object _item;
        private object _initialItem;
        private Point _initialMousePosition;

        private DropTarget _currentDropTarget;
        private DropTarget _hitDropTarget;

        private Popup _dragPopup;

        public DragDropScope()
        {
            _timer.Interval = new TimeSpan(1000);
            _timer.Tick += (s, e) => Tick();
        }

        private void Tick()
        {
            Point p = MouseHelper.GetPosition();

            _dragPopup.HorizontalOffset = p.X + 10;
            _dragPopup.VerticalOffset = p.Y + 20;
        }

        public override void OnApplyTemplate()
        {
            _dragPopup = GetTemplateChild("PART_DragPopup") as Popup;
        }

        public void BeginDrag(object item)
        {
            _initialItem = item;
            _initialMousePosition = Mouse.GetPosition(this);
        }

        private bool IsDragDistanceBigEnough(Point initialPosition, Point currentPosition)
        {
            Vector diff = currentPosition - initialPosition;

            return
                Math.Abs(diff.X) > SystemParameters.MinimumHorizontalDragDistance ||
                Math.Abs(diff.Y) > SystemParameters.MinimumVerticalDragDistance;
        }

        protected override void OnMouseMove(MouseEventArgs e)
        {
            if (_initialItem != null)
            {
                if (e.LeftButton == MouseButtonState.Pressed)
                {
                    Point mousePosition = e.GetPosition(this);

                    if (IsDragDistanceBigEnough(_initialMousePosition, mousePosition))
                    {
                        CaptureMouse();
                        IsInDragDropState = true;

                        DragContent = _item = _initialItem;

                        ShowPopup();

                        _hitDropTarget = null;
                        VisualTreeHelper.HitTest(this, null, HitTestResultCallback, new PointHitTestParameters(mousePosition));

                        UpdateCurrentDropTarget(_hitDropTarget);

                        Operation = _hitDropTarget != null ? DragDropOperation.Move : DragDropOperation.Invalid;

                        e.Handled = true;
                    }
                }
                else
                {
                    Clear();
                }
            }

            base.OnMouseMove(e);
        }

        protected override void OnMouseUp(MouseButtonEventArgs e)
        {
            if (_currentDropTarget != null)
                _currentDropTarget.DropItem(_item);

            Clear();

            base.OnMouseUp(e);
        }

        private HitTestResultBehavior HitTestResultCallback(HitTestResult result)
        {
            if (result.VisualHit is DropTarget dropTarget)
            {
                if (_hitDropTarget == null || dropTarget.DropZIndex > _hitDropTarget.DropZIndex)
                    _hitDropTarget = dropTarget;
            }

            return HitTestResultBehavior.Continue;
        }

        private void UpdateCurrentDropTarget(DropTarget dropTarget)
        {
            if (_currentDropTarget != dropTarget)
            {
                if (_currentDropTarget != null) _currentDropTarget.IsCurrentTarget = false;
                _currentDropTarget = dropTarget;
                if (_currentDropTarget != null) _currentDropTarget.IsCurrentTarget = true;
            }
        }

        private void Clear()
        {
            HidePopup();

            DragContent = _item = _initialItem = null;

            UpdateCurrentDropTarget(null);

            ReleaseMouseCapture();
            IsInDragDropState = false;
        }

        private void ShowPopup()
        {
            Tick();

            _dragPopup.IsOpen = true;
            _timer.Start();
        }

        private void HidePopup()
        {
            _dragPopup.IsOpen = false;
            _timer.Stop();
        }
    }
}
