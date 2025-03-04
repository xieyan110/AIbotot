using System;
using System.Drawing;
using System.Windows.Forms;

namespace WindowsAPI
{

    /// <summary>
    /// Windows API 中桌面相关方法的包装器。
    /// </summary>
    public static class Desktop
    {
        /// <summary>
        /// 获取桌面的屏幕截图
        /// </summary>
        /// <returns>Desktop screenshot</returns>
        public static Bitmap Screenshot()
        {
            Bitmap bmpScreenshot = new Bitmap(Screen.PrimaryScreen.Bounds.Width, Screen.PrimaryScreen.Bounds.Height);
            Graphics g = Graphics.FromImage(bmpScreenshot);
            g.CopyFromScreen(0, 0, 0, 0, Screen.PrimaryScreen.Bounds.Size);
            g.Dispose();
            return bmpScreenshot;
        }

        /// <summary>
        /// 隐藏任务栏。
        /// </summary>
        public static void HideTaskBar()
        {
            IntPtr hWndDesktop = WinAPI.GetDesktopWindow();
            IntPtr hWndStartButton = WinAPI.FindWindowEx(hWndDesktop, 0, "button", 0);
            IntPtr hWndTaskBar = WinAPI.FindWindowEx(hWndDesktop, 0, "Shell_TrayWnd", 0);
            WinAPI.SetWindowPos(hWndStartButton, 0, 0, 0, 0, 0, 0x0080);
            WinAPI.SetWindowPos(hWndTaskBar, 0, 0, 0, 0, 0, 0x0080);
        }

        /// <summary>
        /// 显示任务栏。
        /// </summary>
        public static void ShowTaskBar()
        {
            IntPtr hWndDesktop = WinAPI.GetDesktopWindow();
            IntPtr hWndStartButton = WinAPI.FindWindowEx(hWndDesktop, 0, "button", 0);
            IntPtr hWndTaskBar = WinAPI.FindWindowEx(hWndDesktop, 0, "Shell_TrayWnd", 0);
            WinAPI.SetWindowPos(hWndStartButton, 0, 0, 0, 0, 0, 0x0040);
            WinAPI.SetWindowPos(hWndTaskBar, 0, 0, 0, 0, 0, 0x0040);
        }

        /// <summary>
        /// 以像素为单位获取桌面的宽度。
        /// </summary>
        /// <returns>Desktop width.</returns>
        public static int GetWidth()
        {
            return Screen.PrimaryScreen.Bounds.Width;
        }

        /// <summary>
        /// 以像素为单位获取桌面的高度。
        /// </summary>
        /// <returns>Desktop height.</returns>
        public static int GetHeight()
        {
            return Screen.PrimaryScreen.Bounds.Height;
        }
    }
}