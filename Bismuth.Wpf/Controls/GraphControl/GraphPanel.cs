using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bismuth.Wpf.Controls
{
    public class GraphPanel : Panel
    {
        public static Point GetPosition(DependencyObject obj)
        {
            return (Point)obj.GetValue(PositionProperty);
        }

        public static void SetPosition(DependencyObject obj, Point value)
        {
            obj.SetValue(PositionProperty, value);
        }

        public static readonly DependencyProperty PositionProperty =
            DependencyProperty.RegisterAttached("Position", typeof(Point), typeof(GraphPanel),
                new FrameworkPropertyMetadata(new Point(),
                    //FrameworkPropertyMetadataOptions.AffectsMeasure |
                    FrameworkPropertyMetadataOptions.AffectsArrange |
                    FrameworkPropertyMetadataOptions.AffectsRender));

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

                var p = GetPosition(child);
                var o = GetOrigin(child);

                p.X -= child.DesiredSize.Width * o.X;
                p.Y -= child.DesiredSize.Height * o.Y;

                child.Arrange(new Rect(p, child.DesiredSize));
            }
            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);
        }
    }
}
