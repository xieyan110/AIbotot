using Nodify;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;


namespace Aibot
{

    [AibotItem("窗口-命令行", ActionType = ActionType.WindowsServer)]
    public class RunCommandLineProgram : BaseAibotAction, IAibotAction
    {
        [AibotProperty("参数(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Arguments { get; set; }

        [AibotProperty("结果(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            Arguments.Value?.ToString();

            string command = Arguments.Value?.ToString();
            var result = "";
            if (!string.IsNullOrEmpty(command))
            {
                string[] parts = command.Split(' ');
                if (parts.Length > 0)
                {
                    string executable = parts[0];
                    string arguments = string.Join(" ", parts.Skip(1));
                    result = CommandLineHelper.RunCommandLineProgram(executable, arguments);

                    Console.WriteLine($"Executable: {executable}");
                    Console.WriteLine($"Arguments: {arguments}");
                }
            }

            blackboard.Node.Output.ForEach(x =>
            {
                if (x.PropertyName == "Result")
                    x.Value = result;
            });
            return Task.CompletedTask;
        }
    }
    [AibotItem("窗口-打开", ActionType = ActionType.WindowsServer)]
    public class Open : BaseAibotAction,IAibotAction
    {
        [AibotProperty("文件路径/网址(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty Url { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            Task.Run(() => 
                Process.Start("explorer.exe",Url.Value?.ToString() ?? "")
            );
            return Task.CompletedTask;
        }
    }
}
