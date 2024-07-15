using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using WindowsAPI;


namespace Aibot
{
    [AibotItem("窗口-粘贴", ActionType = ActionType.WindowsServer)]
    public class Paste : BaseAibotAction,IAibotAction
    {
        public new Task Execute(AibotV blackboard)
        {
            SendKeys.SendWait("^V");
            return Task.CompletedTask;
        }
    }
}
