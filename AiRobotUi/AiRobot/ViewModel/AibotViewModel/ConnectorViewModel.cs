using System;
using System.Collections.Generic;
using System.Windows;
using Nodify;

namespace Aibot
{
    public enum ConnectorKeyType
    {
        Root,
        Boolean,
        Integer,
        Double,
        String,
        List,
        Dictionary,
        Object,
    }

    /// <summary>
    /// input 和 output
    /// </summary>
    public class ConnectorViewModel : ObservableObject
    {
        public Guid Id { get; set; }

        public ConnectorViewModel(Guid id)
        {
            if(Id == Guid.Empty)
            {
                Id = id;
            }
        }

        public ConnectorViewModel() : this(Guid.NewGuid()) { }


        private string? _title;
        public string? Title
        {
            get => _title;
            set => SetProperty(ref _title, value);
        }
        private string? _propertyName;
        public string? PropertyName
        {
            get => _propertyName;
            set => SetProperty(ref _propertyName, value);
        }

        private bool _isConnected;
        public bool IsConnected
        {
            get => _isConnected;
            set => SetProperty(ref _isConnected, value);
        }

        private bool _isPublic;
        public bool IsPublic
        {
            get => _isPublic;
            set => SetProperty(ref _isPublic, value);
        }

        private bool _isInput;
        public bool IsInput
        {
            get => _isInput;
            set => SetProperty(ref _isInput, value);
        }

        private Point _anchor;
        public Point Anchor
        {
            get => _anchor;
            set => SetProperty(ref _anchor, value);
        }

        private OperationViewModel _operation = default!;
        public OperationViewModel Operation
        {
            get => _operation;
            set => SetProperty(ref _operation, value);
        }


        #region data
        //public List<ConnectorViewModel> ValueObservers { get; } = new List<ConnectorViewModel>();


        private AibotKeyType _type;
        public AibotKeyType Type
        {
            get => _type;
            set
            {
                if (SetProperty(ref _type, value))
                {
                    Value = GetDefaultValue(_type);
                }
            }
        }

        private readonly Dictionary<bool, object?> _values = new Dictionary<bool, object?>();

        private object? _value = BoxValue.False;
        public object? Value
        {
            get => _value;
            set => SetProperty(ref _value, GetRealValue(value)).Then(() => _values[ValueIsKey] = Value);
        }

        private bool _valueIsKey;
        public bool ValueIsKey
        {
            get => _valueIsKey;
            set
            {
                if (SetProperty(ref _valueIsKey, value) && _values.TryGetValue(_valueIsKey, out var existingValue))
                {
                    Value = existingValue;
                }
            }
        }

        private bool _canChangeType = true;
        public bool CanChangeType
        {
            get => _canChangeType;
            set => SetProperty(ref _canChangeType, value);
        }

        private bool _isRoot = true;
        public bool IsRoot
        {
            get => _isRoot;
            set => SetProperty(ref _isRoot, value);
        }

        private object? GetRealValue(object? value)
        {
            if (value is string str)
            {
                switch (Type)
                {
                    case AibotKeyType.Boolean:
                        bool.TryParse(str, out var b);
                        value = b;
                        break;

                    case AibotKeyType.Integer:
                        int.TryParse(str, out var i);
                        value = i;
                        break;

                    case AibotKeyType.Double:
                        double.TryParse(str, out var d);
                        value = d;
                        break;

                    case AibotKeyType.String:
                    case AibotKeyType.Object:
                        value = str;
                        break;
                }
            }

            return value;
        }

        public static object? GetDefaultValue(AibotKeyType type)
            => type switch
            {
                AibotKeyType.Boolean => BoxValue.False,
                AibotKeyType.Integer => BoxValue.Int0,
                AibotKeyType.Double => BoxValue.Double0,
                AibotKeyType.String => null,
                AibotKeyType.Object => null,
                _ => null
            };
        #endregion
    }
}
