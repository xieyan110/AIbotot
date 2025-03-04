using Nodify;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using Wpf.Ui.Input;

namespace Aibot
{
    public class HelpViewModel : ObservableObject
    {
        //private string _cronExampleUrl = "https://www.bejson.com/othertools/cron";
        //public string CronExampleUrl
        //{
        //    get => _cronExampleUrl;
        //    set => SetProperty(ref _cronExampleUrl, value);
        //}

        public INodifyCommand OpenUrlCommand { get; }

        public HelpViewModel()
        {
            OpenUrlCommand = new DelegateCommand<string>(OpenUrl);
        }

        private void OpenUrl(string url)
        {
            if (!string.IsNullOrEmpty(url))
            {
                Process.Start(new ProcessStartInfo(url) { UseShellExecute = true });
            }
        }

    }

}
