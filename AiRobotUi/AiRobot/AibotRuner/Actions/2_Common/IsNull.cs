using Newtonsoft.Json.Linq;
using Nodify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Net.Mime.MediaTypeNames;

namespace Aibot
{
    [AibotItem("通用-不是Null?", ActionType = ActionType.CommonServer)]
    public class IsNull : IF, IAibotAction
    {
        [AibotProperty("Obj", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV aibot)
        {
            if (!string.IsNullOrEmpty(Text.Value?.ToString() ?? ""))
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

    [AibotItem("通用-算数基本", ActionType = ActionType.CommonServer)]
    public class BaseCalculate : BaseAibotAction, IAibotAction
    {
        [AibotProperty("X(Double)", AibotKeyType.Double, Usage = AibotKeyUsage.Input)]
        public AibotProperty X { get; set; }

        [AibotProperty("运算符(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Operators { get; set; }

        [AibotProperty("Y(Double)", AibotKeyType.Double, Usage = AibotKeyUsage.Input)]
        public AibotProperty Y { get; set; }

        [AibotProperty("结果(Double)", AibotKeyType.Double, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var x = X.Value?.TryDouble() ?? 0;
            var y = Y.Value?.TryDouble() ?? 0;
            var operators = Operators.Value?.ToString() ?? "";
            double result = 0;
            try
            {
                switch (operators)
                {
                    case "+":
                        result = x + y;
                        break;
                    case "-":
                        result = x - y;
                        break;
                    case "*":
                        result = x * y;
                        break;
                    case "/":
                        if (y != 0)
                            result = x / y;
                        break;
                    case "%":
                        if (y != 0)
                            result = x % y;
                        break;
                    default:
                        result = 0;
                        break;
                }
            }
            catch
            {
                result = 0;
            }


            blackboard.Node.Output.ForEach(output =>
            {
                if (output.PropertyName == "Result")
                    output.Value = result;
            });
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

    [AibotItem("通用-锁住", ActionType = ActionType.CommonServer)]
    public class Locking : BaseAibotAction, IAibotAction
    {
        [AibotProperty("锁的名称(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty LockName { get; set; }

        public new async Task Execute(AibotV aibot)
        {
            var lockName = LockName.Value?.ToString() ?? "";
            var semaphore = Lock.GetOrCreate(lockName);
            await semaphore.WaitAsync();
            Console.WriteLine($"Lock '{lockName}' acquired.");
        }
    }


    [AibotItem("通用-解锁", ActionType = ActionType.CommonServer)]
    public class UnLocking : BaseAibotAction, IAibotAction
    {
        [AibotProperty("锁的名称(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty LockName { get; set; }

        public new Task Execute(AibotV aibot)
        {
            var lockName = LockName.Value?.ToString() ?? "";
            if (Lock.Exists(lockName))
            {
                var semaphore = Lock.GetOrCreate(lockName);
                if (Lock.IsLocked(lockName))
                {
                    semaphore.Release();
                    Console.WriteLine($"Lock '{lockName}' released.");
                }
                else
                {
                    Console.WriteLine($"Lock '{lockName}' was not locked. No action taken.");
                }
            }
            else
            {
                Console.WriteLine($"Lock '{lockName}' does not exist. No action taken.");
            }
            return Task.CompletedTask;
        }
    }

    [AibotItem("通用-等待解锁", ActionType = ActionType.CommonServer)]
    public class WaitForUnlock : BaseAibotAction, IAibotAction
    {
        [AibotProperty("锁的名称(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty LockName { get; set; }

        public new async Task Execute(AibotV aibot)
        {
            var lockName = LockName.Value?.ToString() ?? "";
            if (Lock.Exists(lockName))
            {
                var semaphore = Lock.GetOrCreate(lockName);
                if (Lock.IsLocked(lockName))
                {
                    Console.WriteLine($"Waiting for lock '{lockName}' to be released...");
                    await semaphore.WaitAsync();
                    Console.WriteLine($"Lock '{lockName}' was released.");
                    semaphore.Release();  // 立即释放，因为我们只是在等待它被释放
                }
                else
                {
                    Console.WriteLine($"Lock '{lockName}' is not locked. No need to wait.");
                }
            }
            else
            {
                Console.WriteLine($"Lock '{lockName}' does not exist. No action taken.");
            }
        }


    }

}
