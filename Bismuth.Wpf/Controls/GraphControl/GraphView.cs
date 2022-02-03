using System.Collections;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;

namespace Bismuth.Wpf.Controls
{
    public class GraphView : ItemsControl
    {
        static GraphView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphView), new FrameworkPropertyMetadata(typeof(GraphView)));
        }

        public IEnumerable ConnectionsSource
        {
            get { return (IEnumerable)GetValue(ConnectionsSourceProperty); }
            set { SetValue(ConnectionsSourceProperty, value); }
        }

        public static readonly DependencyProperty ConnectionsSourceProperty =
            DependencyProperty.Register(nameof(ConnectionsSource), typeof(IEnumerable), typeof(GraphView), new PropertyMetadata(
                (d, e) => ((GraphView)d)._connectiosHost.ItemsSource = (IEnumerable)e.NewValue));

        public DataTemplate ConnectionTemplate
        {
            get { return (DataTemplate)GetValue(ConnectionTemplateProperty); }
            set { SetValue(ConnectionTemplateProperty, value); }
        }

        public static readonly DependencyProperty ConnectionTemplateProperty =
            DependencyProperty.Register(nameof(ConnectionTemplate), typeof(DataTemplate), typeof(GraphView), new PropertyMetadata(
                (d, e) => ((GraphView)d)._connectiosHost.ItemTemplate = (DataTemplate)e.NewValue));

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new GraphNode();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is GraphNode;
        }

        private GraphConnectionHost _connectiosHost = new GraphConnectionHost();

        public override void OnApplyTemplate()
        {
            var panel = (Panel)GetTemplateChild("XYZ");
            panel.Children.Add(_connectiosHost);
        }

        private void ApplyHost()
        {
            PropertyInfo p = typeof(ItemsControl).GetProperty("ItemsHost", BindingFlags.Instance | BindingFlags.NonPublic);

            var x = p.GetValue(this, null);
            p.SetValue(_connectiosHost, x);
        }

        public ItemCollection Connections => _connectiosHost.Items;

        private class GraphConnectionHost : ItemsControl
        {
            protected override DependencyObject GetContainerForItemOverride()
            {
                return new GraphConnection();
            }

            protected override bool IsItemItsOwnContainerOverride(object item)
            {
                return item is GraphConnection;
            }
        }
    }
}
