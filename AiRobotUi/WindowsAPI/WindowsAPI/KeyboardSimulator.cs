using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace WindowsAPI
{

    public class KeyboardControl
    {
        // Windows API 函数导入
        [DllImport("user32.dll")]
        private static extern bool PostMessage(IntPtr hWnd, uint Msg, IntPtr wParam, IntPtr lParam);

        // Windows消息常量
        private const uint WM_KEYDOWN = 0x0100;
        private const uint WM_KEYUP = 0x0101;

        // 特殊键的映射
        private static readonly Dictionary<string, Keys> SpecialKeys = new Dictionary<string, Keys>
{
    {"Alt", Keys.Alt},
    {"Ctrl", Keys.Control},
    {"Shift", Keys.Shift},
    {"Enter", Keys.Enter},
    {"Return", Keys.Return}, // 同 Enter
    {"Escape", Keys.Escape},
    {"Esc", Keys.Escape}, // Escape 的简写
    {"Space", Keys.Space},
    {"Tab", Keys.Tab},
    {"Backspace", Keys.Back},
    {"Delete", Keys.Delete},
    {"Del", Keys.Delete}, // Delete 的简写
    {"Insert", Keys.Insert},
    {"Ins", Keys.Insert}, // Insert 的简写
    {"Home", Keys.Home},
    {"End", Keys.End},
    {"PageUp", Keys.PageUp},
    {"PageDown", Keys.PageDown},
    {"Up", Keys.Up},
    {"Down", Keys.Down},
    {"Left", Keys.Left},
    {"Right", Keys.Right},
    {"PrintScreen", Keys.PrintScreen},
    {"PrtSc", Keys.PrintScreen}, // PrintScreen 的简写
    {"Pause", Keys.Pause},
    {"NumLock", Keys.NumLock},
    {"CapsLock", Keys.CapsLock},
    {"ScrollLock", Keys.Scroll},
    {"F1", Keys.F1},
    {"F2", Keys.F2},
    {"F3", Keys.F3},
    {"F4", Keys.F4},
    {"F5", Keys.F5},
    {"F6", Keys.F6},
    {"F7", Keys.F7},
    {"F8", Keys.F8},
    {"F9", Keys.F9},
    {"F10", Keys.F10},
    {"F11", Keys.F11},
    {"F12", Keys.F12},
    {"NumPad0", Keys.NumPad0},
    {"NumPad1", Keys.NumPad1},
    {"NumPad2", Keys.NumPad2},
    {"NumPad3", Keys.NumPad3},
    {"NumPad4", Keys.NumPad4},
    {"NumPad5", Keys.NumPad5},
    {"NumPad6", Keys.NumPad6},
    {"NumPad7", Keys.NumPad7},
    {"NumPad8", Keys.NumPad8},
    {"NumPad9", Keys.NumPad9},
    {"Add", Keys.Add}, // 数字键盘的加号
    {"Subtract", Keys.Subtract}, // 数字键盘的减号
    {"Multiply", Keys.Multiply}, // 数字键盘的乘号
    {"Divide", Keys.Divide}, // 数字键盘的除号
    {"Decimal", Keys.Decimal}, // 数字键盘的小数点
    {"Win", Keys.LWin}, // Windows 键
    {"Windows", Keys.LWin}, // Windows 键的另一种表示
    {"Menu", Keys.Apps}, // 应用程序键（通常在右Ctrl旁边）
    {"VolumeMute", Keys.VolumeMute},
    {"VolumeDown", Keys.VolumeDown},
    {"VolumeUp", Keys.VolumeUp},
    {"MediaNextTrack", Keys.MediaNextTrack},
    {"MediaPreviousTrack", Keys.MediaPreviousTrack},
    {"MediaStop", Keys.MediaStop},
    {"MediaPlayPause", Keys.MediaPlayPause},
};

        // 模拟按键按下
        public static void KeyDown(IntPtr hwnd, string key)
        {
            Keys keyCode = GetKeyCode(key);
            PostMessage(hwnd, WM_KEYDOWN, (IntPtr)keyCode, IntPtr.Zero);
        }

        // 模拟按键释放
        public static void KeyUp(IntPtr hwnd, string key)
        {
            Keys keyCode = GetKeyCode(key);
            PostMessage(hwnd, WM_KEYUP, (IntPtr)keyCode, IntPtr.Zero);
        }

        // 模拟按键按下然后释放
        public static void KeyPress(IntPtr hwnd, string key)
        {
            KeyDown(hwnd, key);
            Thread.Sleep(50); // 短暂延迟，模拟真实按键
            KeyUp(hwnd, key);
        }

        // 输入一串字符
        public static void InputString(IntPtr hwnd, string input)
        {
            foreach (char c in input)
            {
                KeyPress(hwnd, c.ToString());
                Thread.Sleep(new Random().Next(50, 150)); // 随机延迟，模拟真实输入
            }
        }

        /// <summary>
        /// 获取键码
        /// </summary>
        /// <param name="key"></param>
        /// <returns></returns>
        /// <exception cref="ArgumentException"></exception>
        private static Keys GetKeyCode(string key)
        {
            // 对特殊键进行大小写不敏感的匹配
            var specialKey = SpecialKeys.FirstOrDefault(k => k.Key.Equals(key, StringComparison.OrdinalIgnoreCase));
            if (!specialKey.Equals(default(KeyValuePair<string, Keys>)))
            {
                return specialKey.Value;
            }
            else if (key.Length == 1)
            {
                key = key.ToUpper();
                char c = key[0];
                if (char.IsLetter(c))
                {
                    // 保持字母的原有大小写
                    return (Keys)c;
                }
                else if (char.IsDigit(c))
                {
                    return (Keys)c;
                }
                else
                {
                    // 对于其他字符，尝试直接转换
                    return (Keys)c;
                }
            }
            else
            {
                throw new ArgumentException($"Unsupported key: {key}");
            }
        }
    }
}
