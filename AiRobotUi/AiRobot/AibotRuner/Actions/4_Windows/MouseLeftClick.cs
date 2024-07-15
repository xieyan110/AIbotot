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
    [AibotItem("鼠标左击", ActionType = ActionType.WindowsServer)]
    public class MouseLeftClick : BaseAibotAction,IAibotAction
    {
        [AibotProperty("点击次数(Int)", AibotKeyType.Integer, Usage=AibotKeyUsage.Input)]
        public AibotProperty Count { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var cout = Count.Value?.TryInt();
            cout = cout == 0 ? 1 : cout;
            for (int i = 0;cout > i; i++)
            {
                Mouse.LeftClick();
                Thread.Sleep(50);
            }
            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标左击(高级)", ActionType = ActionType.WindowsServer)]
    public class MouseClick : BaseAibotAction, IAibotAction
    {

        [AibotProperty("X(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        [AibotProperty("点击次数(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Count { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var x = X.Value?.TryInt() ?? 0;
            var y = Y.Value?.TryInt() ?? 0;

            Mouse.Move(x, y);

            var cout = Count.Value?.TryInt();
            cout = cout == 0 ? 1 : cout;
            for (int i = 0; cout > i; i++)
            {
                Mouse.LeftClick();
                Thread.Sleep(50);
            }
            return Task.CompletedTask;
        }
    }
}
