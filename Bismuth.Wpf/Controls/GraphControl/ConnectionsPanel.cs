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
            var a = Node.Create(Panel.Children[0 + i]);
            var b = Node.Create(Panel.Children[1 + i]);

            var x1 = new Connection(a.L, b.R, new Vector(b.R.X - a.L.X, 0));
            var x2 = new Connection(a.R, b.L, new Vector(b.R.X - a.L.X, 0));


            return new Connection
            {
                A = Node.Create(a),
                B = Node.Create(b)
            };
        }

        private struct Connection
        {
            public Connection(Vector a, Vector b, Vector dist)
            {
                A = a;
                B = b;
                Dist = dist;
            }

            public Vector A;
            public Vector B;
            public Vector Center => (A + B) * 0.5;
            public Vector Dist;
        }

        private struct Node
        {
            public static Node Create(UIElement element)
            {
                var rect = GraphPanel.GetRect(element);
                return new Node
                {
                    Element = element,
                    Rect = rect,
                    Center = ((Vector)rect.TopLeft + (Vector)rect.BottomRight) * 0.5
                };
            }

            public UIElement Element;
            public Rect Rect;
            public Vector Center;

            public Vector L => new Vector(Rect.Left, Center.Y);
            public Vector T => new Vector(Center.X, Rect.Top);
            public Vector R => new Vector(Rect.Right, Center.Y);
            public Vector B => new Vector(Center.X, Rect.Bottom);
        }
    }
}
