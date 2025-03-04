using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace WindowsAPI
{
    public class WindowFinder
    {
        [DllImport("user32.dll")]
        private static extern bool EnumWindows(EnumWindowsProc enumProc, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        private delegate bool EnumWindowsProc(IntPtr hWnd, IntPtr lParam);

        private static IntPtr foundHandle = IntPtr.Zero;
        private static string searchText;

        private static bool EnumWindowsCallback(IntPtr hWnd, IntPtr lParam)
        {
            StringBuilder sb = new StringBuilder(256);
            GetWindowText(hWnd, sb, 256);
            string title = sb.ToString();

            if (title.Contains(searchText))
            {
                foundHandle = hWnd;
                return false; // 停止枚举
            }
            return true; // 继续枚举
        }

        public static IntPtr GetFirstMatchingWindowHandle(string partialTitle)
        {
            searchText = partialTitle;
            foundHandle = IntPtr.Zero;
            EnumWindows(EnumWindowsCallback, IntPtr.Zero);
            return foundHandle;
        }

        public static bool TryGetFirstMatchingWindowHandle(string partialTitle, out IntPtr windowHandle)
        {
            windowHandle = GetFirstMatchingWindowHandle(partialTitle);
            return windowHandle != IntPtr.Zero;
        }
    }

}
