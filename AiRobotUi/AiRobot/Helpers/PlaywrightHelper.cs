using Microsoft.Playwright;
using System;
using System.CodeDom;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Text.Json.Serialization;

namespace Aibot
{
    public class PlaywrightHelper
    {
        private static PlaywrightHelper _instance;
        private IPlaywright _playwright;
        private IBrowser _browser;
        private IBrowserContext _context;
        private IPage _page;

        private PlaywrightHelper() { }

        public static PlaywrightHelper Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new PlaywrightHelper();
                }
                return _instance;
            }
        }

        public enum BrowserType
        {
            Chromium,
            Firefox,
            Webkit
        }

        public class BrowserConfig
        {
            public BrowserType Type { get; set; } = BrowserType.Chromium;
            public string ExecutablePath { get; set; }
            public bool Headless { get; set; } = false;
            public int SlowMo { get; set; } = 0;
            public string[] AdditionalArgs { get; set; }
            public Dictionary<string, string> Environment { get; set; }
        }


        public async Task InitializeBrowser(BrowserConfig config)
        {
            _playwright = await Playwright.CreateAsync();

            var launchOptions = new BrowserTypeLaunchOptions
            {
                Headless = config.Headless,
                ExecutablePath = config.ExecutablePath,
                SlowMo = config.SlowMo,
                Args = config.AdditionalArgs ?? new[]
                {
                "--no-sandbox",
                "--disable-setuid-sandbox",
                "--disable-infobars",
                "--disable-dev-shm-usage",
                "--disable-blink-features=AutomationControlled"
            },
                Env = config.Environment
            };

            _browser = config.Type switch
            {
                BrowserType.Firefox => await _playwright.Firefox.LaunchAsync(launchOptions),
                BrowserType.Webkit => await _playwright.Webkit.LaunchAsync(launchOptions),
                _ => await _playwright.Chromium.LaunchAsync(launchOptions)
            };
            _context = await _browser.NewContextAsync();
            _page = await _context.NewPageAsync();
        }

        public async Task NavigateToUrl(string url, int timeout = 30000)
        {
            await _page.GotoAsync(url, new() { Timeout = timeout });
        }

        public async Task<string> ExecuteJavaScript<T>(string script, T args = default)
        {
            return await _page.EvaluateAsync<string>(script, args);
        }

        public async Task Click(string selector, PageClickOptions options = null)
        {
            try
            {
                await _page.ClickAsync(selector, options ?? new PageClickOptions());
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to click element with selector: {selector}", ex);
            }
        }

        public async Task Fill(string selector, string value, int delay = 0)
        {
            try
            {
                await _page.FillAsync(selector, value);
                if (delay > 0)
                {
                    await Task.Delay(delay);
                }
            }
            catch (Exception ex)
            {
                throw new Exception($"Failed to fill element with selector: {selector}", ex);
            }
        }

        public async Task WaitForSelector(string selector, float timeout = 30000)
        {
            try
            {
                await _page.WaitForSelectorAsync(selector, new() { Timeout = timeout });
            }
            catch (TimeoutException)
            {
                throw new TimeoutException($"Element with selector {selector} not found within {timeout}ms");
            }
        }

        public async Task<string> GetText(string selector)
        {
            var element = await _page.QuerySelectorAsync(selector);
            return element != null ? await element.TextContentAsync() : string.Empty;
        }

        public async Task<bool> IsElementVisible(string selector)
        {
            var element = await _page.QuerySelectorAsync(selector);
            return element != null && await element.IsVisibleAsync();
        }

        public async Task WaitForNavigation(PageWaitForNavigationOptions options = null)
        {
            await _page.WaitForNavigationAsync(options);
        }

        public async Task Screenshot(string path, ScreenshotOptions options = null)
        {
            await _page.ScreenshotAsync(new PageScreenshotOptions
            {
                Path = path,
                FullPage = options?.FullPage ?? false,
                Type = options?.Type ?? ScreenshotType.Png
            });
        }

        public async Task SetHttpHeaders(Dictionary<string, string> headers)
        {
            await _page.SetExtraHTTPHeadersAsync(headers);
        }

        public async Task HandleDialog(bool accept = true, string promptText = null)
        {
            _page.Dialog += async (_, dialog) =>
            {
                if (accept)
                {
                    await dialog.AcceptAsync(promptText);
                }
                else
                {
                    await dialog.DismissAsync();
                }
            };
        }

        public async Task Cleanup()
        {
            if (_page != null) await _page.CloseAsync();
            if (_context != null) await _context.CloseAsync();
            if (_playwright != null)
            {
                _playwright.Dispose();
                _playwright = null;
            }
        }


        public class ScreenshotOptions
        {
            public bool FullPage { get; set; }
            public ScreenshotType Type { get; set; }
        }


        public class ProxyConfig
        {
            public string Server { get; set; }
            public string Username { get; set; }
            public string Password { get; set; }
        }

        public async Task ConfigureProxy(ProxyConfig proxyConfig)
        {
            if (_context != null)
            {
                await _context.CloseAsync();
                _context = await _browser.NewContextAsync(new BrowserNewContextOptions
                {
                    Proxy = new Proxy
                    {
                        Server = proxyConfig.Server,
                        Username = proxyConfig.Username,
                        Password = proxyConfig.Password
                    }
                });
                _page = await _context.NewPageAsync();
            }
        }

        public async Task Hover(string selector, PageHoverOptions options = null)
        {
            await _page.HoverAsync(selector, options ?? new PageHoverOptions());
        }

        public async Task Press(string key)
        {
            await _page.Keyboard.PressAsync(key);
        }

        public async Task Type(string text, int? delay = null)
        {
            await _page.Keyboard.TypeAsync(text, new() { Delay = delay });
        }


        public async Task UploadFile(string selector, string filePath)
        {
            await _page.SetInputFilesAsync(selector, filePath);
        }

        public async Task SetCookies(List<Cookie> cookies)
        {
            await _context.AddCookiesAsync(cookies);
        }

        public async Task<List<BrowserContextCookiesResult>> GetCookies()
        {
            return (List<BrowserContextCookiesResult>)await _context.CookiesAsync();
        }

        public async Task ClearCookies()
        {
            await _context.ClearCookiesAsync();
        }

        public async Task WaitForLoadState(LoadState state = LoadState.Load)
        {
            await _page.WaitForLoadStateAsync(state);
        }

        public async Task WaitForResponse(string urlPattern, Action<IResponse> handler)
        {
            var response = await _page.WaitForResponseAsync(urlPattern);
            handler?.Invoke(response);
        }

        public async Task WaitForNetworkIdle()
        {
            await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
        }

        public void InterceptRequests(string urlPattern, Action<IRequest> handler)
        {
            _page.Request += (_, request) =>
            {
                if (request.Url.Contains(urlPattern))
                {
                    handler?.Invoke(request);
                }
            };
        }

        public async Task BlockRequests(params string[] patterns)
        {
            await _page.RouteAsync("**/*", async (route) =>
            {
                var request = route.Request;
                if (patterns.Any(p => request.Url.Contains(p)))
                {
                    await route.AbortAsync();
                }
                else
                {
                    await route.ContinueAsync();
                }
            });
        }

        public async Task SetExtraHeaders(Dictionary<string, string> headers)
        {
            await _page.SetExtraHTTPHeadersAsync(headers);
        }

    }
}