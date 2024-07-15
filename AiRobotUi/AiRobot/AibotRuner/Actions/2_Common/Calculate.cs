using Nodify;
using System.Threading.Tasks;
using WindowsAPI;
using AutoDLL;
using System.Linq;
using System;
using System.IO;
using System.Drawing;

namespace Aibot
{
    [AibotItem("算数-基本", ActionType = ActionType.CommonServer)]
    public class BaseCalculate : BaseAibotAction,IAibotAction
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
            }catch
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
}
