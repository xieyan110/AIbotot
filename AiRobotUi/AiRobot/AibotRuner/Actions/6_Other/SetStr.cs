using Nodify;
using OpenCvSharp;
using System.Threading.Tasks;

namespace Aibot
{
    //[AibotItem("设置数据", ActionViewType = ActionViewType.SetStr, ActionType = ActionType.CommonServer)]
    //public class SetStr : IAibotAction
    //{
    //    [AibotProperty("Str(Obj)", AibotKeyType.Object, Usage = AibotKeyUsage.Output)]
    //    public AibotProperty Str { get; set; }

    //    public new Task Execute(AibotV Aibot)
    //    {
    //        return Task.CompletedTask;
    //    }
    //}

    //[AibotItem("备注", ActionViewType = ActionViewType.SetStr, ActionType = ActionType.CommonServer)]
    //public class Remark : IAibotAction
    //{
    //    [AibotProperty("结果(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
    //    public AibotProperty Result { get; set; }

    //    public new Task Execute(AibotV blackboard)
    //    {
    //        string result = Result.Value?.ToString() ?? "";
    //        blackboard.Node!.Output.ForEach(node =>
    //        {
    //            if (node.PropertyName == "Result")
    //                node.Value = result;
    //        });

    //        return Task.CompletedTask;
    //    }
    //}
}
