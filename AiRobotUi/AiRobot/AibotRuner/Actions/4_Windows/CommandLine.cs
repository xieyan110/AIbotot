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


    [AibotItem("Py-文件执行", ActionType = ActionType.WindowsServer)]
    public class RunPythonScript : BaseAibotAction, IAibotAction
    {
        [AibotProperty("脚本文件(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ScriptPath { get; set; }

        [AibotProperty("参数(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Arguments { get; set; }

        [AibotProperty("结果(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var result = CommandLineHelper.RunPythonScript(ScriptPath.Value?.ToString(), Arguments.Value?.ToString());

            blackboard.Node.Output.ForEach(x =>
            {
                if (x.PropertyName == "Result")
                    x.Value = result;
            });
            return Task.CompletedTask;
        }
    }
    [AibotItem("Py-代码执行", ActionType = ActionType.WindowsServer)]
    public class ExecutePythonCode : BaseAibotAction, IAibotAction
    {
        [AibotProperty("代码(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Script { get; set; }

        [AibotProperty("名称[默认main]", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ScriptName { get; set; }


        [AibotProperty("参数(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Arguments { get; set; }

        [AibotProperty("结果(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var fileName = ScriptName.Value?.ToString() ?? "main";

            var result = CommandLineHelper.ExecutePythonCode(Script.Value?.ToString(), fileName, Arguments.Value?.ToString());

            blackboard.Node.Output.ForEach(x =>
            {
                if (x.PropertyName == "Result")
                    x.Value = result;
            });
            return Task.CompletedTask;
        }


    }
}
