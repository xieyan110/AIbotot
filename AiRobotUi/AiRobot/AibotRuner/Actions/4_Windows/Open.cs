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
    [AibotItem("打开资源", ActionType = ActionType.WindowsServer)]
    public class Open : BaseAibotAction,IAibotAction
    {
        [AibotProperty("连接(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
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
