using Nodify;
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
    [AibotItem("窗口-[高级]键盘输入", ActionType = ActionType.WindowsServer)]
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

    [AibotItem("窗口-[D]游戏键盘输入", ActionType = ActionType.WindowsServer)]
    public class SendInputString : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }
        [AibotProperty("按钮组(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            InputSimulator.ExecuteInputSequence(intPtr, Text.Value?.ToString() ?? "");
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-[D]键盘按下", ActionType = ActionType.WindowsServer)]
    public class SendKeyUp : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }
        [AibotProperty("键(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            KeyboardControl.KeyDown(intPtr, Text.Value?.ToString() ?? "");
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }
    [AibotItem("窗口-[D]键盘松开", ActionType = ActionType.WindowsServer)]
    public class SendKeyDown : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }
        [AibotProperty("键(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            KeyboardControl.KeyUp(intPtr, Text.Value?.ToString() ?? "");
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

}
