using System.Runtime.InteropServices;

namespace Bismuth.Wpf.Native
{
    [StructLayout(LayoutKind.Sequential)]
    internal struct RECT
    {
        public int left;
        public int top;
        public int right;
        public int bottom;

        public int Width
        {
            get { return right - left; }
            set { right = left + value; }
        }

        public int Height
        {
            get { return bottom - top; }
            set { bottom = top + value; }
        }
    }
}
