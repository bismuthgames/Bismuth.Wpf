using System;
using System.ComponentModel;
using System.Runtime.InteropServices;

namespace Bismuth.Wpf.Native
{
    internal static class User32
    {
        public static readonly IntPtr HRGN_NONE = new IntPtr(-1);

        public const int WM_ACTIVATE = 6;
        public const int WM_GETMINMAXINFO = 36;
        public const int WM_NCCALCSIZE = 131;
        public const int WM_NCPAINT = 133;
        public const int WM_NCACTIVATE = 134;
        public const int WM_SYSCOMMAND = 274;

        public const int GWL_STYLE = -16;
        public const int WS_SYSMENU = 0x80000;
        public const int SW_MAXIMIZE = 3;

        public const int MONITOR_DEFAULTTONULL = 0;
        public const int MONITOR_DEFAULTTOPRIMARY = 1;
        public const int MONITOR_DEFAULTTONEAREST = 2;

        [DllImport("user32.dll")]
        public static extern IntPtr SendMessage(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern IntPtr DefWindowProc(IntPtr hWnd, int msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        public static extern bool GetCursorPos(ref POINT point);

        [DllImport("user32.dll")]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll")]
        public static extern IntPtr SetCursor(IntPtr hCursor);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        public static extern IntPtr LoadCursor(IntPtr hInst, IntPtr iconId);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        public static extern bool ClientToScreen(IntPtr hWnd, ref POINT point);

        [DllImport("user32.dll")]
        public static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [DllImport("user32.dll", CharSet = CharSet.Unicode)]
        public static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [DllImport("user32.dll")]
        public static extern IntPtr MonitorFromWindow(IntPtr hWnd, int flags);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetMonitorInfo(IntPtr hMonitor, ref MONITORINFO monitorInfo);

        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowInfo(IntPtr hWnd, ref WINDOWINFO windowInfo);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetWindowPlacement(IntPtr hwnd, ref WINDOWPLACEMENT lpWndPl);

        public static MONITORINFO MonitorInfoFromWindow(IntPtr hWnd)
        {
            IntPtr monitor = MonitorFromWindow(hWnd, 2);
            MONITORINFO monitorInfo = new MONITORINFO();
            monitorInfo.sizeInBytes = Marshal.SizeOf(typeof(MONITORINFO));
            GetMonitorInfo(monitor, ref monitorInfo);
            return monitorInfo;
        }

        public static MONITORINFO GetMonitorInfo(IntPtr hMonitor)
        {
            MONITORINFO monitorInfo = new MONITORINFO();
            monitorInfo.sizeInBytes = Marshal.SizeOf(typeof(WINDOWINFO));
            GetMonitorInfo(hMonitor, ref monitorInfo);
            return monitorInfo;
        }

        public static WINDOWINFO GetWindowInfo(IntPtr hWnd)
        {
            WINDOWINFO windowInfo = new WINDOWINFO();
            windowInfo.sizeInBytes = Marshal.SizeOf(typeof(WINDOWINFO));
            GetWindowInfo(hWnd, ref windowInfo);
            return windowInfo;
        }

        public static WINDOWPLACEMENT GetWindowPlacement(IntPtr hWnd)
        {
            WINDOWPLACEMENT windowPlacement = new WINDOWPLACEMENT();
            windowPlacement.sizeInBytes = Marshal.SizeOf(typeof(WINDOWPLACEMENT));
            if (GetWindowPlacement(hWnd, ref windowPlacement))
                return windowPlacement;

            throw new Win32Exception(Marshal.GetLastWin32Error());
        }
    }
}
