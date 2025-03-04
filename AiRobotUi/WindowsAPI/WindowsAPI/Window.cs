using System;
using System.Drawing;
using System.Drawing.Imaging;
using System.Text;
using System.Windows.Forms;

namespace WindowsAPI
{

    /// <summary>
    /// 窗口相关函数的 Windows API 包装类。
    /// </summary>
    public static class Window
    {

        /// <summary>
        /// 检查特定窗口是否打开。
        /// </summary>
        /// <param name="windowTitle">窗口标题.</param>
        /// <returns>如果窗口打开则为真.</returns>
        public static bool DoesExist(string windowTitle)
        {
            IntPtr hWnd = WinAPI.FindWindow(null, windowTitle);
            return hWnd != IntPtr.Zero;
        }

        /// <summary>
        /// 查找并返回具有指定标题的第一个窗口的句柄.
        /// </summary>
        /// <param name="windowTitle">窗口标题.</param>
        /// <returns>窗户的把手.</returns>
        public static IntPtr Get(string windowTitle)
        {
            IntPtr hWnd = WinAPI.FindWindow(null, windowTitle);
            if (hWnd == IntPtr.Zero)
                throw new Exception("Window not found.");
            return hWnd;
        }

        /// <summary>
        /// 获取当前焦点窗口的句柄。
        /// </summary>
        /// <returns>焦点窗口的句柄.</returns>
        public static IntPtr GetFocused()
        {
            return WinAPI.GetForegroundWindow();
        }

        /// <summary>
        /// 将焦点放在窗口上。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void SetFocused(IntPtr hWnd)
        {
            WinAPI.SetForegroundWindow(hWnd);
        }

        /// <summary>
        /// 检查指定的窗口是否聚焦。
        /// </summary>
        /// <param name="windowTitle">窗口标题.</param>
        /// <returns>如果窗口被聚焦，则为真.</returns>
        public static bool IsFocused(IntPtr hWnd)
        {
            IntPtr hWndFocused = WinAPI.GetForegroundWindow();
            if (hWnd == IntPtr.Zero || hWndFocused == IntPtr.Zero) return false;
            return hWnd == hWndFocused;
        }

        /// <summary>
        /// 将指定的窗口移动到左上角。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <param name="x">新位置的 x 坐标.</param>
        /// <param name="y">新位置的 y 坐标.</param>
        public static void Move(IntPtr hWnd, int x, int y)
        {
            WinAPI.SetWindowPos(hWnd, 0, x, y, 0, 0, 0x0001);
        }

        /// <summary>
        /// Resize the specified window.
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <param name="x">新尺寸的宽度.</param>
        /// <param name="y">新尺寸的高度.</param>
        public static void Resize(IntPtr hWnd, int width, int height)
        {
            WinAPI.SetWindowPos(hWnd, 0, 0, 0, width, height, 0x002);
        }

        /// <summary>
        /// 隐藏指定的窗口。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void Hide(IntPtr hWnd)
        {
            WinAPI.SetWindowPos(hWnd, 0, 0, 0, 0, 0, 0x0080);
        }

        /// <summary>
        /// 显示指定的隐藏窗口。隐藏与最小化不同。对于最小化的窗口，请改用 normalize 方法。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void Show(IntPtr hWnd)
        {
            WinAPI.SetWindowPos(hWnd, 0, 0, 0, 0, 0, 0x0040);
        }

        /// <summary>
        /// 获取指定窗口的尺寸。包括位置和大小。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <returns>窗户的尺寸.</returns>
        public static Rectangle GetDimensions(IntPtr hWnd)
        {
            Structs.Rect hWndRect = new Structs.Rect();
            WinAPI.GetWindowRect(hWnd, out hWndRect);

            return new Rectangle(hWndRect.X, hWndRect.Y, hWndRect.Width, hWndRect.Height);
        }

        /// <summary>
        /// 获取指定窗口的大小。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <returns>窗口的大小.</returns>
        public static Size GetSize(IntPtr hWnd)
        {
            Rectangle rec = GetDimensions(hWnd);
            Size size = new Size(rec.Width, rec.Height);
            return size;
        }

        /// <summary>
        /// 通过左上角获取指定窗口的位置。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <returns>The location of the window.</returns>
        public static Point GetLocation(IntPtr hWnd)
        {
            Rectangle rec = GetDimensions(hWnd);
            Point point = new Point(rec.X, rec.Y);
            return point;
        }

        /// <summary>
        /// 获取指定窗口的标题
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <returns>窗口标题.</returns>
        public static string GetTitle(IntPtr hWnd)
        {
            const int nChars = 256;
            StringBuilder Buff = new StringBuilder(nChars);

            if (WinAPI.GetWindowText(hWnd, Buff, nChars) > 0)
                return Buff.ToString();

            return null;
        }

        /// <summary>
        /// 更改指定窗口的标题。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <param name="title">The new title.</param>
        public static void SetTitle(IntPtr hWnd, string title)
        {
            WinAPI.SetWindowText(hWnd, title);
        }

