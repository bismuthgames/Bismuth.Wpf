using System.Collections;
using System.Collections.Specialized;
using System.ComponentModel;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;

namespace Bismuth.Wpf.Controls
{
    public enum PropertyInspectorArrangeMode
    {
        Category,
        Name,
        Source
    }

    [TemplatePart(Name = "PART_PropertyEditorsHost", Type = typeof(ItemsControl))]
    public class PropertyInspector : ItemsControl
    {
        static PropertyInspector()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(PropertyInspector), new FrameworkPropertyMetadata(typeof(PropertyInspector)));
        }

        public DataTemplateSelector EditorTemplateSelector
        {
            get { return (DataTemplateSelector)GetValue(EditorTemplateSelectorProperty); }
            set { SetValue(EditorTemplateSelectorProperty, value); }
        }

        public static readonly DependencyProperty EditorTemplateSelectorProperty = DependencyProperty.Register(nameof(EditorTemplateSelector), typeof(DataTemplateSelector), typeof(PropertyInspector),
            new PropertyMetadata(new PropertyEditorTemplateSelector()));

        public PropertyInspectorArrangeMode ArrangeMode
        {
            get { return (PropertyInspectorArrangeMode)GetValue(ArrangeModeProperty); }
            set { SetValue(ArrangeModeProperty, value); }
        }

        public static readonly DependencyProperty ArrangeModeProperty = DependencyProperty.Register(nameof(ArrangeMode), typeof(PropertyInspectorArrangeMode), typeof(PropertyInspector),
            new PropertyMetadata(PropertyInspectorArrangeMode.Category, (d, e) => ((PropertyInspector)d).UpdateArrangeMode()));

        private readonly PropertyItemGenerator _propertyItemGenerator = new PropertyItemGenerator();
        private readonly ICollectionView _viewSource;

        public PropertyInspector()
        {
            _viewSource = CollectionViewSource.GetDefaultView(_propertyItemGenerator.Items);

            UpdateArrangeMode();
        }

        private void UpdateArrangeMode()
        {
            _viewSource.GroupDescriptions.Clear();
            _viewSource.SortDescriptions.Clear();

            switch (ArrangeMode)
            {
                case PropertyInspectorArrangeMode.Category:
                    _viewSource.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PropertyItemGroup.Category)));
                    _viewSource.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PropertyItemGroup.IsAdvanced)));
                    _viewSource.SortDescriptions.Add(new SortDescription(nameof(PropertyItemGroup.Category), ListSortDirection.Ascending));
                    _viewSource.SortDescriptions.Add(new SortDescription(nameof(PropertyItemGroup.IsAdvanced), ListSortDirection.Ascending));
                    _viewSource.SortDescriptions.Add(new SortDescription(nameof(PropertyItemGroup.Name), ListSortDirection.Ascending));
                    break;
                case PropertyInspectorArrangeMode.Name:
                    _viewSource.SortDescriptions.Add(new SortDescription(nameof(PropertyItemGroup.Name), ListSortDirection.Ascending));
                    break;
                case PropertyInspectorArrangeMode.Source:
                    _viewSource.GroupDescriptions.Add(new PropertyGroupDescription(nameof(PropertyItemGroup.SourceTypeNames)));
                    _viewSource.SortDescriptions.Add(new SortDescription(nameof(PropertyItemGroup.SourceTypeNames), ListSortDirection.Ascending));
                    _viewSource.SortDescriptions.Add(new SortDescription(nameof(PropertyItemGroup.Name), ListSortDirection.Ascending));
                    break;
            }
        }

        protected override void OnItemsSourceChanged(IEnumerable oldValue, IEnumerable newValue)
        {
            if (oldValue != null) _propertyItemGenerator.RemoveRange(oldValue);
            if (newValue != null) _propertyItemGenerator.AddRange(newValue);

            _viewSource.Refresh();
        }

        protected override void OnItemsChanged(NotifyCollectionChangedEventArgs e)
        {
            switch (e.Action)
            {
                case NotifyCollectionChangedAction.Add:
                case NotifyCollectionChangedAction.Remove:
                case NotifyCollectionChangedAction.Replace:
                case NotifyCollectionChangedAction.Move:

                    if (e.OldItems != null) _propertyItemGenerator.RemoveRange(e.OldItems);
                    if (e.NewItems != null) _propertyItemGenerator.AddRange(e.NewItems);

                    break;

                case NotifyCollectionChangedAction.Reset:

                    _propertyItemGenerator.Clear();
                    _propertyItemGenerator.AddRange(Items);

                    break;
            }

            _viewSource.Refresh();
        }

        public override void OnApplyTemplate()
        {
            var propertyEditorsHost = GetTemplateChild("PART_PropertyEditorsHost") as ItemsControl;
            if (propertyEditorsHost != null)
                propertyEditorsHost.ItemsSource = _viewSource;
        }
    }
}
