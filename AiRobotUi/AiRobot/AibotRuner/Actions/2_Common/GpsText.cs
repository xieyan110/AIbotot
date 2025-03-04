using Nodify;
using OpenCvSharp;
using System.Threading.Tasks;
using AutoDLL;

namespace Aibot
{
    [AibotItem("定位文字", ActionType = ActionType.CommonServer)]
    public class GpsText : IF,IAibotAction
    {
        [AibotProperty("大图(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty TempPath { get; set; }
        
        [AibotProperty("文字(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }
        
        [AibotProperty("自上而下(Bool)", AibotKeyType.Boolean, Usage=AibotKeyUsage.Input)]
        public AibotProperty IsReverse { get; set; }

        [AibotProperty("Index(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty Index { get; set; }

        [AibotProperty("X(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Output)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Output)]
        public AibotProperty Y { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            try
            {
                var tempPath = TempPath.Value?.ToString() ?? "";
                var text = Text.Value?.ToString() ?? "";
                var isReverse = IsReverse.Value?.Case<bool>() ?? false;
                var index = Index.Value?.TryInt() ?? 1;
                index = index == 0 ? 1 : index;

                var pPOcr = new PPOcr();

                var p = pPOcr.GetTextPoint(tempPath, text, isReverse, index);

                if (p is null)
                    (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);
                else
                {
                    (blackboard["IsSuccess"], blackboard["IsError"]) = (true, false);

                    blackboard.Node!.Output.ForEach(x =>
                    {
                        if (x.PropertyName == "X") x.Value = p.X;
                        if (x.PropertyName == "Y") x.Value = p.Y;
                    });
                }

            }
            catch
            {
                (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);
            }


            return Task.CompletedTask;
        }
    }
}
