using System.Windows;
using System;
using System.Windows.Input;
using Nodify;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Windows.Controls;


namespace Aibot
{
    public partial class MainWindow : Window
    {

        public MainWindow()
        {
            InitializeComponent();
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
