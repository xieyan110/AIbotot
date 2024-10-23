using Microsoft.Playwright;
using Nodify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.Json;
using System.Threading.Tasks;


namespace Aibot
{
    [AibotItem("Pl启动浏览器", ActionType = ActionType.WindowsServer)]
    public class PlaywrightLaunchBrowser : BaseAibotAction, IAibotAction
    {
        [AibotProperty("浏览器类型[chrome/firefox/webkit]", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty BrowserType { get; set; }

        [AibotProperty("浏览器路径", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty BrowserPath { get; set; }

        [AibotProperty("是否无头模式", AibotKeyType.Boolean, Usage = AibotKeyUsage.Input)]
        public AibotProperty Headless { get; set; }

        [AibotProperty("操作延迟(ms)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty SlowMo { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var config = new PlaywrightHelper.BrowserConfig
            {
                Type = BrowserType.Value?.ToString()?.ToLower() switch
                {
                    "firefox" => PlaywrightHelper.BrowserType.Firefox,
                    "webkit" => PlaywrightHelper.BrowserType.Webkit,
                    _ => PlaywrightHelper.BrowserType.Chromium
                },
                ExecutablePath = BrowserPath.Value?.ToString(),
                Headless = Headless.Value?.CastTo<bool>() ?? false,
                SlowMo = SlowMo.Value?.TryInt() ?? 0
            };

            await PlaywrightHelper.Instance.InitializeBrowser(config);
        }
    }


    [AibotItem("Pl导航", ActionType = ActionType.WindowsServer)]
    public class PlaywrightNavigate : BaseAibotAction, IAibotAction
    {
        [AibotProperty("URL", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Url { get; set; }

        [AibotProperty("超时时间(ms)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Timeout { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.NavigateToUrl(
                Url.Value?.ToString() ?? "",
                Timeout.Value?.TryInt() ?? 30000
            );
        }
    }

    [AibotItem("Pl点击", ActionType = ActionType.WindowsServer)]
    public class PlaywrightClick : BaseAibotAction, IAibotAction
    {
        [AibotProperty("选择器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Selector { get; set; }

        [AibotProperty("点击次数", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty ClickCount { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var count = ClickCount.Value?.TryInt() ?? 1;
            for (int i = 0; i < count; i++)
            {
                await PlaywrightHelper.Instance.Click(Selector.Value?.ToString() ?? "");
                await Task.Delay(50);
            }
        }
    }

    [AibotItem("Pl输入文本", ActionType = ActionType.WindowsServer)]
    public class PlaywrightInputText : BaseAibotAction, IAibotAction
    {
        [AibotProperty("选择器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Selector { get; set; }

        [AibotProperty("文本", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        [AibotProperty("输入延迟(ms)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Delay { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.Fill(
                Selector.Value?.ToString() ?? "",
                Text.Value?.ToString() ?? "",
                Delay.Value?.TryInt() ?? 0
            );
        }
    }

    [AibotItem("Pl执行JS", ActionType = ActionType.WindowsServer)]
    public class PlaywrightExecuteJs : BaseAibotAction, IAibotAction
    {
        [AibotProperty("JS代码", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Script { get; set; }

        [AibotProperty("返回结果", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var result = await PlaywrightHelper.Instance.ExecuteJavaScript<object>(
                Script.Value?.ToString() ?? ""
            );

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Result")
                    node.Value = result?.ToString();
            });
        }
    }

    [AibotItem("Pl截图", ActionType = ActionType.WindowsServer)]
    public class PlaywrightScreenshot : BaseAibotAction, IAibotAction
    {
        [AibotProperty("保存路径", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Path { get; set; }

        [AibotProperty("是否全页面", AibotKeyType.Boolean, Usage = AibotKeyUsage.Input)]
        public AibotProperty FullPage { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.Screenshot(
                Path.Value?.ToString() ?? $"screenshot_{DateTime.Now:yyyyMMddHHmmss}.png",
                new PlaywrightHelper.ScreenshotOptions
                {
                    FullPage = FullPage.Value?.CastTo<bool>() ?? false
                }
            );
        }
    }

    [AibotItem("Pl等待元素", ActionType = ActionType.WindowsServer)]
    public class PlaywrightWaitForElement : BaseAibotAction, IAibotAction
    {
        [AibotProperty("选择器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Selector { get; set; }

        [AibotProperty("超时时间(ms)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Timeout { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.WaitForSelector(
                Selector.Value?.ToString() ?? "",
                Timeout.Value?.TryInt() ?? 30000
            );
        }
    }

    [AibotItem("Pl获取文本", ActionType = ActionType.WindowsServer)]
    public class PlaywrightGetText : BaseAibotAction, IAibotAction
    {
        [AibotProperty("选择器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Selector { get; set; }

        [AibotProperty("文本内容", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Text { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var text = await PlaywrightHelper.Instance.GetText(Selector.Value?.ToString() ?? "");

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Text")
                    node.Value = text;
            });
        }
    }

    [AibotItem("Pl检查元素是否可见", ActionType = ActionType.WindowsServer)]
    public class PlaywrightIsElementVisible : IF, IAibotAction
    {
        [AibotProperty("选择器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Selector { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var isVisible = await PlaywrightHelper.Instance.IsElementVisible(
                Selector.Value?.ToString() ?? ""
            );

            if (isVisible)
                (blackboard["IsSuccess"], blackboard["IsError"]) = (true, false);
            else
                (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);
        }
    }


    [AibotItem("Pl设置代理", ActionType = ActionType.WindowsServer)]
    public class PlaywrightSetProxy : BaseAibotAction, IAibotAction
    {
        [AibotProperty("代理服务器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Server { get; set; }

        [AibotProperty("用户名", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Username { get; set; }

        [AibotProperty("密码", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Password { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.ConfigureProxy(new PlaywrightHelper.ProxyConfig
            {
                Server = Server.Value?.ToString(),
                Username = Username.Value?.ToString(),
                Password = Password.Value?.ToString()
            });
        }
    }

    [AibotItem("Pl鼠标悬停", ActionType = ActionType.WindowsServer)]
    public class PlaywrightHover : BaseAibotAction, IAibotAction
    {
        [AibotProperty("选择器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Selector { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.Hover(Selector.Value?.ToString() ?? "");
        }
    }

    [AibotItem("Pl按键操作", ActionType = ActionType.WindowsServer)]
    public class PlaywrightKeyPress : BaseAibotAction, IAibotAction
    {
        [AibotProperty("按键", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Key { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.Press(Key.Value?.ToString() ?? "");
        }
    }

    [AibotItem("Pl文件上传", ActionType = ActionType.WindowsServer)]
    public class PlaywrightUploadFile : BaseAibotAction, IAibotAction
    {
        [AibotProperty("选择器", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Selector { get; set; }

        [AibotProperty("文件路径", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FilePath { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.UploadFile(
                Selector.Value?.ToString() ?? "",
                FilePath.Value?.ToString() ?? ""
            );
        }
    }

    [AibotItem("Pl设置Cookie", ActionType = ActionType.WindowsServer)]
    public class PlaywrightSetCookies : BaseAibotAction, IAibotAction
    {
        [AibotProperty("Cookie JSON", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty CookieJson { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var cookies = JsonSerializer.Deserialize<List<Cookie>>(CookieJson.Value?.ToString() ?? "[]");
            await PlaywrightHelper.Instance.SetCookies(cookies);
        }
    }

    [AibotItem("Pl获取Cookie", ActionType = ActionType.WindowsServer)]
    public class PlaywrightGetCookies : BaseAibotAction, IAibotAction
    {
        [AibotProperty("Cookie JSON", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty CookieJson { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var cookies = await PlaywrightHelper.Instance.GetCookies();

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "CookieJson")
                    node.Value = JsonSerializer.Serialize(cookies);
            });
        }
    }

    [AibotItem("Pl清除Cookie", ActionType = ActionType.WindowsServer)]
    public class PlaywrightClearCookies : BaseAibotAction, IAibotAction
    {
        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.ClearCookies();
        }
    }

    [AibotItem("Pl等待加载状态", ActionType = ActionType.WindowsServer)]
    public class PlaywrightWaitForLoadState : BaseAibotAction, IAibotAction
    {
        [AibotProperty("状态[load/domcontentloaded/networkidle]", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty State { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var state = State.Value?.ToString()?.ToLower() switch
            {
                "domcontentloaded" => LoadState.DOMContentLoaded,
                "networkidle" => LoadState.NetworkIdle,
                _ => LoadState.Load
            };
            await PlaywrightHelper.Instance.WaitForLoadState(state);
        }
    }

    [AibotItem("Pl拦截请求", ActionType = ActionType.WindowsServer)]
    public class PlaywrightBlockRequests : BaseAibotAction, IAibotAction
    {
        [AibotProperty("URL模式(逗号分隔)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Patterns { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var patterns = (Patterns.Value?.ToString() ?? "")
                .Split(',')
                .Select(p => p.Trim())
                .Where(p => !string.IsNullOrEmpty(p))
                .ToArray();

            await PlaywrightHelper.Instance.BlockRequests(patterns);
        }
    }

    [AibotItem("Pl设置请求头", ActionType = ActionType.WindowsServer)]
    public class PlaywrightSetHeaders : BaseAibotAction, IAibotAction
    {
        [AibotProperty("请求头JSON", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty HeadersJson { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            var headers = JsonSerializer.Deserialize<Dictionary<string, string>>(
                HeadersJson.Value?.ToString() ?? "{}"
            );
            await PlaywrightHelper.Instance.SetExtraHeaders(headers);
        }
    }

    [AibotItem("Pl清理资源", ActionType = ActionType.WindowsServer)]
    public class PlaywrightCleanup : BaseAibotAction, IAibotAction
    {
        public new async Task Execute(AibotV blackboard)
        {
            await PlaywrightHelper.Instance.Cleanup();
        }
    }
}

