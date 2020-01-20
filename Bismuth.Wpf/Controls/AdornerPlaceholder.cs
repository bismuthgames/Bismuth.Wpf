using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Media;

namespace Bismuth.Wpf.Controls
{
    public class AdornerPlaceholder : Decorator
    {
        private class UIElementAdorner : Adorner
        {
            private readonly UIElement _element;
            private readonly FrameworkElement _adornedElement;

            public UIElementAdorner(UIElement element, FrameworkElement adornedElement) : base(adornedElement)
            {
                _element = element ?? throw new ArgumentNullException(nameof(element));
                _adornedElement = adornedElement ?? throw new ArgumentNullException(nameof(adornedElement));
            }

            protected override int VisualChildrenCount => 1;

            protected override Visual GetVisualChild(int index)
            {
                if (index != 0) throw new ArgumentOutOfRangeException(nameof(index));
                return _element;
            }

            protected override Size MeasureOverride(Size constraint)
            {
                _element.Measure(constraint);
                return _element.DesiredSize;
            }

            protected override Size ArrangeOverride(Size finalSize)
            {
                _element.Arrange(new Rect(0, 0, _adornedElement.ActualWidth, _adornedElement.ActualHeight));
                return finalSize;
            }
        }

        private AdornerLayer _adornerLayer;
        private UIElementAdorner _adorner;

        public AdornerPlaceholder()
        {
            IsVisibleChanged += AdornerPlaceholder_IsVisibleChanged;
        }

        private void AdornerPlaceholder_IsVisibleChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (IsVisible)
                AddAdorner();
            else
                RemoveAdorner();
        }

        protected override void OnVisualChildrenChanged(DependencyObject visualAdded, DependencyObject visualRemoved)
        {
            RemoveAdorner();
            if (IsVisible)
                AddAdorner();
        }

        private void AddAdorner()
        {
            if (Child == null) return;

            _adornerLayer = AdornerLayer.GetAdornerLayer(this);
            if (_adornerLayer == null) return;

            _adorner = new UIElementAdorner(Child, this);
            _adornerLayer.Add(_adorner);
        }

        private void RemoveAdorner()
        {
            if (_adornerLayer == null || _adorner == null) return;

            _adornerLayer.Remove(_adorner);
            _adorner = null;
        }
    }
}
