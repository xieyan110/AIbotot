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
    public class CommandLineHelper
    {
        public static string RunCommandLineProgram(string programName, string arguments = "")
        {
            try
            {
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = programName ?? "",
                        Arguments = arguments ?? "",
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

        public static string RunPythonScript(string scriptPath, string arguments = "")
        {
            try
            {
                // 不需要指定Python解释器路径,直接使用"python"
                var process = new Process
                {
                    StartInfo = new ProcessStartInfo
                    {
                        FileName = "python",
                        Arguments = $"\"{scriptPath}\" {arguments}",
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

        public static string ExecutePythonCode(string pythonCode, string fileName, string arguments = "")
        {
            try
            {
                // 创建 PythonScript 文件夹(如果不存在)
                string folderPath = Path.Combine(Directory.GetCurrentDirectory(), "PythonScript");
                Directory.CreateDirectory(folderPath);

                // 将 Python 代码保存到 main.py 文件
                string filePath = Path.Combine(folderPath, $"{fileName}.py");
                File.WriteAllText(filePath, pythonCode);

                // 执行 Python 脚本
                return RunPythonScript(filePath, arguments);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return string.Empty;
            }
        }


    }

}