using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsAPI
{

    /// <summary>
    /// 用于在全局坐标和相对于窗口的坐标之间来回切换。
    /// </summary>
    public class Conversion
    {
        /// <summary>
        /// 将全局坐标转换为窗口坐标
        /// </summary>
        /// <param name="hWnd">The handle to the window.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The converted coordinate.</returns>
        public static Point ConvertToWindowCoordinates(IntPtr hWnd, int x, int y)
        {
            Structs.Rect hWndRect = new Structs.Rect();
            WinAPI.GetWindowRect(hWnd, out hWndRect);
            Point point = new Point(hWndRect.X + x, hWndRect.Y + y);
            return point;
        }

        /// <summary>
        /// 获取鼠标相对于窗口左上角的位置。
        /// </summary>
        /// <param name="hWnd">The handle to the window.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The mouse position.</returns>
        public static Point GetCoordinateRelativeToWindow(IntPtr hWnd)
        {
            Structs.Rect hWndRect = new Structs.Rect();
            WinAPI.GetWindowRect(hWnd, out hWndRect);
            int x = Cursor.Position.X;
            int y = Cursor.Position.Y;
            Point point = new Point(x - hWndRect.X, y - hWndRect.Y);
            return point;
        }
    }
}