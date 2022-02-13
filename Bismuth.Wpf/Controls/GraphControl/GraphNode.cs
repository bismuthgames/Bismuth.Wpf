using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;

namespace Bismuth.Wpf.Controls
{
    public class GraphNode : ContentControl
    {
        static GraphNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphNode), new FrameworkPropertyMetadata(typeof(GraphNode)));
        }

        public override void OnApplyTemplate()
        {
            var thumb = (Thumb)GetTemplateChild("Grip");
            thumb.DragDelta += Thumb_DragDelta;
        }

        private void Thumb_DragDelta(object sender, DragDeltaEventArgs e)
        {
            GraphPanel.SetX(this, GraphPanel.GetX(this) + e.HorizontalChange);
            GraphPanel.SetY(this, GraphPanel.GetY(this) + e.VerticalChange);
        }
    }
}
