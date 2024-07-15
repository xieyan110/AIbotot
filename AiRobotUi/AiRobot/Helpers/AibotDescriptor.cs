using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using Newtonsoft.Json;
using Nodify;

namespace Aibot
{
    public static class AibotDescriptor
    {
        private class KeyDescription
        {
            public KeyDescription(string displayName, 
                                  string propertyName, 
                                  AibotKeyType type, 
                                  bool canChangeType,
                                  bool isRoot)
            {
                DisplayName = displayName;
                PropertyName = propertyName;
                Type = type;
                CanChangeType = canChangeType;
                IsRoot = isRoot;
            }

            public string DisplayName { get; }
            public string PropertyName { get; }
            public AibotKeyType Type { get; }
            public bool CanChangeType { get; }
            public bool IsRoot { get; }

        }

        private class ItemDescription
        {
            public string? Name { get; set; }
            public ActionType? ActionType { get; set; }
            public ActionViewType ActionViewType { get; set; }
            public List<KeyDescription> Input { get; } = new List<KeyDescription>();
            public List<KeyDescription> Output { get; } = new List<KeyDescription>();
        }

        public static AibotAction? GetItem(AibotItemReferenceViewModel? actionRef)
        {
            if (actionRef?.Type != null)
            {
                var description = GetDescription(actionRef.Type);

                var input = description.Input.OrderBy(x => !x.IsRoot).Select(d => new ConnectorViewModel
                {
                    Title = d.DisplayName,
                    Type = d.Type,
                    PropertyName = d.PropertyName,
                    CanChangeType = d.CanChangeType,
                    IsRoot = d.IsRoot,
                    ValueIsKey = true,
                    IsInput = true,
                });

                var output = description.Output.OrderBy(x => !x.IsRoot).Select(d => new ConnectorViewModel
                {
                    Title = d.DisplayName,
                    Type = d.Type,
                    PropertyName = d.PropertyName,
                    CanChangeType = d.CanChangeType,
                    IsRoot = d.IsRoot,
                    ValueIsKey = true,
                });

                return new AibotAction
                {
                    Name = actionRef.Name,
                    Type = actionRef.Type,
                    Input = new NodifyObservableCollection<ConnectorViewModel>(input),
                    Output = new NodifyObservableCollection<ConnectorViewModel>(output),
                };
            }

            return default;
        }

        public static AibotItemReferenceViewModel GetReference(Type type)
        {
            var desc = GetDescription(type);

            return new AibotItemReferenceViewModel
            {
                Name = desc.Name,
                Type = type,
                ActionType = desc?.ActionType ?? ActionType.CommonServer,
                ActionViewType = desc?.ActionViewType ?? ActionViewType.Normal,
            };
        }

        private static readonly Dictionary<Type, ItemDescription> _descriptions = new Dictionary<Type, ItemDescription>();
        private static ItemDescription GetDescription(Type type)
        {
            if (!_descriptions.TryGetValue(type, out var description))
            {
                var actionAttr = type.GetCustomAttribute<AibotItemAttribute>();

                var desc = new ItemDescription
                {
                    Name = actionAttr?.DisplayName ?? type.Name,
                    ActionType = actionAttr?.ActionType ?? ActionType.CommonServer,
                    ActionViewType = actionAttr?.ActionViewType ?? ActionViewType.Normal,
                };

                var props = type.GetProperties();
                for (int i = 0; i < props.Length; i++)
                {
                    var prop = props[i];
                    var keyAttr = prop.GetCustomAttribute<AibotPropertyAttribute>();

                    if (keyAttr != null)
                    {
                        var key = new KeyDescription(keyAttr.Name ?? prop.Name, prop.Name, keyAttr.Type, keyAttr.CanChangeType, keyAttr.IsRoot);

                        if (keyAttr.Usage == AibotKeyUsage.Input)
                        {
                            desc.Input.Add(key);
                        }
                        else
                        {
                            desc.Output.Add(key);
                        }
                    }
                }

                _descriptions.Add(type, desc);

                return desc;
            }

            return description;
        }

        public static List<AibotItemReferenceViewModel> GetAvailableItems<T>()
        {
            var result = new List<AibotItemReferenceViewModel>();
            var ourType = typeof(T);

            var types = ourType.Assembly.GetTypes();

            for (int i = 0; i < types.Length; i++)
            {
                var type = types[i];
                if (type.IsClass && !type.IsAbstract && ourType.IsAssignableFrom(type))
                {
                    result.Add(GetReference(type));
                }
            }

            return result.OrderBy(op => (op.ActionType))
                         .Select(x=>x)
                         .ToList();
        }


        public static List<T> ToList<T>(this object obj)
        {
            return JsonConvert.DeserializeObject<List<T>>(JsonConvert.SerializeObject(obj) ?? "") ?? new List<T>();
        }

        public static T Case<T>(this object obj) where T : new()
        {
            return JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj) ?? "") ?? new T();
        }
        public static int TryInt(this object obj)
        {
            var i = obj.TryDouble();
            return (int)i;

        }
        public static double TryDouble(this object obj)
        {
            if (double.TryParse(obj.ToString(), out var r))
            {
                return r;
            }
            return 0;
        }

        public static string GetEnumDescription(this Enum value)
        {
            var fieldInfo = value.GetType().GetField(value?.ToString() ?? "");
            var attributes = (DescriptionAttribute[])fieldInfo.GetCustomAttributes(typeof(DescriptionAttribute), false);
            return attributes.Length > 0 ? attributes[0].Description : value?.ToString() ?? "";
        }

    }
}
