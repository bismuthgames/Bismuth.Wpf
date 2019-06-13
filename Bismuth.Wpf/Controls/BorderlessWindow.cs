﻿using System;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Controls.Primitives;
using System.Windows.Input;
using System.Windows.Interop;
using Bismuth.Wpf.Helpers;

namespace Bismuth.Wpf.Controls
{
    [TemplatePart(Name = "PART_TitleBar", Type = typeof(Panel))]
    [TemplatePart(Name = "PART_CloseButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_RestoreButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_MinimizeButton", Type = typeof(Button))]
    [TemplatePart(Name = "PART_TopLeftThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_TopThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_TopRightThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_RightThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_BottomRightThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_BottomThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_BottomLeftThumb", Type = typeof(Thumb))]
    [TemplatePart(Name = "PART_LeftThumb", Type = typeof(Thumb))]
    public class BorderlessWindow : Window
    {
        static BorderlessWindow()
        {
            DefaultStyleKeyProperty.OverrideMetadata(typeof(BorderlessWindow), new FrameworkPropertyMetadata(typeof(BorderlessWindow)));
        }

        protected override void OnSourceInitialized(EventArgs e)
        {
            HwndSource hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            hwndSource.AddHook(new HwndSourceHook(new BorderlessWindowCore().WindowProc));

            IntPtr handle = hwndSource.Handle;

            WindowHelper.RemoveButtons(handle);
            WindowHelper.EnableShadows(handle);
        }

        public override void OnApplyTemplate()
        {
            var titleBar = GetTemplateChild("PART_TitleBar") as Panel;
            if (titleBar != null)
                titleBar.MouseDown += TitleBar_MouseDown;

            var closeButton = GetTemplateChild("PART_CloseButton") as Button;
            if (closeButton != null)
                closeButton.Click += (s, e) => Close();

            var restoreButton = GetTemplateChild("PART_RestoreButton") as Button;
            if (restoreButton != null)
                restoreButton.Click += (s, e) => Restore();

            var minimizeButton = GetTemplateChild("PART_MinimizeButton") as Button;
            if (minimizeButton != null)
                minimizeButton.Click += (s, e) => Minimize();

            SubscribeToThumb("PART_TopLeftThumb", ResizeDirection.TopLeft);
            SubscribeToThumb("PART_TopThumb", ResizeDirection.Top);
            SubscribeToThumb("PART_TopRightThumb", ResizeDirection.TopRight);
            SubscribeToThumb("PART_RightThumb", ResizeDirection.Right);
            SubscribeToThumb("PART_BottomRightThumb", ResizeDirection.BottomRight);
            SubscribeToThumb("PART_BottomThumb", ResizeDirection.Bottom);
            SubscribeToThumb("PART_BottomLeftThumb", ResizeDirection.BottomLeft);
            SubscribeToThumb("PART_LeftThumb", ResizeDirection.Left);
        }

        private void Minimize()
        {
            WindowState = WindowState.Minimized;
        }

        private void Restore()
        {
            WindowState = WindowState == WindowState.Normal ? WindowState.Maximized : WindowState.Normal;
        }

        private void TitleBar_MouseDown(object sender, MouseButtonEventArgs e)
        {
            if (e.ChangedButton == MouseButton.Left)
            {
                DragMove();
                //WindowState = WindowState.Normal;
            }
        }

        private void SubscribeToThumb(string childName, ResizeDirection direction)
        {
            var thumb = GetTemplateChild(childName) as Thumb;
            if (thumb != null)
            {
                thumb.PreviewMouseDown += (s, e) => { Resize(direction); };
                //thumb.MouseMove += (s, e) => thumb.CaptureMouse();
                //thumb.PreviewMouseUp += (s, e) => thumb.ReleaseMouseCapture();
            }
        }

        private void Resize(ResizeDirection direction)
        {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(this);
            if (hwndSource != null)
            {
                WindowHelper.Resize(hwndSource.Handle, direction);
            }
        }
    }
}
