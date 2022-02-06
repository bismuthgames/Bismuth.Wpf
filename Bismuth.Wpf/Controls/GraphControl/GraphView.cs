using System;
using System.Collections;
using System.Collections.Specialized;
using System.Linq;
using System.Reflection;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Bismuth.Wpf.Controls
{
    public class GraphView : ItemsControl
    {
        static GraphView()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(GraphView), new FrameworkPropertyMetadata(typeof(GraphView)));
        }

        private readonly CompositeCollection _cc = new CompositeCollection();
        private readonly CollectionContainer _cc1 = new CollectionContainer();
        private readonly CollectionContainer _cc2 = new CollectionContainer();

        public GraphView()
        {
            _cc.Add(_cc1);
            _cc.Add(_cc2);

            _cc1.Collection = Items;
            _cc2.Collection = Connections;

            _connectiosHost.ItemsSourceChanged += (s, e) => _cc2.Collection = e ?? _connectiosHost.Items;
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

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            _cc1.Collection = newValue ?? Items;
        }

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

            var merge = (ItemsControl)GetTemplateChild("Merge");
            merge.ItemsSource = _cc;
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
            public GraphConnectionHost()
            {
                Template = null;
            }

            protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
            {
                ItemsSourceChanged(this, newValue);
            }

            public event EventHandler<IEnumerable> ItemsSourceChanged;

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
