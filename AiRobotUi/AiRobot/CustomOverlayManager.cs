using Aibot.Page;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;

namespace Aibot
{

    public static class CustomOverlayManager
    {
        private static CustomOverlayWindow _instance;
        private static readonly object _lock = new object();

        public static CustomOverlayWindow GetInstance()
        {
            if (_instance == null || !IsWindowValid(_instance))
            {
                lock (_lock)
                {
                    if (_instance == null || !IsWindowValid(_instance))
                    {
                        _instance = null; // Ensure any invalid instance is cleared
                        Application.Current.Dispatcher.Invoke(() =>
                        {
                            _instance = new CustomOverlayWindow();
                        });
                    }
                }
            }
            return _instance;
        }

        private static bool IsWindowValid(CustomOverlayWindow window)
        {
            return Application.Current.Dispatcher.Invoke(() =>
            {
                return window != null && window.IsLoaded && !window.IsClosing;
            });
        }

        public static void ResetInstance()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance != null && _instance.IsLoaded && !_instance.IsClosing)
                {
                    _instance.Reset();
                }
                else
                {
                    _instance = new CustomOverlayWindow();
                }
            });
        }

        public static void CloseInstance()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                if (_instance != null && _instance.IsLoaded && !_instance.IsClosing)
                {
                    _instance.Close();
                    _instance = null;
                }
            });
        }
        public static void ShutdownApplication()
        {
            CloseInstance();
            Application.Current.Shutdown();
        }
        public static void Show()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().Show();
            });
        }


        public static void ForceTopmost()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().ForceTopmost();
            });
        }

        public static void SetTopmost(bool isTopmost)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().Topmost = isTopmost;
            });
        }

        public static void SetSize(double width, double height)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().SetSize(width, height);
            });
        }

        public static void SetPosition(double left, double top)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().SetPosition(left, top);
            });
        }
        
        public static void SetTitle(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().SetTitle(text);
            });
        }

        public static void SetBackgroundColor(string colorString)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().SetBackgroundColor(colorString);
            });
        }

        public static void SetTextColor(string colorString)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().SetTextColor(colorString);
            });
        }

        public static void StartLoadingAnimation(string text)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().StartLoadingAnimation(text);
            });
        }

        public static void StopLoadingAnimation()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().StopLoadingAnimation();
            });
        }

        public static void SetShowLogs(bool show)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().SetShowLogs(show);
            });
        }

        public static void AddLogMessage(string message)
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().AddLogMessage(message);
            });
        }

        public static void ClearLog()
        {
            Application.Current.Dispatcher.Invoke(() =>
            {
                GetInstance().ClearLog();
            });
        }

    }
}
