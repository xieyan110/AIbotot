using System;
using System.Drawing;
using System.Runtime.InteropServices;
using System.Text;

namespace WindowsAPI
{
    /// <summary>
    /// Windows API 方法。所有其他类都是这些方法的包装器。
    /// </summary>
    public static class WinAPI
    {
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDesktopWindow();
        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindowEx(IntPtr parentHandle, int childAfter, string className, int windowTitle);
        [DllImport("user32.dll")]
        internal static extern bool GetWindowRect(IntPtr hWnd, out Structs.Rect lpRect);
        [DllImport("user32.dll")]
        internal static extern int GetWindowText(IntPtr hWnd, StringBuilder text, int count);
        [DllImport("user32.dll")]
        internal static extern bool PrintWindow(IntPtr hWnd, IntPtr hdcBlt, int nFlags);
        [DllImport("user32.dll")]
        internal static extern IntPtr FindWindow(String sClassName, String sAppName);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetForegroundWindow();
        [DllImport("user32.dll")]
        internal static extern bool SetForegroundWindow(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowPos(IntPtr hWnd, int hWndInsertAfter, int x, int y, int cX, int cY, int wFlags);
        [DllImport("user32.dll")]
        internal static extern IntPtr SetWindowText(IntPtr hWnd, String lpString);
        [DllImport("user32.dll")]
        internal static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);
        [DllImport("user32.dll")]
        internal static extern int GetWindowLong(IntPtr hWnd, int nIndex);
        [DllImport("user32.dll")]
        internal static extern int SetWindowLong(IntPtr hWnd, int nIndex, int value);
        [DllImport("user32.dll")]
        internal static extern bool ClientToScreen(IntPtr hWnd, ref Point lpPoint);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetMenu(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern int GetMenuItemCount(IntPtr hMenu);
        [DllImport("user32.dll")]
        internal static extern bool DrawMenuBar(IntPtr hWnd);
        [DllImport("user32.dll")]
        internal static extern bool RemoveMenu(IntPtr hMenu, uint uPosition, uint uFlags);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern bool EndTask(IntPtr hWnd, bool fShutDown, bool fForce);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetSystemMenu(IntPtr hWnd, bool bRevert);
        [DllImport("user32.dll")]
        internal static extern bool SetLayeredWindowAttributes(IntPtr hwnd, uint crKey, byte bAlpha, uint dwFlags);
        [DllImport("user32.dll")]
        internal static extern IntPtr GetDC(IntPtr hwnd);
        [DllImport("user32.dll")]
        internal static extern int ReleaseDC(IntPtr hwnd, IntPtr dc);
        [DllImport("user32.dll", SetLastError = true)]
        internal static extern uint SendInput(uint nInputs, ref Structs.INPUT pInputs, int cbSize);
        
        [DllImport("User32")]
        public static extern bool OpenClipboard(IntPtr hWndNewOwner);

        [DllImport("User32")]
        public static extern bool CloseClipboard();

        [DllImport("User32")]
        public static extern bool EmptyClipboard();

        [DllImport("User32")]
        public static extern bool IsClipboardFormatAvailable(int format);

        [DllImport("User32")]
        public static extern IntPtr GetClipboardData(int uFormat);

        [DllImport("User32", CharSet = CharSet.Unicode)]
        public static extern IntPtr SetClipboardData(int uFormat, IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern IntPtr GlobalLock(IntPtr hMem);

        [DllImport("kernel32.dll")]
        public static extern bool GlobalUnlock(IntPtr hMem);

    }
}