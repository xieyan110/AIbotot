using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAPI;
using Wpf.Ui.Controls;


namespace Aibot
{
    [AibotItem("窗口-键盘输入", ActionType = ActionType.WindowsServer)]
    public class SendKey : BaseAibotAction,IAibotAction
    {
        [AibotProperty("文本(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            SendKeys.SendWait(Text.Value?.ToString() ?? "");
            return Task.CompletedTask;
        }
    }
}
