using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

namespace WindowsAPI
{
    public class MouseControl
    {
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;
        private const uint WM_RBUTTONDOWN = 0x0204;
        private const uint WM_RBUTTONUP = 0x0205;
        private const uint WM_MBUTTONDOWN = 0x0207;
        private const uint WM_MBUTTONUP = 0x0208;
        private const uint WM_MOUSEWHEEL = 0x020A;

        private const int WHEEL_DELTA = 120;

        private static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        public static void LeftDown(IntPtr hwnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(hwnd, WM_LBUTTONDOWN, (IntPtr)0x0001, lParam);
        }

        public static void LeftUp(IntPtr hwnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(hwnd, WM_LBUTTONUP, (IntPtr)0x0001, lParam);
        }

        public static void LeftClick(IntPtr hwnd, int x, int y)
        {
            LeftDown(hwnd, x, y);
            Thread.Sleep(50);
            LeftUp(hwnd, x, y);
        }

        public static void RightDown(IntPtr hwnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(hwnd, WM_RBUTTONDOWN, IntPtr.Zero, lParam);
        }

        public static void RightUp(IntPtr hwnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(hwnd, WM_RBUTTONUP, IntPtr.Zero, lParam);
        }

        public static void RightClick(IntPtr hwnd, int x, int y)
        {
            RightDown(hwnd, x, y);
            Thread.Sleep(50);
            RightUp(hwnd, x, y);
        }

        public static void MiddleDown(IntPtr hwnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(hwnd, WM_MBUTTONDOWN, IntPtr.Zero, lParam);
        }

        public static void MiddleUp(IntPtr hwnd, int x, int y)
        {
            IntPtr lParam = MakeLParam(x, y);
            PostMessage(hwnd, WM_MBUTTONUP, IntPtr.Zero, lParam);
        }

        public static void MiddleClick(IntPtr hwnd, int x, int y)
        {
            MiddleDown(hwnd, x, y);
            Thread.Sleep(50);
            MiddleUp(hwnd, x, y);
        }

        public static void Scroll(IntPtr hwnd, int x, int y, int delta)
        {
            IntPtr wParam = MakeLParam(0, delta * WHEEL_DELTA);
            IntPtr lParam = MakeLParam(x, y);
            SendMessage(hwnd, WM_MOUSEWHEEL, wParam, lParam);
        }

        private static Point ClientToScreen(IntPtr hwnd, int x, int y)
        {
            POINT point = new POINT { X = x, Y = y };
            ClientToScreen(hwnd, ref point);
            return new Point(point.X, point.Y);
        }

        public static void MoveTo(IntPtr hwnd, int x, int y)
        {
            Point screenPoint = ClientToScreen(hwnd, x, y);
            SetCursorPos(screenPoint.X, screenPoint.Y);
        }

        public static void MoveRelative(int deltaX, int deltaY)
        {
            POINT currentPos;
            GetCursorPos(out currentPos);
            SetCursorPos(currentPos.X + deltaX, currentPos.Y + deltaY);
        }

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);
    }
}