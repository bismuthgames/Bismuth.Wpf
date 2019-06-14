using System;
using System.Windows;
using System.Windows.Interop;
using Bismuth.Wpf.Native;

namespace Bismuth.Wpf.Helpers
{
    public enum ResizeDirection
    {
        Left = 1,
        Right = 2,
        Top = 3,
        TopLeft = 4,
        TopRight = 5,
        Bottom = 6,
        BottomLeft = 7,
        BottomRight = 8,
    }

    public static class WindowHelper
    {
        public static void Resize(IntPtr handle, ResizeDirection direction)
        {
            User32.SendMessage(handle, User32.WM_SYSCOMMAND, (IntPtr)(61440 + direction), IntPtr.Zero);
        }

        public static void RemoveButtons(IntPtr handle)
        {
            int value = User32.GetWindowLong(handle, User32.GWL_STYLE);

            User32.SetWindowLong(handle, User32.GWL_STYLE, value & ~User32.WS_SYSMENU);
        }

        public static void EnableShadows(IntPtr handle)
        {
            MARGINS margins = new MARGINS
            {
                bottomHeight = 1,
                leftWidth = 1,
                rightWidth = 1,
                topHeight = 1
            };

            DwmApi.DwmExtendFrameIntoClientArea(handle, ref margins);
        }

        public static Point GetWindowPosition(Window window)
        {
            var hwndSource = (HwndSource)PresentationSource.FromVisual(window);
            var windowInfo = User32.GetWindowInfo(hwndSource.Handle);
            return new Point(windowInfo.client.left, windowInfo.client.top);
        }
    }
}
