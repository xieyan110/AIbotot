using Nodify;
using OpenCvSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;
using WindowsAPI;
using Wpf.Ui.Controls;


namespace Aibot
{
    [AibotItem("Adb设备列表", ActionType = ActionType.AndroidServer)]
    public class AdbGetAdbDeviceList : BaseAibotAction, IAibotAction
    {

        [AibotProperty("设备(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Device { get; set; }

        public new Task Execute(AibotV blackboard)
        {

            var device = string.Join(", ", AdbHelper.GetAdbDeviceList());

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Device")
                    node.Value = device;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("AdbWifi调试", ActionType = ActionType.AndroidServer)]
    public class AdbDebugOverWifi : IF, IAibotAction
    {

        [AibotProperty("设备IP(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty DeviceIP { get; set; }

        public new Task Execute(AibotV blackboard)
        {

            var isSuccess = AdbHelper.RemoteDebugOverWifi(DeviceIP.Value?.ToString() ?? "");

            if(isSuccess)
                (blackboard["IsSuccess"], blackboard["IsError"]) = (true, false);
            else
                (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);

            return Task.CompletedTask;
        }
    }
    
    [AibotItem("Adb投屏", ActionType = ActionType.AndroidServer)]
    public class AdbDebugRunScrcpy : BaseAibotAction, IAibotAction
    {
        [AibotProperty("设备[必填](String)", AibotKeyType.Object, IsRoot = false, Usage = AibotKeyUsage.Input)]
        public AibotProperty DeviceName { get; set; }

        public new Task Execute(AibotV blackboard)
        {

            AdbHelper.RunScrcpy(DeviceName.Value?.ToString() ?? "");

            return Task.CompletedTask;
        }
    }

    [AibotItem("Adb截取屏幕", ActionType = ActionType.AndroidServer)]
    public class AdbScreenshot : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("文件名[空为随机](String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FileName { get; set; }

        [AibotProperty("文件路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty FilePath { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";
            var filePath = AdbHelper.Screenshot(FileName.Value?.ToString() ?? "",null, deviceName);

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "FilePath")
                    node.Value = filePath;
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("Adb打开应用", ActionType = ActionType.AndroidServer)]
    public class AdbOpenApp : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("包名(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty PackageName { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            AdbHelper.OpenApp(PackageName.Value?.ToString() ?? "", deviceName);
            return Task.CompletedTask;
        }
    }
    [AibotItem("Adb关闭应用", ActionType = ActionType.AndroidServer)]
    public class AdbKillApp : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("包名(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty PackageName { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            AdbHelper.KillApp(PackageName.Value?.ToString() ?? "", deviceName);
            return Task.CompletedTask;
        }
    }

 
    [AibotItem("Adb点击", ActionType = ActionType.AndroidServer)]
    public class AdbClick : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }

        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        [AibotProperty("点击次数(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Count { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";


            var x = X.Value?.TryInt() ?? 0;
            var y = Y.Value?.TryInt() ?? 0;

            var cout = Count.Value?.TryInt();
            cout = cout == 0 ? 1 : cout;
            for (int i = 0; cout > i; i++)
            {
                AdbHelper.Click(x, y, deviceName);
                Thread.Sleep(50);
            }
            return Task.CompletedTask;
        }
    }
    [AibotItem("Adb长按操作", ActionType = ActionType.AndroidServer)]
    public class AdbLongPress : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }

        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        [AibotProperty("时长(毫秒)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Duration { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            var x = X.Value?.TryInt() ?? 0;
            var y = Y.Value?.TryInt() ?? 0;
            var duration = Duration.Value?.TryInt() ?? 0;
            if (duration == 0)
                duration = 500;

            AdbHelper.LongPress(x, y, duration, deviceName);
            return Task.CompletedTask;
        }
    }
    [AibotItem("Adb返回操作", ActionType = ActionType.AndroidServer)]
    public class AdbGoBack : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("次数(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Count { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            var count = Count.Value?.TryInt() ?? 0;
            count = count == 0 ? 1 : count;
            for (int i = 0; i < count; i++)
            {
                AdbHelper.GoBack(deviceName);
            }
            return Task.CompletedTask;
        }
    }
    [AibotItem("Adb键盘输入", ActionType = ActionType.AndroidServer)]
    public class AdbInputText : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("文本(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            AdbHelper.InputText(Text.Value?.ToString() ?? "", deviceName);
            return Task.CompletedTask;
        }
    }
    [AibotItem("Adb检测应用安装", ActionType = ActionType.AndroidServer)]
    public class AdbIsAppInstalled : IF, IAibotAction
    {
        [AibotProperty("设备[可为空](String)", AibotKeyType.Object, IsRoot = false, Usage = AibotKeyUsage.Input)]
        public AibotProperty DeviceName { get; set; }

        [AibotProperty("包名(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty PackageName { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            var isInstall = AdbHelper.IsAppInstalled(PackageName.Value?.ToString() ?? "");
            if (!isInstall)
                (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);
            else
                (blackboard["IsSuccess"], blackboard["IsError"]) = (true, false);

            return Task.CompletedTask;
        }
    }

    [AibotItem("Adb安装应用", ActionType = ActionType.AndroidServer)]
    public class AdbInstallApp : BaseAdbAibotAction, IAibotAction
    {
        [AibotProperty("Apk文件路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApkPath { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            AdbHelper.InstallApp(ApkPath.Value?.ToString() ?? "", deviceName);

            return Task.CompletedTask;
        }
    }

    [AibotItem("Adb纵向滑动", ActionType = ActionType.AndroidServer)]
    public class AdbVerticalSwipe : BaseAdbAibotAction, IAibotAction
    {

        [AibotProperty("起始X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty StartX { get; set; }

        [AibotProperty("起始Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty StartY { get; set; }

        [AibotProperty("Y移动距离[上正下负](Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty MoveY { get; set; }
        
        [AibotProperty("滑动时间(毫秒)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Duration { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            var startX = StartX.Value?.TryInt() ?? 0;
            var startY = StartY.Value?.TryInt() ?? 0;
            var moveY = MoveY.Value?.TryInt() ?? 0;
            var duration = Duration.Value?.TryInt() ?? 0;
            duration = duration == 0 ? 500 : duration;


            AdbHelper.SwipeVertical(startX, startY, moveY, duration, deviceName);
            return Task.CompletedTask;
        }
    }

    [AibotItem("Adb横向滑动", ActionType = ActionType.AndroidServer)]
    public class AdbHorizontalSwipe : BaseAdbAibotAction, IAibotAction
    {

        [AibotProperty("起始X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty StartX { get; set; }

        [AibotProperty("起始Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty StartY { get; set; }

        [AibotProperty("X移动距离[左正右负](Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty MoveX { get; set; }

        [AibotProperty("滑动时间(毫秒)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Duration { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var deviceName = DeviceName.Value?.ToString() ?? "";

            var startX = StartX.Value?.TryInt() ?? 0;
            var startY = StartY.Value?.TryInt() ?? 0;
            var moveX = MoveX.Value?.TryInt() ?? 0;
            var duration = Duration.Value?.TryInt() ?? 0;
            duration = duration == 0 ? 500 : duration;


            AdbHelper.SwipeHorizontal(startX, startY, moveX, duration, deviceName);
            return Task.CompletedTask;
        }
    }
}
