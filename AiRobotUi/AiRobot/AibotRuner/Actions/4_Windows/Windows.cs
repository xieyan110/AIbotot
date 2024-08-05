using System;
using System.Linq;
using System.Threading.Tasks;
using System.Speech.Synthesis;
using Nodify;
using WindowsAPI;
using System.IO;

namespace Aibot
{

    [AibotItem("窗口-桌面截图", ActionType = ActionType.WindowsServer)]
    public class ScreenshotDesktop : BaseAibotAction, IAibotAction
    {

        [AibotProperty("文件路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty FilePath { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var bmp = Desktop.Screenshot();

            var localDir = Path.Combine(Directory.GetCurrentDirectory(), "File");

            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            string savePath = Path.Combine(localDir, "Desktop.png");

            bmp.Save(savePath);

            blackboard[FilePath] = savePath;
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("FilePath" == n.PropertyName)
                    n.Value = savePath;
            });
            return Task.CompletedTask;
        }
    }


    [AibotItem("窗口-焦点句柄", ActionType = ActionType.WindowsServer)]
    public class WindowsGetFocused : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty IntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = Window.GetFocused();

            blackboard.Node!.Output.ForEach(x =>
            {
                if (x.PropertyName == "IntPtr")
                    x.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-通过标题获取焦点", ActionType = ActionType.WindowsServer)]
    public class WindowsTitleGetFocused : IF, IAibotAction
    {

        [AibotProperty("窗口标题(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Title { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty IntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            IntPtr? intPtr = null;
            try
            {
                intPtr = Window.Get(Title.Value?.ToString() ?? "");
                (blackboard["IsSuccess"], blackboard["IsError"]) = (true, false);
            }
            catch
            {
                (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);
            }

            blackboard.Node!.Output.ForEach(x =>
            {
                if (x.PropertyName == "IntPtr")
                    x.Value = intPtr;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-标题获取", ActionType = ActionType.WindowsServer)]
    public class WindowsGetTitle : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("标题(String)", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty Title { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var title = Window.GetTitle(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if (n.PropertyName == "Title")
                {
                    n.Value = title;
                }
                if (n.PropertyName == "OutIntPtr")
                {
                    n.Value = intPtr;
                }
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-标题更改", ActionType = ActionType.WindowsServer)]
    public class WindowsSetTitle : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("Title(String)", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Title { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var title = Title.Value!.ToString();
            Window.SetTitle(intPtr, title);

            blackboard.Node!.Output.ForEach(n =>
            {
                if (n.PropertyName == "OutIntPtr")
                {
                    n.Value = intPtr;
                }
            });

            return Task.CompletedTask;
        }
    }


    [AibotItem("窗口-移动", ActionType = ActionType.WindowsServer)]
    public class WindowsMove : BaseAibotAction, IAibotAction
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
            var intPtr = IntPtr.Value!.CastTo<IntPtr>();
            var x = X.Value!.TryInt();
            var y = Y.Value!.TryInt();

            Window.Move(intPtr, x, y);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-调整大小", ActionType = ActionType.WindowsServer)]
    public class WindowsResize : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("宽度(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Width { get; set; }

        [AibotProperty("高度(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Height { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.CastTo<IntPtr>();
            var width = Width.Value!.TryInt();
            var height = Height.Value!.TryInt();

            Window.Resize(intPtr, width, height);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-最大化", ActionType = ActionType.WindowsServer)]
    public class WindowsMaximize : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();

            Window.Maximize(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-最小化", ActionType = ActionType.WindowsServer)]
    public class WindowsMinimize : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();

            Window.Minimize(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口化", ActionType = ActionType.WindowsServer)]
    public class WindowsNormalize : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();

            Window.Normalize(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-应用截图", ActionType = ActionType.WindowsServer)]
    public class WindowsScreenshot : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("文件名", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FileName { get; set; }

        [AibotProperty("文件路径", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty FilePath { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();
            var fileName = FileName.Value?.ToString() ?? "Window.png";

            var bmp = WindowCapture.CaptureWindow(intPtr);
            var localDir = Path.Combine(Directory.GetCurrentDirectory(), "File");

            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            string savePath = Path.Combine(localDir, fileName);

            bmp.Save(savePath);

            blackboard[FilePath] = savePath;
            blackboard.Node!.Output.ForEach(n =>
            {
                if ("FilePath" == n.PropertyName)
                    n.Value = savePath;
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-关闭", ActionType = ActionType.WindowsServer)]
    public class WindowsClose : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();

            Window.Close(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-左反转", ActionType = ActionType.WindowsServer)]
    public class WindowsFlipLeft : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();

            Window.FlipLeft(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-右反转", ActionType = ActionType.WindowsServer)]
    public class WindowsFlipRight : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.Case<IntPtr>();

            Window.FlipRight(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }



    [AibotItem("窗口-桌面高宽", ActionType = ActionType.WindowsServer)]
    public class WindowsWidthAndHeight : BaseAibotAction, IAibotAction
    {
        [AibotProperty("宽(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Output)]
        public AibotProperty Width { get; set; }
        [AibotProperty("高(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Output)]
        public AibotProperty Height { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            blackboard.Node!.Output.ForEach(x =>
            {
                if (x.PropertyName == "Width")
                    x.Value = Desktop.GetWidth();
                if (x.PropertyName == "Height")
                    x.Value = Desktop.GetHeight();

            });
            return Task.CompletedTask;
        }
    }


    [AibotItem("窗口-隐藏", ActionType = ActionType.WindowsServer)]
    public class WindowsHide : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.CastTo<IntPtr>();
            Window.Hide(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("窗口-显示", ActionType = ActionType.WindowsServer)]
    public class WindowsShow : BaseAibotAction, IAibotAction
    {
        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty IntPtr { get; set; }

        [AibotProperty("IntPtr", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutIntPtr { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var intPtr = IntPtr.Value!.CastTo<IntPtr>();
            Window.Show(intPtr);

            blackboard.Node!.Output.ForEach(n =>
            {
                if ("OutIntPtr" == n.PropertyName)
                    n.Value = intPtr;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("任务栏-藏任", ActionType = ActionType.WindowsServer)]
    public class WindowsHideTaskBar : BaseAibotAction, IAibotAction
    {
        public new Task Execute(AibotV blackboard)
        {
            Desktop.HideTaskBar();
            return Task.CompletedTask;
        }
    }

    [AibotItem("任务栏-显示", ActionType = ActionType.WindowsServer)]
    public class WindowsShowTaskBar : BaseAibotAction, IAibotAction
    {
        public new Task Execute(AibotV blackboard)
        {
            Desktop.ShowTaskBar();
            return Task.CompletedTask;
        }
    }
}
