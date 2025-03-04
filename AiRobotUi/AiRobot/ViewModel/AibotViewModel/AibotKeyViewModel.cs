using System.Collections.Generic;
using Nodify;

namespace Aibot
{
    public class AibotKeyViewModel : ObservableObject
    {
        // Cache the key and the input value so we can restore them when swapping input types
        private readonly Dictionary<bool, object?> _values = new Dictionary<bool, object?>();

        public string? PropertyName { get; set; }

        private string _name = "New key";
        public string Name
        {
            get => _name;
            set
            {
                if (!string.IsNullOrWhiteSpace(value))
                {
                    SetProperty(ref _name, value);
                }
            }
        }

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
                    default:
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
    }
}
