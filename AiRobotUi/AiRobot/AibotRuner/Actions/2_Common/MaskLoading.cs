using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aibot.AibotRuner.Actions._2_Common
{
    [AibotItem("遮罩-加载", ActionType = ActionType.WindowsServer)]
    public class MaskLoading : BaseAibotAction, IAibotAction
    {
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }

        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        [AibotProperty("宽度(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Width { get; set; }

        [AibotProperty("高度(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Height { get; set; }

        [AibotProperty("背景颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty BackgroundColor { get; set; }

        [AibotProperty("文字颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty TextColor { get; set; }

        [AibotProperty("加载文本(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty LoadingText { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            CustomOverlayManager.ClearLog();

            CustomOverlayManager.Show();
            CustomOverlayManager.SetTopmost(true);
            CustomOverlayManager.ForceTopmost();
            CustomOverlayManager.SetShowLogs(false);
            CustomOverlayManager.SetSize(Width.Value.TryInt(), Height.Value.TryInt());
            CustomOverlayManager.SetPosition(X.Value.TryInt(), Y.Value.TryInt());
            CustomOverlayManager.SetBackgroundColor(BackgroundColor.Value.ToString());
            CustomOverlayManager.SetTextColor(TextColor.Value.ToString());
            CustomOverlayManager.StartLoadingAnimation(LoadingText.Value.ToString());

            return Task.CompletedTask;
        }
    }

    [AibotItem("遮罩-日志", ActionType = ActionType.WindowsServer)]
    public class MaskLog : BaseAibotAction, IAibotAction
    {
        [AibotProperty("X坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }

        [AibotProperty("Y坐标(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        [AibotProperty("宽度(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Width { get; set; }

        [AibotProperty("高度(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Height { get; set; }

        [AibotProperty("背景颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty BackgroundColor { get; set; }

        [AibotProperty("文字颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty TextColor { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            CustomOverlayManager.StopLoadingAnimation();
            CustomOverlayManager.ClearLog();

            CustomOverlayManager.Show();
            CustomOverlayManager.SetTopmost(true);
            CustomOverlayManager.ForceTopmost();
            CustomOverlayManager.SetTitle("【日志】");
            CustomOverlayManager.SetSize(Width.Value.TryInt(), Height.Value.TryInt());
            CustomOverlayManager.SetPosition(X.Value.TryInt(), Y.Value.TryInt());
            CustomOverlayManager.SetBackgroundColor(BackgroundColor.Value.ToString());
            CustomOverlayManager.SetTextColor(TextColor.Value.ToString());
            CustomOverlayManager.SetShowLogs(true);  // 显示日志

            return Task.CompletedTask;
        }
    }

    [AibotItem("遮罩-关闭", ActionType = ActionType.WindowsServer)]
    public class MaskClose : BaseAibotAction, IAibotAction
    {
        public new Task Execute(AibotV blackboard)
        {
            CustomOverlayManager.CloseInstance();
            return Task.CompletedTask;
        }
    }
}
