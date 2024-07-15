using Nodify;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Controls;

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
        public MainViewModel()
        {
            MenuItems = new ObservableCollection<MenuItem>
            {
                new MenuItem { Name = "列表", View = new ListEditor(), IconSource = "/Images/list.png" },
                new MenuItem { Name = "工作流", View = new MainEditor(), IconSource = "/Images/workfows.png" },
                new MenuItem { Name = "使用说明", View = new Hellp(), IconSource = "/Images/hellp.png" },
            };
            SelectedMenuItem = MenuItems.FirstOrDefault();
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
