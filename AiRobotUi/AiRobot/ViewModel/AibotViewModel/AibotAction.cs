using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Nodify;

namespace Aibot
{
    public class AibotAction: ObservableObject
    {
        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private Type _type;

        public Type Type
        {
            get => _type;
            set => SetProperty(ref _type, value);
        }

        private NodifyObservableCollection<ConnectorViewModel> _input = new NodifyObservableCollection<ConnectorViewModel>();
        public NodifyObservableCollection<ConnectorViewModel> Input
        {
            get => _input;
            set
            {
                if (value == null)
                {
                    value = new NodifyObservableCollection<ConnectorViewModel>();
                }

                SetProperty(ref _input!, value);
            }
        }

        private NodifyObservableCollection<ConnectorViewModel> _output = new NodifyObservableCollection<ConnectorViewModel>();
        public NodifyObservableCollection<ConnectorViewModel> Output
        {
            get => _output;
            set
            {
                if (value == null)
                {
                    value = new NodifyObservableCollection<ConnectorViewModel>();
                }

                SetProperty(ref _output!, value);
            }
        }
    }
}
