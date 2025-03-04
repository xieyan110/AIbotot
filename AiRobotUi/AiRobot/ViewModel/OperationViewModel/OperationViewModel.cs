using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Windows;
using Nodify;

namespace Aibot
{
    public enum NodeStatus
    {
        /// <summary>
        /// 未执行
        /// </summary>
        NotExecuted,
        /// <summary>
        /// 执行中
        /// </summary>
        Executing,
        /// <summary>
        /// 已经执行
        /// </summary>
        Executed
    }

    /// <summary>
    /// Node
    /// </summary>
    public class OperationViewModel : ObservableObject
    {
        public OperationViewModel()
        {
            Input.WhenAdded(x =>
            {
                x.Operation = this;
                x.IsInput = true;
                x.PropertyChanged += OnInputValueChanged;
            })
            .WhenRemoved(x =>
            {
                x.PropertyChanged -= OnInputValueChanged;
            });
        }

        private void OnInputValueChanged(object? sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(ConnectorViewModel.Value))
            {
                OnInputValueChanged();
            }

        }

        private NodeStatus _nodeStatus;
        /// <summary>
        /// node 状态，未执行，执行中，已执行
        /// </summary>
        public NodeStatus NodeStatus
        {
            get => _nodeStatus;
            set => SetProperty(ref _nodeStatus, value);
        }

        private Point _location;
        public Point Location
        {
            get => _location;
            set => SetProperty(ref _location, value);
        }

        private Size _size;
        public Size Size
        {
            get => _size;
            set => SetProperty(ref _size, value);
        }

        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }

        private bool _isSelected;
        public bool IsSelected
        {
            get => _isSelected;
            set => SetProperty(ref _isSelected, value);
        }

        #region state
        private string? _name;
        public string? Name
        {
            get => _name;
            set => SetProperty(ref _name, value);
        }

        private bool _isRenaming;
        public bool IsRenaming
        {
            get => _isRenaming;
            set => SetProperty(ref _isRenaming, value);
        }

        private bool _isActive;
        public bool IsActive
        {
            get => _isActive;
            set => SetProperty(ref _isActive, value);
        }

   

        private AibotItemReferenceViewModel? _actionReference;
        public AibotItemReferenceViewModel? ActionReference
        {
            get => _actionReference;
            set
            {
                if (SetProperty(ref _actionReference, value))
                {
                    SetAction(_actionReference);
                }
            }
        }

        public AibotAction? Action { get; private set; }

        public bool IsEditable { get; set; } = true;

        private void SetAction(AibotItemReferenceViewModel? actionRef)
        {
            Action = AibotDescriptor.GetItem(actionRef);
            if(Input.Count == 0 && Action?.Input.Count != 0)
            {
                Input = Action!.Input;
            }
            if (Output.Count == 0 && Action?.Output.Count != 0)
            {
                Output = Action!.Output;
            }
            OnPropertyChanged(nameof(Action));
        }

        #endregion

        public bool IsReadOnly { get; set; }


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
      
        protected virtual void OnInputValueChanged()
        {

        }
    }
}
