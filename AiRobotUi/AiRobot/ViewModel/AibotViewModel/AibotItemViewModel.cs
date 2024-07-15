using System;
using Nodify;

namespace Aibot
{
    public class AibotItemViewModel : ObservableObject
    {
        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private Type? _type;
        public Type? Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private NodifyObservableCollection<AibotKeyViewModel> _input = new NodifyObservableCollection<AibotKeyViewModel>();
        public NodifyObservableCollection<AibotKeyViewModel> Input
        {
            get => _input;
            set
            {
                if (value == null)
                {
                    value = new NodifyObservableCollection<AibotKeyViewModel>();
                }

                SetProperty(ref _input!, value);
            }
        }

        private NodifyObservableCollection<AibotKeyViewModel> _output = new NodifyObservableCollection<AibotKeyViewModel>();
        public NodifyObservableCollection<AibotKeyViewModel> Output
        {
            get => _output;
            set
            {
                if (value == null)
                {
                    value = new NodifyObservableCollection<AibotKeyViewModel>();
                }

                SetProperty(ref _output!, value);
            }
        }
    }
}
