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
    [AibotItem("鼠标右击", ActionType = ActionType.WindowsServer)]
    public class MouseRightClick : BaseAibotAction,IAibotAction
    {
        [AibotProperty("点击次数(Int)", AibotKeyType.Integer, Usage=AibotKeyUsage.Input)]
        public AibotProperty Count { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var cout = Count.Value?.TryInt();
            cout = cout == 0 ? 1 : cout;
            for (int i = 0;cout > i; i++)
            {
                Mouse.RightClick();
                Thread.Sleep(50);
            }
            return Task.CompletedTask;
        }
    }
}
