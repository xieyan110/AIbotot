using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aibot
{
    public class IF : IAibotAction
    {
        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Input)]
        public object? InputRoot { get; set; }

        [AibotProperty("成功(Bool)", AibotKeyType.Boolean, IsRoot = true, Usage = AibotKeyUsage.Output)]
        public AibotProperty IsSuccess { get; set; }

        [AibotProperty("失败(Bool)", AibotKeyType.Boolean, IsRoot = true, Usage = AibotKeyUsage.Output)]
        public AibotProperty IsError { get; set; }

        public Task Execute(AibotV aibot)
        {
            (aibot["IsSuccess"], aibot["IsError"]) = (true, false);
            return Task.CompletedTask;
        }
    }
}
