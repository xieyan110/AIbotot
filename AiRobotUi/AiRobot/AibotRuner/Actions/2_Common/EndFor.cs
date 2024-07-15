using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Aibot
{
    [AibotItem("EndFor", ActionType = ActionType.CommonServer)]
    public class EndFor : IAibotAction
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
}
