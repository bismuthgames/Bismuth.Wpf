using System;
using System.Runtime.InteropServices;
using Bismuth.Wpf.Native;

namespace Bismuth.Wpf.Controls
{
    internal class BorderlessWindowCore
    {
        public IntPtr WindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            switch (msg)
            {
                case User32.WM_NCCALCSIZE:
                    return WmNcCalcSize(hWnd, wParam, lParam, ref handled);
                case User32.WM_NCACTIVATE: // Prevent rendering of borders.
                    return WmNcActivate(hWnd, wParam, lParam, ref handled);
                default:
                    return IntPtr.Zero;
            }
        }

        private IntPtr WmNcActivate(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            handled = true;
            return User32.DefWindowProc(hWnd, User32.WM_NCACTIVATE, wParam, User32.HRGN_NONE);
        }

        private IntPtr WmNcCalcSize(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (User32.GetWindowPlacement(hWnd).showCmd == User32.SW_MAXIMIZE)
            {
                RECT rect1 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                User32.DefWindowProc(hWnd, User32.WM_NCCALCSIZE, wParam, lParam);
                RECT rect2 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                rect2.top = rect1.top + User32.GetWindowInfo(hWnd).cyWindowBorders;
                Marshal.StructureToPtr(rect2, lParam, true);
            }

            handled = true;
            return IntPtr.Zero;
        }

        /* Test of different size calculations.
        private IntPtr WmNcCalcSize_TEST1(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (User32.GetWindowPlacement(hWnd).showCmd == User32.SW_MAXIMIZE)
            {
                RECT rect1 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                User32.DefWindowProc(hWnd, User32.WM_NCCALCSIZE, wParam, lParam);
                RECT rect2 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                MONITORINFO monitorInfo = User32.MonitorInfoFromWindow(hWnd);
                if (monitorInfo.monitorArea.Width == monitorInfo.workArea.Width &&
                    monitorInfo.monitorArea.Height == monitorInfo.workArea.Height)
                    rect2.bottom--;

                rect2.top = rect1.top + User32.GetWindowInfo(hWnd).cyWindowBorders;
                Marshal.StructureToPtr(rect2, lParam, true);
            }

            handled = true;
            return IntPtr.Zero;
        }

        private IntPtr WmNcCalcSize_TEST2(IntPtr hWnd, IntPtr wParam, IntPtr lParam, ref bool handled)
        {
            if (User32.GetWindowPlacement(hWnd).showCmd == User32.SW_MAXIMIZE)
            {
                RECT rect1 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));
                User32.DefWindowProc(hWnd, User32.WM_NCCALCSIZE, wParam, lParam);
                RECT rect2 = (RECT)Marshal.PtrToStructure(lParam, typeof(RECT));

                IntPtr hMonitor = User32.MonitorFromWindow(hWnd, User32.MONITOR_DEFAULTTONEAREST);
                MONITORINFO monitorInfo = User32.GetMonitorInfo(hMonitor);

                if (monitorInfo.monitorArea.Width == monitorInfo.workArea.Width &&
                    monitorInfo.monitorArea.Height == monitorInfo.workArea.Height)
                    rect2.bottom--;

                rect2.top = rect1.top + User32.GetWindowInfo(hWnd).cyWindowBorders;
                Marshal.StructureToPtr(rect2, lParam, true);
            }

            handled = true;
            return IntPtr.Zero;
        }

        private IntPtr WmGetMinMaxInfo(IntPtr hWnd, IntPtr lParam, ref bool handled)
        {
            MINMAXINFO minMaxInfo = (MINMAXINFO)Marshal.PtrToStructure(lParam, typeof(MINMAXINFO));

            IntPtr hMonitor = User32.MonitorFromWindow(hWnd, User32.MONITOR_DEFAULTTONEAREST);

            if (hMonitor != IntPtr.Zero)
            {
                MONITORINFO monitorInfo = User32.GetMonitorInfo(hMonitor);
                RECT rcWorkArea = monitorInfo.workArea;
                RECT rcMonitorArea = monitorInfo.monitorArea;
                minMaxInfo.maxPosition.x = Math.Abs(rcWorkArea.left - rcMonitorArea.left);
                minMaxInfo.maxPosition.y = Math.Abs(rcWorkArea.top - rcMonitorArea.top);
                minMaxInfo.maxSize.x = Math.Abs(rcWorkArea.right - rcWorkArea.left);
                minMaxInfo.maxSize.y = Math.Abs(rcWorkArea.bottom - rcWorkArea.top);
                minMaxInfo.minTrackSize.x = 200;
                minMaxInfo.minTrackSize.y = 100;
            }

            Marshal.StructureToPtr(minMaxInfo, lParam, true);

            handled = true;
            return IntPtr.Zero;
        }
        //*/
    }
}
