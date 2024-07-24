using Nodify;
using OpenCvSharp;
using System;
using System.IO;
using System.Threading.Tasks;


namespace Aibot
{
    [AibotItem("定位图片", ActionType = ActionType.CommonServer)]
    public class GpsImage : IF,IAibotAction
    {
        [AibotProperty("大图(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty TempPath { get; set; }
        
        [AibotProperty("小图(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty WaferPath { get; set; }
        
        [AibotProperty("相似度(小数)", AibotKeyType.Double, Usage=AibotKeyUsage.Input)]
        public AibotProperty MatchVal { get; set; }

        [AibotProperty("X(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Output)]
        public AibotProperty X { get; set; }
        [AibotProperty("Y(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Output)]
        public AibotProperty Y { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            try
            {
                var tempPath = TempPath.Value?.ToString() ?? "";
                var waferPath = WaferPath.Value?.ToString() ?? "";
                var matchVal = MatchVal.Value?.Case<double>() ?? 0;
                matchVal = matchVal == 0 ? 0.8 : matchVal;

                Mat temp = new Mat(tempPath, ImreadModes.AnyColor);
                // matched graph
                Mat wafer = new Mat(waferPath, ImreadModes.AnyColor);


                double minVal, maxVal;
                Point minLoc, maxLoc;
                var result = temp.MatchTemplate(wafer, TemplateMatchModes.CCorrNormed);

                result.MinMaxLoc(out minVal, out maxVal, out minLoc, out maxLoc);
                maxLoc.X = maxLoc.X + (int)(wafer.Rows / 2);
                maxLoc.Y = maxLoc.Y + (int)(wafer.Width / 2);
                if (maxVal < matchVal)
                    (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);
                else
                    (blackboard["IsSuccess"], blackboard["IsError"]) = (true, false);

                blackboard.Node!.Output.ForEach(x =>
                {
                    if (x.PropertyName == "X") x.Value = maxLoc.X;
                    if (x.PropertyName == "Y") x.Value = maxLoc.Y;

                });
            }
            catch
            {
                (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);
            }
           

            return Task.CompletedTask;
        }
    }
}
