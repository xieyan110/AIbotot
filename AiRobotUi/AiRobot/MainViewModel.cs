using Nodify;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using Wpf.Ui.Input;

namespace Aibot
{
    public class MenuItem
    {
        public string Name { get; set; }
        public UserControl View { get; set; }
        public string IconSource { get; set; }  // 新增属性，用于存储图标路径
    }


    public class MainViewModel : ObservableObject
    {

        private bool _isVisible = true;
        public bool IsVisible
        {
            get => _isVisible;
            set
            {
                _isVisible = value;
                OnPropertyChanged(nameof(IsVisible));
            }
        }

        private bool _isGrayedOut;
        public bool IsGrayedOut
        {
            get => _isGrayedOut;
            set
            {
                _isGrayedOut = value;
                OnPropertyChanged(nameof(IsGrayedOut));
            }
        }

        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Name = "列表", View = new ListEditor(), IconSource = "/Images/list.png" },
                new MenuItem { Name = "工作流", View = new MainEditor(), IconSource = "/Images/workfows.png" },
                new MenuItem { Name = "使用说明", View = new Hellp(), IconSource = "/Images/hellp.png" },
            };
            SelectedMenuItem = MenuItems.FirstOrDefault();
            // 获取或创建 CustomOverlayWindow 实例
            CustomOverlayManager.GetInstance();
        }

        public event PropertyChangedEventHandler PropertyChanged;
        protected virtual void OnPropertyChanged(string propertyName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public ObservableCollection<MenuItem> MenuItems { get; set; } = new();

        private MenuItem _selectedMenuItem;

        public MenuItem SelectedMenuItem
        {
            get => _selectedMenuItem;
            set
            {
                SetProperty(ref _selectedMenuItem, value);
                CurrentView = _selectedMenuItem.View;
            }
        }

        private UserControl _currentView;
        public UserControl CurrentView
        {
            get => _currentView;
            set => SetProperty(ref _currentView, value);
        }

    }


}
