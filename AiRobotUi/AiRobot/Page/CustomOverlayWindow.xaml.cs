using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using System.Windows.Threading;

namespace Aibot.Page
{
    /// <summary>
    /// CustomOverlayWindow.xaml 的交互逻辑
    /// </summary>
    public partial class CustomOverlayWindow : Window
    {
        private DispatcherTimer _animationTimer;
        private string _currentAnimationText;
        private int _currentCharIndex;
        private List<string> _logMessages = new List<string>();
        private const int MaxLogMessages = 500;
        private bool _showLogs = true;
        public bool IsClosing { get; private set; }


        [DllImport("user32.dll")]
        private static extern bool SetWindowPos(IntPtr hWnd, IntPtr hWndInsertAfter, int X, int Y, int cx, int cy, uint uFlags);

        private static readonly IntPtr HWND_TOPMOST = new IntPtr(-1);
        private const uint SWP_NOMOVE = 0x0002;
        private const uint SWP_NOSIZE = 0x0001;



        internal CustomOverlayWindow()
        {
            InitializeComponent();
            Loaded += (sender, args) => SetSize(300, 400);
        }

        public void ForceTopmost()
        {
            IntPtr handle = new System.Windows.Interop.WindowInteropHelper(this).Handle;
            SetWindowPos(handle, HWND_TOPMOST, 0, 0, 0, 0, SWP_NOMOVE | SWP_NOSIZE);
        }

        // 新添加的方法来设置是否显示日志
        public void SetShowLogs(bool show)
        {
            _showLogs = show;
            UpdateLogVisibility();
        }

        private void UpdateLogVisibility()
        {
            if (LogTextBlock.Parent is ScrollViewer scrollViewer)
            {
                scrollViewer.Visibility = _showLogs ? Visibility.Visible : Visibility.Collapsed;
            }
        }

        public void AddLogMessage(string message)
        {
            _logMessages.Add(message);
            if (_logMessages.Count > MaxLogMessages)
            {
                _logMessages.RemoveAt(0);
            }
            if (_showLogs)
            {
                UpdateLogDisplay();
            }
        }

        private void UpdateLogDisplay()
        {
            if (_showLogs)
            {
                LogTextBlock.Text = string.Join(Environment.NewLine, _logMessages);
                Dispatcher.InvokeAsync(() =>
                {
                    if (LogTextBlock.Parent is ScrollViewer scrollViewer)
                    {
                        scrollViewer.ScrollToBottom();
                    }
                });
            }
        }

        public void ClearLog()
        {
            _logMessages.Clear();
            if (_showLogs)
            {
                LogTextBlock.Text = "";
            }
        }

        public void StartLoadingAnimation(string text)
        {
            _currentAnimationText = text;
            _currentCharIndex = 0;

            if (_animationTimer == null)
            {
                _animationTimer = new DispatcherTimer();
                _animationTimer.Tick += AnimationTimer_Tick;
                _animationTimer.Interval = TimeSpan.FromSeconds(0.2);
            }

            AnimatedTextBlock.Text = "";
            _animationTimer.Start();
        }

        private void AnimationTimer_Tick(object sender, EventArgs e)
        {
            if (_currentCharIndex < _currentAnimationText.Length)
            {
                AnimatedTextBlock.Text += _currentAnimationText[_currentCharIndex];
                _currentCharIndex++;
            }
            else
            {
                AnimatedTextBlock.Text = "";
                _currentCharIndex = 0;
            }
        }

        // 添加一个重置方法
        internal void Reset()
        {
            StopLoadingAnimation();
            ClearLog();
            SetTitle("");
            SetBackgroundColor("#80000000");
            SetTextColor("#FFFFFF");
            SetSize(300, 400);
            SetPosition(100, 100);
            SetTopmost(false);
            SetShowLogs(true); // 重置时默认显示日志
        }

        public void SetTopmost(bool isTopmost)
        {
            Topmost = isTopmost;
        }

        public void SetTitle(string text)
        {
            AnimatedTextBlock.Text = text;
        }

        public void SetSize(double width, double height)
        {
            MainGrid.Width = width;
            MainGrid.Height = height;
        }

        public void SetPosition(double left, double top)
        {
            Left = left;
            Top = top;
        }

        public void SetBackgroundColor(string colorString)
        {
            Color color = ParseColor(colorString);
            MainBorder.Background = new SolidColorBrush(color);
        }

        public void SetTextColor(string colorString)
        {
            Color color = ParseColor(colorString);
            AnimatedTextBlock.Foreground = new SolidColorBrush(color);
            LogTextBlock.Foreground = new SolidColorBrush(color);
        }

        private Color ParseColor(string colorString)
        {
            colorString = colorString.Trim();

            if (colorString.StartsWith("#"))
            {
                return (Color)ColorConverter.ConvertFromString(colorString);
            }
            else if (colorString.StartsWith("rgb", StringComparison.OrdinalIgnoreCase))
            {
                var rgbValues = colorString.Replace("rgb(", "").Replace(")", "").Split(',');
                if (rgbValues.Length == 3 &&
                    byte.TryParse(rgbValues[0], out byte r) &&
                    byte.TryParse(rgbValues[1], out byte g) &&
                    byte.TryParse(rgbValues[2], out byte b))
                {
                    return Color.FromRgb(r, g, b);
                }
            }

            return Colors.White;
        }


        public void StopLoadingAnimation()
        {
            if (_animationTimer != null)
            {
                _animationTimer.Stop();
            }
            AnimatedTextBlock.Text = "";
        }


        protected override void OnClosing(CancelEventArgs e)
        {
            IsClosing = true;
            base.OnClosing(e);
        }

    }

}
