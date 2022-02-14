using System.Diagnostics;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bismuth.Wpf.Controls
{
    public class GraphPanel : Panel
    {
        public Panel PPP;

        public static double GetX(DependencyObject obj)
        {
            return (double)obj.GetValue(XProperty);
        }

        public static void SetX(DependencyObject obj, double value)
        {
            obj.SetValue(XProperty, value);
        }

        public static readonly DependencyProperty XProperty =
            DependencyProperty.RegisterAttached("X", typeof(double), typeof(GraphPanel),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnXYChanged));

        public static double GetY(DependencyObject obj)
        {
            return (double)obj.GetValue(YProperty);
        }

        public static void SetY(DependencyObject obj, double value)
        {
            obj.SetValue(YProperty, value);
        }

        public static readonly DependencyProperty YProperty =
            DependencyProperty.RegisterAttached("Y", typeof(double), typeof(GraphPanel),
                new FrameworkPropertyMetadata(0.0,
                    FrameworkPropertyMetadataOptions.AffectsParentMeasure |
                    FrameworkPropertyMetadataOptions.AffectsParentArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender,
                    OnXYChanged));

        public static Point GetOrigin(DependencyObject obj)
        {
            return (Point)obj.GetValue(OriginProperty);
        }

        public static void SetOrigin(DependencyObject obj, Point value)
        {
            obj.SetValue(OriginProperty, value);
        }

        public static readonly DependencyProperty OriginProperty =
            DependencyProperty.RegisterAttached("Origin", typeof(Point), typeof(GraphPanel),
                new FrameworkPropertyMetadata(new Point(0.5, 0.5),
                    //FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));

        private static void OnXYChanged(DependencyObject d, DependencyPropertyChangedEventArgs e)
        {
            if (!(d is Visual visual)) return;

            if (VisualTreeHelper.GetParent(d) is GraphPanel panel)
                panel.PPP?.InvalidateVisual();
            else
                Debug.WriteLine($"ERROR: TimelinePanel for '{visual.GetType()}' not found.");
        }

        protected override Size MeasureOverride(Size availableSize)
        {
            availableSize = new Size(double.PositiveInfinity, double.PositiveInfinity);
            foreach (UIElement child in InternalChildren)
            {
                child?.Measure(availableSize);
            }
            return default;
        }

        protected override Size ArrangeOverride(Size finalSize)
        {
            foreach (UIElement child in InternalChildren)
            {
                if (child is null) continue;

                child.Arrange(GetRect(child));
            }
            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
        }

        internal static Rect GetRect(UIElement element)
        {
            var p = new Point(GetX(element), GetY(element));
            var o = GetOrigin(element);

            p.X -= element.DesiredSize.Width * o.X;
            p.Y -= element.DesiredSize.Height * o.Y;

            return new Rect(p, element.DesiredSize);
        }
    }
}
