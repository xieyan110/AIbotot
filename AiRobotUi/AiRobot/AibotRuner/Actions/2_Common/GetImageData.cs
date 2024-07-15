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
    [AibotItem("图片Json", ActionType = ActionType.CommonServer)]
    public class GetImageData : BaseAibotAction,IAibotAction
    {
        [AibotProperty("图片(String)", AibotKeyType.String, Usage=AibotKeyUsage.Input)]
        public AibotProperty TempPath { get; set; }
        
        [AibotProperty("JsonData(String)", AibotKeyType.String, Usage=AibotKeyUsage.Output)]
        public AibotProperty JsonData { get; set; }
        
        public new Task Execute(AibotV blackboard)
        {
            var tempPath = TempPath.Value?.ToString() ?? "";

            var pPOcr = new PPOcr();

            var jsonData = pPOcr.GetOcrJson(tempPath);

            blackboard.Node.Output.ForEach(output =>
            {
                if (output.PropertyName == "JsonData")
                    output.Value = jsonData.ToJsonString();
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("图片详细Json", ActionType = ActionType.CommonServer)]
    public class GetChatImageData : BaseAibotAction, IAibotAction
    {
        [AibotProperty("图片(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty TempPath { get; set; }

        [AibotProperty("JsonData(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty JsonData { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var tempPath = TempPath.Value?.ToString() ?? "";

            var pPOcr = new PPOcr();

            var jsonData = pPOcr.GetOcrResults(tempPath).Where(x => !string.IsNullOrEmpty(x.Text)).ToList();

            blackboard.Node.Output.ForEach(output =>
            {
                if (output.PropertyName == "JsonData")
                    output.Value = jsonData.ToJsonString();
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("图片文字", ActionType = ActionType.CommonServer)]
    public class GetImageString : BaseAibotAction, IAibotAction
    {
        [AibotProperty("图片(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty TempPath { get; set; }

        [AibotProperty("文字(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var tempPath = TempPath.Value?.ToString() ?? "";

            var pPOcr = new PPOcr();

            var jsonData = pPOcr.GetOcrResults(tempPath).Where(x => !string.IsNullOrEmpty(x.Text)).ToList();

            var text = string.Join("\n", jsonData.Where(x => x.Score > 0.7).Select(x => x.Text));

            blackboard.Node.Output.ForEach(output =>
            {
                if (output.PropertyName == "Text")
                    output.Value = text;
            });
            return Task.CompletedTask;
        }
    }

    [AibotItem("图片宽高", ActionType = ActionType.CommonServer)]
    public class GetImageWH : BaseAibotAction, IAibotAction
    {
        [AibotProperty("图片(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty TempPath { get; set; }

        [AibotProperty("宽(Int)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Width { get; set; }

        [AibotProperty("高(Int)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Height { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var tempPath = TempPath.Value?.ToString() ?? "";

            int width = 0;
            int height = 0;
            if (File.Exists(tempPath))
            {
                using (Image image = Image.FromFile(tempPath))
                {
                    width = image.Width;
                    height = image.Height;
                }
            }

            blackboard.Node.Output.ForEach(output =>
            {
                if (output.PropertyName == "Width")
                    output.Value = width;
                else if (output.PropertyName == "Height")
                    output.Value = height;
            });

            return Task.CompletedTask;
        }
    }
}
