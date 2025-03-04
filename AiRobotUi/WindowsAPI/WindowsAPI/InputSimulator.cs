using System;
using System.Collections.Generic;
using System.Threading;
using System.Runtime.InteropServices;


namespace WindowsAPI
{
    public class InputSimulator
    {
        [StructLayout(LayoutKind.Sequential)]
        struct MOUSEINPUT
        {
            public int dx;
            public int dy;
            public uint mouseData;
            public uint dwFlags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [StructLayout(LayoutKind.Sequential)]
        struct INPUT
        {
            public uint type;
            public MOUSEINPUT mi;
        }

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint SendInput(uint nInputs, INPUT[] pInputs, int cbSize);

        [DllImport("user32.dll")]
        private static extern bool ClientToScreen(IntPtr hWnd, ref POINT lpPoint);

        [StructLayout(LayoutKind.Sequential)]
        struct POINT
        {
            public int X;
            public int Y;
        }

        private const uint INPUT_MOUSE = 0;
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_ABSOLUTE = 0x8000;
        private const uint MOUSEEVENTF_MOVE = 0x0001;

        public static void ExecuteInputSequence(IntPtr hwnd, string sequence)
        {
            var actions = ParseSequence(sequence);
            foreach (var action in actions)
            {
                ExecuteAction(hwnd, action);
            }
        }



        private static List<(string action, double duration)> ParseSequence(string sequence)
        {
            var actions = new List<(string action, double duration)>();
            var items = sequence.Split(new[] { ')', '(' }, StringSplitOptions.RemoveEmptyEntries);

            for (int i = 0; i < items.Length; i += 2)
            {
                if (i + 1 < items.Length)
                {
                    string action = items[i].Trim();
                    if (double.TryParse(items[i + 1], out double duration))
                    {
                        actions.Add((action, duration));
                    }
                    else
                    {
                        Console.WriteLine($"Warning: Invalid duration format for action '{action}': {items[i + 1]}");
                    }
                }
                else
                {
                    Console.WriteLine($"Warning: Missing duration for action: {items[i]}");
                }
            }
            return actions;
        }
        private static void ExecuteAction(IntPtr hwnd, (string action, double duration) actionInfo)
        {
            string action = actionInfo.action.ToLower();
            int durationMs = (int)(actionInfo.duration * 1000);

            switch (action)
            {
                case "pass":
                case "wait":
                    Thread.Sleep(durationMs);
                    break;
                case "lmb":
                    MouseClick(hwnd, MOUSEEVENTF_LEFTDOWN, MOUSEEVENTF_LEFTUP, durationMs);
                    break;
                case "rmb":
                    MouseClick(hwnd, MOUSEEVENTF_RIGHTDOWN, MOUSEEVENTF_RIGHTUP, durationMs);
                    break;
                case "mmb":
                    MouseClick(hwnd, MOUSEEVENTF_MIDDLEDOWN, MOUSEEVENTF_MIDDLEUP, durationMs);
                    break;
                default:
                    KeyboardControl.KeyDown(hwnd, action);
                    Thread.Sleep(durationMs);
                    KeyboardControl.KeyUp(hwnd, action);
                    break;
            }

            Thread.Sleep(10);
        }

        private static void MouseClick(IntPtr hwnd, uint downFlag, uint upFlag, int duration)
        {
            POINT p = new POINT { X = 641, Y = 353 };
            ClientToScreen(hwnd, ref p);

            INPUT[] inputs = new INPUT[3];

            // Move mouse to the window
            inputs[0].type = INPUT_MOUSE;
            inputs[0].mi.dx = p.X * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Width;
            inputs[0].mi.dy = p.Y * 65536 / System.Windows.Forms.Screen.PrimaryScreen.Bounds.Height;
            inputs[0].mi.dwFlags = MOUSEEVENTF_MOVE | MOUSEEVENTF_ABSOLUTE;

            // Mouse down
            inputs[1].type = INPUT_MOUSE;
            inputs[1].mi.dwFlags = downFlag;

            // Mouse up
            inputs[2].type = INPUT_MOUSE;
            inputs[2].mi.dwFlags = upFlag;

            SendInput(1, inputs, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(duration);
            SendInput(1, new INPUT[] { inputs[1] }, Marshal.SizeOf(typeof(INPUT)));
            Thread.Sleep(50);
            SendInput(1, new INPUT[] { inputs[2] }, Marshal.SizeOf(typeof(INPUT)));
        }
    }
}
