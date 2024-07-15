using Nodify;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;
using System.Windows.Forms;
using System.Windows.Media.TextFormatting;
using WindowsAPI;


namespace Aibot
{
    [AibotItem("文件-读取", ActionType = ActionType.WindowsServer)]
    public class FileRead : BaseAibotAction, IAibotAction
    {
        [AibotProperty("文件路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FilePath { get; set; }

        [AibotProperty("结果(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }
        public new Task Execute(AibotV blackboard)
        {
            var file = FilePath.Value?.ToString() ?? "file.txt";
            var text = "";
            if (File.Exists(file))
            {
                var allText = File.ReadAllLines(file);
                var last500Items = allText.Skip(Math.Max(0, allText.Count() - 500)).Take(500).ToList();

                text = string.Join("\n", last500Items);
            }
            else
            {
                using (FileStream fs = File.Create(file))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes(text);
                    fs.Write(info, 0, info.Length);
                }
            }

            blackboard.Node!.Output.ForEach(x =>
        {
            if (x.PropertyName == "Result") x.Value = text;

        });
            return Task.CompletedTask;
        }
    }

    [AibotItem("文件-重写", ActionType = ActionType.WindowsServer)]
    public class FileRewrite : BaseAibotAction, IAibotAction
    {
        [AibotProperty("文件路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FilePath { get; set; }
        [AibotProperty("内容(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var file = FilePath.Value?.ToString() ?? "file.txt";
            var text = Text.Value?.ToString() ?? "";
            File.WriteAllText(file, text);

            return Task.CompletedTask;
        }
    }

    [AibotItem("文件-追加写入", ActionType = ActionType.WindowsServer)]
    public class FileAddEnd : BaseAibotAction, IAibotAction
    {
        [AibotProperty("文件路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FilePath { get; set; }
        [AibotProperty("内容(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }
        [AibotProperty("结果(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var file = FilePath.Value?.ToString() ?? "file.txt";
            var text = Text.Value?.ToString() ?? "";
            var result = "";
            if (!File.Exists(file))
            {
                using (FileStream fs = File.Create(file))
                {
                    byte[] info = new UTF8Encoding(true).GetBytes("");
                    fs.Write(info, 0, info.Length);
                }
                result = text;
            }
            else
            {
                result = File.ReadAllText(file);
                result = $"{result}\n{text}";
            }

            File.WriteAllText(file, result);


            blackboard.Node!.Output.ForEach(x =>
            {
                if (x.PropertyName == "Result") x.Value = result;

            });

            return Task.CompletedTask;
        }
    }
}
