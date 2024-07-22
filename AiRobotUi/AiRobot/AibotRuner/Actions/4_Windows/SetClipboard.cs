using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAPI;

namespace Aibot
{
    [AibotItem("窗口-复制内容", ActionType = ActionType.WindowsServer)]
    public class SetClipboard : BaseAibotAction,IAibotAction
    {
        [AibotProperty("内容(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var text = Text.Value?.ToString() ?? "";
            Lazy:
            if (!WinAPI.OpenClipboard(IntPtr.Zero))
            {
                goto Lazy;
            }

            WinAPI.EmptyClipboard();
            WinAPI.SetClipboardData(13, Marshal.StringToHGlobalUni(text));
            WinAPI.CloseClipboard();

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-粘贴", ActionType = ActionType.WindowsServer)]
    public class Paste : BaseAibotAction, IAibotAction
    {
        public new Task Execute(AibotV blackboard)
        {
            SendKeys.SendWait("^V");
            return Task.CompletedTask;
        }
    }
}
