using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class GraphNode : ContentControl
    {
        static GraphNode()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphNode), new FrameworkPropertyMetadata(typeof(GraphNode)));
        }
    }
}
