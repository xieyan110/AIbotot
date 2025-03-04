using System.Threading.Tasks;

namespace Aibot
{
    [AibotItem("打印信息",ActionViewType=ActionViewType.Print, ActionType = ActionType.CommonServer)]
    public class Print : BaseAibotAction,IAibotAction
    {
        [AibotProperty("Source(String)", AibotKeyType.String)]
        public AibotProperty Source { get; set; }

        [AibotProperty("清除(Bool)", AibotKeyType.Boolean)]
        public AibotProperty IsClear { get; set; }

        public new Task Execute(AibotV Aibot)
        {
            var isClear = IsClear.Value.Case<bool>();
            CustomOverlayManager.ClearLog();
            if (isClear)
                Aibot.Node!.Name = "";

            if (Aibot.Node?.Name?.Split('\n').Length > 30)
                Aibot.Node.Name = "";
            
            Aibot.Node!.Name += $"\n{Source.Value}";
            Aibot.Node!.Name = Aibot.Node!.Name.Trim('\n');


            CustomOverlayManager.AddLogMessage(Aibot.Node!.Name);
            return Task.CompletedTask;
        }
    }
}
