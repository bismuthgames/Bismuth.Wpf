using System.Windows;
using Bismuth.Wpf.Native;

namespace Bismuth.Wpf.Helpers
{
    public static class MouseHelper
    {
        public static Point GetPosition()
        {
            POINT point = new POINT();
            User32.GetCursorPos(ref point);
            return new Point(point.x, point.y);
        }

        public static void SetPosition(Point point)
        {
            User32.SetCursorPos((int)point.X, (int)point.Y);
        }
    }
}
