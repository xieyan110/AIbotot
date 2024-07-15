using Nodify;
using OpenCvSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using WindowsAPI;


namespace Aibot
{
    [AibotItem("桌面截图", ActionType = ActionType.WindowsServer)]
    public class ScreenshotDesktop : BaseAibotAction,IAibotAction
    {

        [AibotProperty("文件路径(String)", AibotKeyType.String, Usage=AibotKeyUsage.Output)]
        public AibotProperty FilePath { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var bmp = Desktop.Screenshot();

            var localDir = Path.Combine(Directory.GetCurrentDirectory(), "File");

            if (!Directory.Exists(localDir))
            {
                Directory.CreateDirectory(localDir);
            }

            string savePath = Path.Combine(localDir, "Desktop.png");

            bmp.Save(savePath);

            blackboard[FilePath] = savePath;
            blackboard.Node!.Output.ForEach(n => 
            {
                if("FilePath" == n.PropertyName)
                    n.Value = savePath;
            });
            return Task.CompletedTask;
        }
    }
}
