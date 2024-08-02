using System;
using System.Runtime.InteropServices;
using System.Drawing;
using System.Threading;

namespace WindowsAPI
{
 
    public class MouseControl
    {
        // Windows API 函数导入
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool SendMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        [DllImport("user32.dll")]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool ScreenToClient(IntPtr hWnd, ref POINT lpPoint);

        [DllImport("user32.dll")]
        private static extern bool SetCursorPos(int X, int Y);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);


        // 定义POINT结构体，用于坐标转换
        [StructLayout(LayoutKind.Sequential)]
        public struct POINT
        {
            public int X;
            public int Y;
        }

        // Windows消息常量
        private const uint WM_LBUTTONDOWN = 0x0201;
        private const uint WM_LBUTTONUP = 0x0202;
        private const uint WM_RBUTTONDOWN = 0x0204;
        private const uint WM_RBUTTONUP = 0x0205;
        private const uint WM_MBUTTONDOWN = 0x0207;
        private const uint WM_MBUTTONUP = 0x0208;
        private const uint WM_MOUSEWHEEL = 0x020A;

        // 其他常量
        private const int WHEEL_DELTA = 120;

        // 创建LPARAM
        private static IntPtr MakeLParam(int LoWord, int HiWord)
        {
            return (IntPtr)((HiWord << 16) | (LoWord & 0xffff));
        }

        // 获取当前鼠标在窗口中的坐标
        private static Point GetCursorPositionInWindow(IntPtr hwnd)
        {
            POINT point;
            GetCursorPos(out point);
            ScreenToClient(hwnd, ref point);
            return new Point(point.X, point.Y);
        }

        // 模拟鼠标左键按下
        public static void LeftDown(IntPtr hwnd)
        {
            Point pos = GetCursorPositionInWindow(hwnd);
            IntPtr lParam = MakeLParam(pos.X, pos.Y);
            PostMessage(hwnd, WM_LBUTTONDOWN, (IntPtr)0x0001, lParam);
        }

        // 模拟鼠标左键释放
        public static void LeftUp(IntPtr hwnd)
        {
            Point pos = GetCursorPositionInWindow(hwnd);
            IntPtr lParam = MakeLParam(pos.X, pos.Y);
            PostMessage(hwnd, WM_LBUTTONUP, (IntPtr)0x0001, lParam);
        }

        // 模拟鼠标左键点击
        public static void LeftClick(IntPtr hwnd)
        {
            LeftDown(hwnd);
            Thread.Sleep(50);
            LeftUp(hwnd);
        }

        // 模拟鼠标右键按下
        public static void RightDown(IntPtr hwnd)
        {
            Point pos = GetCursorPositionInWindow(hwnd);
            IntPtr lParam = MakeLParam(pos.X, pos.Y);
            PostMessage(hwnd, WM_RBUTTONDOWN, IntPtr.Zero, lParam);
        }

        // 模拟鼠标右键释放
        public static void RightUp(IntPtr hwnd)
        {
            Point pos = GetCursorPositionInWindow(hwnd);
            IntPtr lParam = MakeLParam(pos.X, pos.Y);
            PostMessage(hwnd, WM_RBUTTONUP, IntPtr.Zero, lParam);
        }

        // 模拟鼠标右键点击
        public static void RightClick(IntPtr hwnd)
        {
            RightDown(hwnd);
            Thread.Sleep(50);
            RightUp(hwnd);
        }

        // 模拟鼠标中键按下
        public static void MiddleDown(IntPtr hwnd)
        {
            Point pos = GetCursorPositionInWindow(hwnd);
            IntPtr lParam = MakeLParam(pos.X, pos.Y);
            PostMessage(hwnd, WM_MBUTTONDOWN, IntPtr.Zero, lParam);
        }

        // 模拟鼠标中键释放
        public static void MiddleUp(IntPtr hwnd)
        {
            Point pos = GetCursorPositionInWindow(hwnd);
            IntPtr lParam = MakeLParam(pos.X, pos.Y);
            PostMessage(hwnd, WM_MBUTTONUP, IntPtr.Zero, lParam);
        }

        // 模拟鼠标中键点击
        public static void MiddleClick(IntPtr hwnd)
        {
            MiddleDown(hwnd);
            Thread.Sleep(50);
            MiddleUp(hwnd);
        }

        // 模拟鼠标滚轮
        public static void Scroll(IntPtr hwnd, int delta)
        {
            Point pos = GetCursorPositionInWindow(hwnd);
            IntPtr wParam = MakeLParam(0, delta * WHEEL_DELTA);
            IntPtr lParam = MakeLParam(pos.X, pos.Y);
            SendMessage(hwnd, WM_MOUSEWHEEL, wParam, lParam);
        }


        private static Point ClientToScreen(IntPtr hwnd, int x, int y)
        {
            POINT point = new POINT { X = x, Y = y };
            ClientToScreen(hwnd, ref point);
            return new Point(point.X, point.Y);
        }

        // 新增：移动鼠标到指定窗口的客户区坐标
        public static void MoveTo(IntPtr hwnd, int x, int y)
        {
            Point screenPoint = ClientToScreen(hwnd, x, y);
            SetCursorPos(screenPoint.X, screenPoint.Y);
        }

        // 新增：移动鼠标（相对当前位置）
        public static void MoveRelative(int deltaX, int deltaY)
        {
            POINT currentPos;
            GetCursorPos(out currentPos);
            SetCursorPos(currentPos.X + deltaX, currentPos.Y + deltaY);
        }

    }
}
