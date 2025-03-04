using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Aibot
{
    public class AdbHelper
    {
        private static string AdbPath = "./PlatformTools/adb.exe"; // 根据实际情况设置 adb 路径
        private static string scrcpyPath = "./PlatformTools/scrcpy.exe"; // 根据实际情况设置 --video-codec=h265 -m1920 --max-fps=60 --no-audio -K

        #region scrcpy
        public static void RunScrcpy(string deviceName = "")
        {
            ProcessStartInfo startInfo = new ProcessStartInfo
            {
                FileName = scrcpyPath,
                Arguments = BuildArguments(deviceName),
                RedirectStandardOutput = true,
                RedirectStandardError = true,
                UseShellExecute = false,
                CreateNoWindow = true
            };

            try
            {
                Process process = Process.Start(startInfo);

                // 如果不需要等待，可以在这里返回
                Console.WriteLine($"Started scrcpy for device {deviceName}");
            }
            catch (Exception ex)
            {
                Console.WriteLine("Exception: " + ex.Message);
            }
        }


        private static string BuildArguments(string deviceName)
        {
            string arguments = "--video-codec=h265 -m1920 --max-fps=60 --no-audio -K";
            if (!string.IsNullOrEmpty(deviceName))
            {
                arguments = $"-s {deviceName} " + arguments;
            }
            return arguments;
        }
        #endregion

        /// <summary>
        /// 执行 ADB 命令
        /// </summary>
        /// <param name="command">ADB 命令参数</param>
        /// <param name="deviceName">可选的设备名称</param>
        public static void RunAdbCommand(string[] command, string deviceName = "")
        {
            try
            {
                var arguments = string.Join(" ", command);
                if (!string.IsNullOrEmpty(deviceName))
                {
                    arguments = $"-s {deviceName} " + arguments;
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = AdbPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                process.WaitForExit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
            }
        }

        /// <summary>
        /// 执行 ADB 命令并获取输出
        /// </summary>
        /// <param name="command">ADB 命令参数</param>
        /// <param name="deviceName">可选的设备名称</param>
        /// <returns>命令输出</returns>
        public static string RunAdbCommandWithOutput(string[] command, string deviceName = "")
        {
            try
            {
                var arguments = string.Join(" ", command);
                if (!string.IsNullOrEmpty(deviceName))
                {
                    arguments = $"-s {deviceName} " + arguments;
                }

                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = AdbPath,
                        Arguments = arguments,
                        RedirectStandardOutput = true,
                        UseShellExecute = false,
                        CreateNoWindow = true
                    }
                };

                process.Start();
                string output = process.StandardOutput.ReadToEnd();
                process.WaitForExit();
                return output;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return string.Empty;
            }
        }

        /// <summary>
        /// 执行 ADB 命令
        /// </summary>
        /// <param name="command">ADB 命令参数</param>


        /// <summary>
        /// 杀死指定的应用
        /// </summary>
        /// <param name="packageName">应用的包名</param>
        public static void KillApp(string packageName, string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "am", "force-stop", packageName }, deviceName);
            Console.WriteLine($"Successfully killed app: {packageName}");
        }

        /// <summary>
        /// 打开指定的应用(双开微信可以使用下面的命令打开微信，不过 /.ui.LauncherUI 不知道会不会变)
        /// adb shell am start -n com.tencent.mm/.ui.LauncherUI
        /// </summary>
        /// <param name="packageName">应用的包名</param>
        public static void OpenApp(string packageName, string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "monkey", "-p", packageName, "-c", "android.intent.category.LAUNCHER", "1"  }, deviceName);
            Console.WriteLine($"Successfully opened app: {packageName}");
        }

        /// <summary>
        /// 等待指定的秒数
        /// </summary>
        /// <param name="seconds">等待的秒数</param>
        public static void Wait(int seconds)
        {
            Console.WriteLine($"Waiting {seconds} seconds...");
            System.Threading.Thread.Sleep(seconds * 1000);
        }



        /// <summary>
        /// 检查指定的应用是否已安装
        /// </summary>
        /// <param name="packageName">应用的包名</param>
        /// <returns>是否已安装</returns>
        public static bool IsAppInstalled(string packageName, string deviceName = "")
        {
            string output = RunAdbCommandWithOutput(new[] { "shell", "pm", "list", "packages", "-f", packageName  }, deviceName);
            return output.Contains(packageName);
        }

        /// <summary>
        /// 安装应用
        /// </summary>
        /// <param name="apkPath">APK 文件路径</param>
        public static void InstallApp(string apkPath, string deviceName = "")
        {
            RunAdbCommand(new[] { "install", "-r", apkPath  }, deviceName);
        }

        /// <summary>
        /// 使用ADB通过IP地址远程连接到Android设备进行调试
        /// </summary>
        /// <param name="ipAddress">Android设备的IP地址</param>
        /// <returns>如果连接成功,返回true,否则返回false</returns>
        public static bool RemoteDebugOverWifi(string ipAddress, string deviceName = "")
        {
            try
            {
                // 首先,尝试断开任何现有连接
                //string output = RunAdbCommandWithOutput(new string[] { "disconnect"  }, deviceName);

                // 连接到指定的IP地址
                var output = RunAdbCommandWithOutput(new string[] { "tcpip", "5555"  }, deviceName);
                output = RunAdbCommandWithOutput(new string[] { "connect", ipAddress  }, deviceName);

                // 检查输出以确定是否连接成功
                if (output.Contains("connected"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// 获取当前默认的输入法
        /// </summary>
        /// <returns>输入法包名</returns>
        public static string GetDefaultIme(string deviceName = "")
        {
            return RunAdbCommandWithOutput(new[] { "shell", "settings", "get", "secure", "default_input_method" },deviceName).Trim();
        }

        /// <summary>
        /// 切换输入法
        /// </summary>
        /// <param name="ime">输入法包名</param>
        public static void SwitchIme(string ime, string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "ime", "set", ime  }, deviceName);
        }

        /// <summary>
        /// 输入文本
        /// </summary>
        /// <param name="text">要输入的文本</param>
        public static void InputText(string text, string deviceName = "")
        {
            string defaultIme = GetDefaultIme(deviceName);
            string adbKeyboardPackage = "com.android.adbkeyboard";
            if (!IsAppInstalled(adbKeyboardPackage, deviceName))
            {
                string apkPath = Path.Combine(Directory.GetCurrentDirectory(), "ADBKeyboard", "ADBKeyboard.apk");
                if (File.Exists(apkPath))
                {
                    InstallApp(apkPath, deviceName);
                }
                else
                {
                    Console.WriteLine("Error: ADBKeyboard APK file does not exist");
                    return;
                }
            }
            SwitchIme($"{adbKeyboardPackage}/.AdbIME", deviceName);
            Thread.Sleep(500);
            // 修改正则表达式以包含表情符号
            // 修改正则表达式以包含换行符
            Regex regex = new Regex(@"[-]|[a-zA-Z0-9]+|[\u4e00-\u9fa5]+|[\u0020-\u002F\u003A-\u0040\u005B-\u0060\u007B-\u007E]+|[\uD800-\uDBFF][\uDC00-\uDFFF]|[^\x00-\x7F]|\r?\n");
            MatchCollection matches = regex.Matches(text);

            foreach (Match match in matches)
            {
                string part = match.Value;
                if (Regex.IsMatch(part, @"[-a-zA-Z0-9@#$%^&*()_+]+"))
                {
                    // 如果是英文字符、数字或特殊字符，使用 ADB_INPUT_TEXT 命令
                    string escapedPart = part.Replace("&", "\\&")
                                             .Replace("(", "\\(")
                                             .Replace(")", "\\)")
                                             .Replace("<", "\\<")
                                             .Replace(">", "\\>")
                                             .Replace("|", "\\|")
                                             .Replace("$", "\\$")
                                             .Replace("\"", "\\\"")
                                             .Replace("'", "\\'")
                                             .Replace("`", "\\`")
                                             .Replace(" ", "\\ ");
                    RunAdbCommand(new[] { "shell", "am", "broadcast", "-a", "ADB_INPUT_TEXT", "--es", "msg", escapedPart }, deviceName);
                }
                else if (part == " ")
                {
                    RunAdbCommand(new[] { "shell", "am", "broadcast", "-a", "ADB_INPUT_CODE", "--ei", "code", "62" }, deviceName);
                }
                else if (part == "\r\n" || part == "\n")
                {
                    RunAdbCommand(new[] { "shell", "am", "broadcast", "-a", "ADB_INPUT_CODE", "--ei", "code", "66" }, deviceName);
                }
                else if (char.IsSurrogatePair(part, 0) || part.Any(c => char.GetUnicodeCategory(c) == System.Globalization.UnicodeCategory.OtherSymbol))
                {
                    // 对于表情符号和其他特殊Unicode字符，使用 ADB_INPUT_CHARS 命令
                    string unicodeValues = string.Join(",", part.Select(c => ((int)c).ToString()));
                    RunAdbCommand(new[] { "shell", "am", "broadcast", "-a", "ADB_INPUT_CHARS", "--eia", "chars", unicodeValues }, deviceName);
                }
                else
                {
                    // 对于其他字符（包括中文），使用 ADB_INPUT_TEXT 命令
                    RunAdbCommand(new[] { "shell", "am", "broadcast", "-a", "ADB_INPUT_TEXT", "--es", "msg", part }, deviceName);
                }
            }

            Console.WriteLine($"Successfully input text: {text}");
            SwitchIme(defaultIme, deviceName);
        }

        /// <summary>
        /// 执行返回操作
        /// </summary>
        public static void GoBack(string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "input", "keyevent", "KEYCODE_BACK"  }, deviceName);
            Console.WriteLine("Successfully executed back operation");
        }

        /// <summary>
        /// 执行滑动操作
        /// </summary>
        /// <param name="direction">滑动方向，up 为上滑，down 为下滑</param>
        /// <param name="startX">起始点 X 坐标</param>
        /// <param name="startY">起始点 Y 坐标</param>
        /// <param name="moveY">Y 轴移动的距离</param>
        /// <param name="duration">滑动持续时间（毫秒）</param>
        public static void SwipeVertical(int startX, int startY, int moveY, int duration, string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "input", "swipe", startX.ToString(), startY.ToString(), startX.ToString(), (startY + moveY).ToString(), duration.ToString()  }, deviceName);
        }
        public static void SwipeHorizontal(int startX, int startY, int moveX, int duration, string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "input", "swipe", startX.ToString(), startY.ToString(), (startX + moveX).ToString(), (startY).ToString(), duration.ToString()  }, deviceName);
        }

        /// <summary>
        /// 执行长按操作
        /// </summary>
        /// <param name="x">长按位置的 X 坐标</param>
        /// <param name="y">长按位置的 Y 坐标</param>
        /// <param name="duration">长按持续时间（毫秒），默认为 1000 毫秒</param>
        public static void LongPress(int x, int y, int duration = 1000, string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "input", "swipe", x.ToString(), y.ToString(), x.ToString(), y.ToString(), duration.ToString()  }, deviceName);
            Console.WriteLine("Successfully executed long press operation");
        }

        /// <summary>
        /// 执行点击操作
        /// </summary>
        /// <param name="x">点击位置的 X 坐标</param>
        /// <param name="y">点击位置的 Y 坐标</param>
        public static void Click(int x, int y, string deviceName = "")
        {
            RunAdbCommand(new[] { "shell", "input", "tap", x.ToString(), y.ToString()  }, deviceName);
            Console.WriteLine($"Successfully executed click operation at ({x}, {y})");
        }

        /// <summary>
        /// 获取连接的 ADB 设备列表
        /// </summary>
        /// <returns>设备序列号数组</returns>
        public static string[] GetAdbDeviceList()
        {
            string output = RunAdbCommandWithOutput(new[] { "devices"  });
            var devices = Regex.Matches(output, @"(\S+)\s+device");
            return devices.Count > 0 ? devices.Cast<Match>().Where(m => m.Groups[1].Value != "of").Select(m => m.Groups[1].Value).ToArray() : Array.Empty<string>();
        }


        /// <summary>
        /// 生成随机文件名
        /// </summary>
        /// <param name="length">文件名长度，默认为 10</param>
        /// <returns>随机文件名</returns>
        private static Random random = new Random();
        public static string GenerateRandomFilename(int length = 10)
        {
            const string chars = "abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
            return new string(Enumerable.Repeat(chars, length).Select(s => s[random.Next(s.Length)]).ToArray()) + ".png";
        }

        /// <summary>
        /// 截取屏幕并保存到本地
        /// </summary>
        /// <returns>截图文件路径</returns>
        /// <summary>
        /// 截取屏幕并保存到指定本地目录
        /// </summary>
        /// <param name="filename">截图文件名,为空则自动生成随机文件名</param>
        /// <param name="localDir">本地保存截图的目录路径</param>
        /// <returns>截图文件在本地的完整路径</returns>
        public static string Screenshot(string filename = null, string localDir = null,string deviceName="")
        {
            if (string.IsNullOrEmpty(filename))
                filename = GenerateRandomFilename();

            if (string.IsNullOrEmpty(localDir))
                localDir = Path.Combine(Directory.GetCurrentDirectory(), "File");

            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            string localFilePath = Path.Combine(localDir, filename);

            RunAdbCommand(new[] { "shell", "screencap", "-p", $"/sdcard/{filename}"  }, deviceName);
            RunAdbCommand(new[] { "pull", $"/sdcard/{filename}", localFilePath  }, deviceName);

            return localFilePath;
        }
    }
}