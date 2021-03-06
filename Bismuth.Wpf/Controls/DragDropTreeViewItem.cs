﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using Bismuth.Wpf.Extensions;

namespace Bismuth.Wpf.Controls
{
    [TemplatePart(Name = "PART_DropTarget", Type = typeof(DropTarget))]
    [TemplatePart(Name = "PART_BeforeDropTarget", Type = typeof(DropTarget))]
    [TemplatePart(Name = "PART_AfterDropTarget", Type = typeof(DropTarget))]
    public class DragDropTreeViewItem : MultiSelectTreeViewItem
    {
        static DragDropTreeViewItem()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(DragDropTreeViewItem), new FrameworkPropertyMetadata(typeof(DragDropTreeViewItem)));
        }

        public Func<object, bool> AcceptFunction
        {
            get { return (Func<object, bool>)GetValue(AcceptFunctionProperty); }
            set { SetValue(AcceptFunctionProperty, value); }
        }

        public static readonly DependencyProperty AcceptFunctionProperty = DependencyProperty.Register(nameof(AcceptFunction), typeof(Func<object, bool>), typeof(DragDropTreeViewItem));

        public IEnumerable<Type> AcceptTypes
        {
            get { return (IEnumerable<Type>)GetValue(AcceptTypesProperty); }
            set { SetValue(AcceptTypesProperty, value); }
        }

        public static readonly DependencyProperty AcceptTypesProperty = DependencyProperty.Register(nameof(AcceptTypes), typeof(IEnumerable<Type>), typeof(DragDropTreeViewItem));

        public bool IsDraggable
        {
            get { return (bool)GetValue(IsDraggableProperty); }
            set { SetValue(IsDraggableProperty, value); }
        }

        public static readonly DependencyProperty IsDraggableProperty = DependencyProperty.Register(nameof(IsDraggable), typeof(bool), typeof(DragDropTreeViewItem), new PropertyMetadata(true));

        public ICommand DropCommand
        {
            get { return (ICommand)GetValue(DropCommandProperty); }
            set { SetValue(DropCommandProperty, value); }
        }

        public static readonly DependencyProperty DropCommandProperty = DependencyProperty.Register(nameof(DropCommand), typeof(ICommand), typeof(DragDropTreeViewItem));

        public ICommand DropBeforeCommand
        {
            get { return (ICommand)GetValue(DropBeforeCommandProperty); }
            set { SetValue(DropBeforeCommandProperty, value); }
        }

        public static readonly DependencyProperty DropBeforeCommandProperty = DependencyProperty.Register(nameof(DropBeforeCommand), typeof(ICommand), typeof(DragDropTreeViewItem));

        public ICommand DropAfterCommand
        {
            get { return (ICommand)GetValue(DropAfterCommandProperty); }
            set { SetValue(DropAfterCommandProperty, value); }
        }

        public static readonly DependencyProperty DropAfterCommandProperty = DependencyProperty.Register(nameof(DropAfterCommand), typeof(ICommand), typeof(DragDropTreeViewItem));

        public override void OnApplyTemplate()
        {
            var dropTarget = GetTemplateChild("PART_DropTarget") as DropTarget;
            var beforeDropTarget = GetTemplateChild("PART_BeforeDropTarget") as DropTarget;
            var afterDropTarget = GetTemplateChild("PART_AfterDropTarget") as DropTarget;

            if (dropTarget != null)
            {
                dropTarget.IsEnabled = DropCommand != null;
            }

            if (beforeDropTarget != null)
            {
                beforeDropTarget.IsEnabled = DropBeforeCommand != null;
                beforeDropTarget.DropZIndex = TreeDepth;
            }

            if (afterDropTarget != null)
            {
                afterDropTarget.IsEnabled = DropAfterCommand != null;
                afterDropTarget.DropZIndex = TreeDepth;
            }
        }

        protected override void OnMouseLeftButtonDown(MouseButtonEventArgs e)
        {
            base.OnMouseLeftButtonDown(e);

            if (IsDraggable)
                this.FindVisualParent<DragDropScope>()?.BeginDrag(ParentTreeView.SelectedItems.OfType<object>().ToArray());
        }

        protected override DependencyObject GetContainerForItemOverride()
        {
            return new DragDropTreeViewItem();
        }

        protected override bool IsItemItsOwnContainerOverride(object item)
        {
            return item is DragDropTreeViewItem;
        }
    }
}
