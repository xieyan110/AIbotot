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
using Nodify;

namespace Aibot
{
    [AibotItem("鼠标-移动", ActionType = ActionType.WindowsServer)]
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
    [AibotItem("鼠标-左击", ActionType = ActionType.WindowsServer)]
    public class MouseLeftClick : BaseAibotAction, IAibotAction
    {
        [AibotProperty("点击次数(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Count { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var cout = Count.Value?.TryInt();
            cout = cout == 0 ? 1 : cout;
            for (int i = 0; cout > i; i++)
            {
                MouseKeyController.MouseLeftClick();
                Thread.Sleep(50);
            }
            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标-左击(高级)", ActionType = ActionType.WindowsServer)]
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

    [AibotItem("鼠标-滚动", ActionType = ActionType.WindowsServer)]
    public class MouseLeftClickWheel : BaseAibotAction, IAibotAction
    {
        [AibotProperty("滚动[正上负下](Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Offset { get; set; }

        public new Task Execute(AibotV blackboard)
        {

            MouseKeyController.MoveMouseWheel(Offset.Value?.TryInt() ?? 0);
            

            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标-右击", ActionType = ActionType.WindowsServer)]
    public class MouseRightClick : BaseAibotAction, IAibotAction
    {
        [AibotProperty("点击次数(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Count { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var cout = Count.Value?.TryInt();
            cout = cout == 0 ? 1 : cout;
            for (int i = 0; cout > i; i++)
            {
                Mouse.RightClick();
                Thread.Sleep(50);
            }
            return Task.CompletedTask;
        }
    }
    [AibotItem("鼠标-简易拖动", ActionType = ActionType.WindowsServer)]
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

    [AibotItem("鼠标-拖动", ActionType = ActionType.WindowsServer)]
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
    [AibotItem("鼠标-[D]左键点击", ActionType = ActionType.WindowsServer)]
    public class SendMouseLeftClick : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }

        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var x = X.Value!.TryInt();
            var y = Y.Value!.TryInt();
            MouseControl.LeftClick(intPtr, x, y);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }
    [AibotItem("鼠标-[D]右键点击", ActionType = ActionType.WindowsServer)]
    public class SendMouseRightClick : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var x = X.Value!.TryInt();
            var y = Y.Value!.TryInt();
            MouseControl.RightClick(intPtr, x, y);
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标-[D]中键点击", ActionType = ActionType.WindowsServer)]
    public class SendMouseMiddleClick : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var x = X.Value!.TryInt();
            var y = Y.Value!.TryInt();
            MouseControl.MiddleClick(intPtr, x, y);
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标-[D]滚轮", ActionType = ActionType.WindowsServer)]
    public class SendMouseScroll : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }
        [AibotProperty("滚动量(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Delta { get; set; }
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var x = X.Value!.TryInt();
            var y = Y.Value!.TryInt();
            var delta = Delta.Value!.TryInt();
            MouseControl.Scroll(intPtr, x, y, delta);
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标-[D]移动到指定位置", ActionType = ActionType.WindowsServer)]
    public class SendMouseMoveTo : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var x = X.Value!.TryInt();
            var y = Y.Value!.TryInt();
            MouseControl.MoveTo(intPtr, x, y);
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("鼠标-[D]相对移动", ActionType = ActionType.WindowsServer)]
    public class SendMouseMoveRelative : BaseAibotAction, IAibotAction
    {
        [AibotProperty("X偏移量(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty DeltaX { get; set; }

        [AibotProperty("Y偏移量(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty DeltaY { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deltaX = DeltaX.Value!.TryInt();
            var deltaY = DeltaY.Value!.TryInt();
            MouseControl.MoveRelative(deltaX, deltaY);
            return Task.CompletedTask;
        }
    }

}
