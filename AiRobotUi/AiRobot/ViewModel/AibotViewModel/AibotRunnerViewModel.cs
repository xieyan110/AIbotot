using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using Nodify;

namespace Aibot
{
    public class AibotRunnerViewModel : ObservableObject
    {

        /// <summary>
        /// 创建Action
        /// </summary>
        /// <param name="action"></param>
        /// <returns></returns>
        public IAibotAction? CreateAction(AibotAction? action)
        {
            if (action?.Type != null && typeof(IAibotAction).IsAssignableFrom(action.Type))
            {
                // TODO: DI Container
                var result = (IAibotAction?)Activator.CreateInstance(action.Type);

                InitializeKeys(action.Input, result, action.Type);
                InitializeKeys(action.Output, result, action.Type);

                return result;
            }

            return default;
        }

        /// <summary>
        /// 创建 Action 的参数
        /// </summary>
        /// <param name="Aibot"></param>
        /// <returns></returns>
        public AibotV CreateAibot(AibotAction? action)
        {
            AibotV result = new AibotV();

            foreach(var i in action?.Input)
            {
                if(i.Title != null)
                    result.Set(new AibotKey(i.Title, i.Type), i.Value);
            }

            //result.CopyTo(_original);

            //_debugger.Attach(result);
            return result;
        }

        private void InitializeKeys(NodifyObservableCollection<ConnectorViewModel> keys, object? instance, Type type)
        {
            for (int i = 0; i < keys.Count; i++)
            {
                var vm = keys[i];
                var key = CreateActionValue(vm);

                // TODO: Property cache
                if (vm.PropertyName != null)
                {
                    var prop = type.GetProperty(vm.PropertyName);

                    if (prop?.CanWrite ?? false)
                    {
                        prop.SetValue(instance, key);
                    }
                }
            }
        }

        private AibotProperty CreateActionValue(ConnectorViewModel key)
        {
            if (key.Value is AibotKeyViewModel bkv)
            {
                return new AibotProperty(new AibotKey(bkv.Name, bkv.Type));
            }

            return new AibotProperty(key.Value);
        }

    }
}
