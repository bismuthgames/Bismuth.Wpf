using System.Collections;
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
            panel.Children.Insert(1, _connectiosHost);

            var graphPanal = (GraphPanel)GetTemplateChild("GraphPanel");
            _connectiosHost.Other = graphPanal;
        }

        public ItemCollection Connections => _connectiosHost.Items;
    }

    public class GraphConnectionHost : ItemsControl
    {
        public GraphConnectionHost()
        {
            var factory = new FrameworkElementFactory(typeof(ConnectionsPanel));
            factory.SetValue(Panel.IsItemsHostProperty, true);
            factory.Name = "PPP";
            Template = new ControlTemplate { VisualTree = factory };
            //Template = null;
            //AddVisualChild(new StackPanel { IsItemsHost = true });
        }

        public GraphPanel Other;

        public override void OnApplyTemplate()
        {
            var p = (ConnectionsPanel)GetTemplateChild("PPP");
            p.Panel = Other;
            Other.PPP = p;
        }

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
