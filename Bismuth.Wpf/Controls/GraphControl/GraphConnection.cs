using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class GraphConnection : ContentControl
    {
        static GraphConnection()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphConnection), new FrameworkPropertyMetadata(typeof(GraphConnection)));
        }
    }
}
