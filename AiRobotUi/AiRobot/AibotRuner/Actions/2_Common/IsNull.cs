using Newtonsoft.Json.Linq;
using Nodify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Aibot
{
    [AibotItem("通用-不是Null?", ActionType = ActionType.CommonServer)]
    public class IsNull : IF,IAibotAction
    {
        [AibotProperty("Obj", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV aibot)
        {
            if(!string.IsNullOrEmpty(Text.Value?.ToString() ?? ""))
                (aibot["IsSuccess"], aibot["IsError"]) = (true, false);
            else
                (aibot["IsSuccess"], aibot["IsError"]) = (false, true);

            return Task.CompletedTask;
        }
    }

    [AibotItem("通用-是否相等?", ActionType = ActionType.CommonServer)]
    public class IsEqual : IF, IAibotAction
    {
        [AibotProperty("Value1", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Value1 { get; set; }
        [AibotProperty("Value2", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Value2 { get; set; }

        public new Task Execute(AibotV aibot)
        {
            if ((Value1.Value?.ToString() ?? "") == (Value2.Value?.ToString() ?? ""))
                (aibot["IsSuccess"], aibot["IsError"]) = (true, false);
            else
                (aibot["IsSuccess"], aibot["IsError"]) = (false, true);


            return Task.CompletedTask;
        }
    }

    [AibotItem("通用-是否包含?", ActionType = ActionType.CommonServer)]
    public class IsContains : IF, IAibotAction
    {
        [AibotProperty("Value1", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Value1 { get; set; }
        [AibotProperty("Value2", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Value2 { get; set; }

        public new Task Execute(AibotV aibot)
        {

            if ((Value1.Value?.ToString() ?? "").Contains(Value2.Value?.ToString() ?? ""))
                (aibot["IsSuccess"], aibot["IsError"]) = (true, false);
            else
                (aibot["IsSuccess"], aibot["IsError"]) = (false, true);


            return Task.CompletedTask;
        }
    }

    [AibotItem("通用-计数器", ActionType = ActionType.CommonServer)]
    public class CountAction : BaseAibotAction, IAibotAction
    {
        [AibotProperty("初始值(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty StartValue { get; set; }

        [AibotProperty("叠加值(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty AddValue { get; set; }
        
        [AibotProperty("重置(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty ResetValue { get; set; }

        [AibotProperty("结果(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV aibot)
        {
            var start = StartValue.Value?.TryInt() ?? 0;
            if (ResetValue.Value?.TryInt() != 0 && StartValue.Value?.TryInt() > ResetValue.Value?.TryInt())
            {
                start = 0;
                aibot.Node!.Input.ForEach(node =>
                {
                    if (node.PropertyName == "StartValue")
                        node.Value = ResetValue.Value?.TryInt();
                });
            }

            var result = start + (AddValue.Value?.TryInt() ?? 0);

            aibot.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Result")
                    node.Value = result;
            });
            return Task.CompletedTask;
        }
    }
}
