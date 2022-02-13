using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace Bismuth.Wpf.Controls
{
    public class ConnectionsPanel : Panel
    {
        public Panel Panel { get; set; }

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
            int c = 0;
            foreach (UIElement child in InternalChildren)
            {
                if (child is null) continue;

                var conn = GetConnection(child, c++);

                var p = (Point)conn.Center;
                p.X -= child.DesiredSize.Width * 0.5;
                p.Y -= child.DesiredSize.Height * 0.5;

                child.Arrange(new Rect(p, child.DesiredSize));
            }
            return finalSize;
        }

        protected override void OnRender(DrawingContext dc)
        {
            base.OnRender(dc);

            int c = 0;
            foreach (UIElement child in InternalChildren)
            {
                if (child is null) continue;

                var conn = GetConnection(child, c++);

                var pen = new Pen(Brushes.Red, 4);

                //dc.DrawLine(pen, (Point)conn.A, (Point)conn.B);

                var d = (conn.B.X - conn.A.X) * 0.5;

                var bs = new BezierSegment(
                    (Point)conn.A + new Vector(d, 0),
                    (Point)conn.B - new Vector(d, 0),
                    (Point)conn.B,
                    true);

                var pf = new PathFigure((Point)conn.A, new PathSegment[] { bs }, false);

                var pg = new PathGeometry();
                pg.Figures.Add(pf);

                dc.DrawGeometry(null, pen, pg);
            }
        }

        private Connection GetConnection(UIElement element, int i)
        {
            var a = Panel.Children[0 + i];
            var b = Panel.Children[1 + i];

            return new Connection
            {
                A = new Vector(GraphPanel.GetX(a), GraphPanel.GetY(a)),
                B = new Vector(GraphPanel.GetX(b), GraphPanel.GetY(b))
            };
        }

        private struct Connection
        {
            public Vector A;
            public Vector B;
            public Vector Center => (A + B) * 0.5;
        }
    }
}
