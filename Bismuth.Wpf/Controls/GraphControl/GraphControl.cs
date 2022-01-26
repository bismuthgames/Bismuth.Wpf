using System.Collections;
using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class GraphControl : ItemsControl
    {
        static GraphControl()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphControl), new FrameworkPropertyMetadata(typeof(GraphControl)));
        }

        public IEnumerable ConnectionsSource
        {
            get { return (IEnumerable)GetValue(ConnectionsSourceProperty); }
            set { SetValue(ConnectionsSourceProperty, value); }
        }

        public static readonly DependencyProperty ConnectionsSourceProperty =
            DependencyProperty.Register(nameof(ConnectionsSource), typeof(IEnumerable), typeof(GraphControl));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GraphNode();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GraphNode;
        }
    }
}
