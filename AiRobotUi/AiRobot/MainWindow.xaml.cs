using System.Windows;
using System;
using System.Windows.Input;
using Nodify;
using System.Collections.ObjectModel;
using System.ComponentModel;
using Vanara.PInvoke;
using Wpf.Ui.Controls;
using Wpf.Ui.Tray.Controls;
using System.Windows.Interop;

namespace Aibot
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void HideToTrayButton_Click(object sender, RoutedEventArgs e)
        {
            this.Hide();
            ((MainViewModel)DataContext).IsVisible = false;
        }

        private void MinimizeButton_Click(object sender, RoutedEventArgs e)
        {
            this.WindowState = WindowState.Minimized;
        }

        private void CloseButton_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }
        protected override void OnClosing(CancelEventArgs e)
        {
            // 如果是正常关闭（不是最小化到托盘），则调用原有的 OnClosed 逻辑
            if (!e.Cancel)
            {
                base.OnClosed(e);
                CustomOverlayManager.ShutdownApplication();
            }
            else
            {
                // 最小化到托盘
                e.Cancel = true;
                this.Hide();
                ((MainViewModel)DataContext).IsVisible = false;
            }
        }

        private void OnNotifyIconLeftDoubleClick(NotifyIcon sender, RoutedEventArgs e)
        {
            if (Application.Current.MainWindow.Visibility == Visibility.Visible)
            {
                Application.Current.MainWindow.Hide();
            }
            else
            {
                Application.Current.MainWindow.Activate();
                Application.Current.MainWindow.Focus();
                Application.Current.MainWindow.Show();

                var window = Application.Current.MainWindow;
                if (window != null)
                {
                    if (window.Visibility != Visibility.Visible)
                    {
                        window.Visibility = Visibility.Visible;
                    }
                    if (window.WindowState == WindowState.Minimized)
                    {
                        nint hWnd = new WindowInteropHelper(Application.Current.MainWindow).Handle;
                        _ = User32.SendMessage(hWnd, User32.WindowMessage.WM_SYSCOMMAND, User32.SysCommand.SC_RESTORE, IntPtr.Zero);
                    }
                }

            }
        }

        protected override void OnClosed(EventArgs e)
        {
            base.OnClosed(e);
            CustomOverlayManager.ShutdownApplication();
        }

        protected override void OnSourceInitialized(EventArgs e)
        {

            Hotkey.Regist(this, HotkeyModifiers.MOD_SHIFT, Key.W, () =>
            {
                MainViewModel viewModel = (MainViewModel)DataContext;
                var editor = viewModel.MenuItems[1].View as MainEditor;

                // 页面 MainEditor工作流 的关闭
                if (editor is null) return;
                var view = (ApplicationViewModel)editor.DataContext;

                view.Editors.ForEach(x =>
                {
                    x.Calculator.PauseCommand.Execute(null);
                });

                // 页面 ListEditor工作流列表 的关闭
                var listEditor = viewModel.MenuItems[0].View as ListEditor;

                if (listEditor is null) return;
                var view2 = (ListEditorViewModel)listEditor.DataContext;

                view2.OperationList.ForEach(x =>
                {
                    x.PauseCommand.Execute(null);
                });
            });
        }
    }

}
