using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aibot
{
    public class BaseAibotAction : IAibotAction
    {
        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Input)]
        public AibotProperty InputRoot { get; set; }

        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutputRoot { get; set; }

        public Task Execute(AibotV blackboard)
        {
            return Task.CompletedTask;
        }
    }

    public class BaseAdbAibotAction : IAibotAction
    {
        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Input)]
        public AibotProperty InputRoot { get; set; }

        [AibotProperty("设备[可为空](String)", AibotKeyType.Object, IsRoot = false, Usage = AibotKeyUsage.Input)]
        public AibotProperty DeviceName { get; set; }

        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutputRoot { get; set; }

        [AibotProperty("设备(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Device { get; set; }

        public Task Execute(AibotV blackboard)
        {
            return Task.CompletedTask;
        }
    }

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