        /// <summary>
        /// 最大化指定的窗口。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void Maximize(IntPtr hWnd)
        {
            WinAPI.ShowWindow(hWnd, 3);
        }

        /// <summary>
        /// 最小化指定的窗口。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void Minimize(IntPtr hWnd)
        {
            WinAPI.ShowWindow(hWnd, 6);
        }

        /// <summary>
        /// 如果指定的窗口最大化或最小化，则将其恢复到其原始位置。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void Normalize(IntPtr hWnd)
        {
            WinAPI.ShowWindow(hWnd, 1);
        }

        /// <summary>
        /// 获取指定窗口的屏幕截图。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <returns></returns>
        public static Bitmap Screenshot(IntPtr hWnd)
        {
            Normalize(hWnd);
            Structs.Rect rc;
            WinAPI.GetWindowRect(hWnd, out rc);

            Bitmap bmp = new Bitmap(rc.Width, rc.Height, PixelFormat.Format32bppArgb);
            Graphics gfxBmp = Graphics.FromImage(bmp);
            IntPtr hdcBitmap = gfxBmp.GetHdc();

            WinAPI.PrintWindow(hWnd, hdcBitmap, 0);

            gfxBmp.ReleaseHdc(hdcBitmap);
            gfxBmp.Dispose();

            // 如果您想省略窗口标题和边框，请使用以下命令：
            //bmp = ImageProcessing.Crop(bmp, new Rectangle(8, 30, bmp.Width - 16, bmp.Height - 30 - 8));

            return bmp;
        }

        /// <summary>
        /// 删除窗口的整个菜单栏。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void RemoveMenu(IntPtr hWnd)
        {
            IntPtr hMenu = WinAPI.GetMenu(hWnd);
            int count = WinAPI.GetMenuItemCount(hMenu);
            //loop & remove
            for (int i = 0; i < count; i++)
                WinAPI.RemoveMenu(hMenu, 0, (0x400 | 0x1000));

            //force a redraw
            WinAPI.DrawMenuBar(hWnd);
        }

        /// <summary>
        /// 关闭一个窗口，但在关闭前提示选择保存。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void Close(IntPtr hWnd)
        {
            // fShutDown = true will kill the window instantly
            // fShutDown = false will show the message box before closing for Saving Changes
            WinAPI.EndTask(hWnd, true, true);
        }

        /// <summary>
        /// 禁用窗口的关闭按钮。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void DisableCloseButton(IntPtr hWnd)
        {
            IntPtr hMenu;
            int n;
            hMenu = WinAPI.GetSystemMenu(hWnd, false);
            if (hMenu != IntPtr.Zero)
            {
                n = WinAPI.GetMenuItemCount(hMenu);
                if (n > 0)
                {
                    WinAPI.RemoveMenu(hMenu, (uint)(n - 1), 0x400 | 0x1000);
                    WinAPI.RemoveMenu(hMenu, (uint)(n - 2), 0x400 | 0x1000);
                    WinAPI.DrawMenuBar(hWnd);
                }
            }
        }

        /// <summary>
        /// 禁用窗口的最大化按钮。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void DisableMaximizeButton(IntPtr hWnd)
        {
            var currentStyle = WinAPI.GetWindowLong(hWnd, -16);
            WinAPI.SetWindowLong(hWnd, -16, (currentStyle & ~0x10000));
        }

        /// <summary>
        /// 禁用窗口的最小化按钮。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void DisableMinimizeButton(IntPtr hWnd)
        {
            var currentStyle = WinAPI.GetWindowLong(hWnd, -16);
            WinAPI.SetWindowLong(hWnd, -16, (currentStyle & ~0x20000));
        }

        /// <summary>
        /// 翻转窗口的布局。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void FlipLeft(IntPtr hWnd)
        {
            var currentStyle = WinAPI.GetWindowLong(hWnd, -20);
            WinAPI.SetWindowLong(hWnd, -20, (currentStyle | (int)0x00400000L));
        }

        /// <summary>
        /// 将窗口的布局翻转回其原始状态。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        public static void FlipRight(IntPtr hWnd)
        {
            WinAPI.SetWindowLong(hWnd, -20, 0);
        }

        /// <summary>
        /// 允许用户单击窗口。
        /// </summary>
        /// <param name="Handle">窗户的句柄.</param>
        public static void EnableMouseTransparency(IntPtr hWnd)
        {
            WinAPI.SetWindowLong(hWnd, -20, Convert.ToInt32(WinAPI.GetWindowLong(hWnd, -20) | 0x00080000 | 0x00000020L));
        }

        /// <summary>
        /// 将全局坐标转换为窗口坐标。
        /// </summary>
        /// <param name="hWnd">窗户的句柄.</param>
        /// <param name="x">The x coordinate.</param>
        /// <param name="y">The y coordinate.</param>
        /// <returns>The coordinate.</returns>
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
        /// <param name="hWnd">窗户的句柄.</param>
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