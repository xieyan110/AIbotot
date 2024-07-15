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
using System.Drawing;

namespace Aibot
{
    [AibotItem("鼠标移动", ActionType = ActionType.WindowsServer)]
    public class MouseMove : BaseAibotAction,IAibotAction
    {
        [AibotProperty("X(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var x = X.Value?.TryInt() ?? 0;
            var y = Y.Value?.TryInt() ?? 0;

            Mouse.Move(x, y);
            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标简易拖动", ActionType = ActionType.WindowsServer)]
    public class MouseELeftDrag : BaseAibotAction, IAibotAction
    {
        [AibotProperty("坐标1(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Point1 { get; set; }

        [AibotProperty("坐标2(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Point2 { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var a = Point1.Value.ToString().Split(" × ");
            var b = Point2.Value.ToString().Split(" × ");
            var point1 = new Point(a[0].TryInt(), a[1].TryInt());
            var point2 = new Point(b[0].TryInt(), b[1].TryInt());

            MouseDrag.LeftDrag(point1, point2, 50, 15);

            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标拖动", ActionType = ActionType.WindowsServer)]
    public class MouseLeftDrag : BaseAibotAction, IAibotAction
    {
        [AibotProperty("StarX(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty StarX { get; set; }
        [AibotProperty("StarY(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty StarY { get; set; }
        
        [AibotProperty("EndX(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty EndX { get; set; }
        [AibotProperty("EndY(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty EndY { get; set; }

        [AibotProperty("间隔毫秒(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Lag { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var point1 = new Point(StarX.Value.TryInt(), StarY.Value.TryInt());
            var point2 = new Point(EndX.Value.TryInt(), EndY.Value.TryInt());

            MouseDrag.LeftDrag(point1, point2, 100, Lag.Value.TryInt());

            return Task.CompletedTask;
        }
    }
}
