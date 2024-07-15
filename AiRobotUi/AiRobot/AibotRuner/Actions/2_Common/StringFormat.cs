using AutoDLL;
using Nodify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Documents;

namespace Aibot
{

    [AibotItem("通用-字符串格式化", ActionType = ActionType.CommonServer)]
    public class StringFormat : BaseAibotAction, IAibotAction
    {
        [AibotProperty("格式模板(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Template { get; set; }

        [AibotProperty("Text(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        [AibotProperty("结果(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }


        public new Task Execute(AibotV blackboard)
        {
            string template = Template.Value?.ToString() ?? "";
            string text = Text.Value?.ToString() ?? "";

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Result")
                    node.Value = string.Format(template, text);
            });

            return Task.CompletedTask;
        }

    }

    [AibotItem("List-聊天整合", ActionType = ActionType.CommonServer)]
    public class ChatJsonToList : BaseAibotAction, IAibotAction
    {
        [AibotProperty("详细图片Json(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Template { get; set; }

        [AibotProperty("开头Text(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Top { get; set; }

        [AibotProperty("结束Text(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty End { get; set; }

        [AibotProperty("结果Json(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }


        public new Task Execute(AibotV blackboard)
        {
            var template = Template.Value?.CastTo<List<PPOcrResult>>() ?? new();
            string top = Top.Value?.ToString() ?? "";
            string end = End.Value?.ToString() ?? "";

            var chat = new List<string>();
            var group = "";
            var isSelectTop = false;

            foreach (var item in template)
            {
                if (item.Text.Contains(top))
                {
                    if (isSelectTop)
                    {
                        group = "";
                    }
                    group += item.Text;
                    isSelectTop = true;
                }else if (item.Text.Contains(end))
                {
                    isSelectTop = false;
                    chat.Add(group);
                    group = "";
                }
                else if (isSelectTop)
                {
                    group += item.Text;
                }
            }

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Result")
                    node.Value = chat.ToJsonString();
            });

            return Task.CompletedTask;
        }

    }

    [AibotItem("Lsit-新增", ActionType = ActionType.CommonServer)]
    public class ListAdd : BaseAibotAction, IAibotAction
    {
        [AibotProperty("Json数据(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Template { get; set; }

        [AibotProperty("Text(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        [AibotProperty("结果Json(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var template = Template.Value?.CastTo<List<string>>() ?? new();
            string text = Text.Value?.ToString() ?? "";

            template.Add(text);

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Result")
                    node.Value = template.ToJsonString();
            });

            return Task.CompletedTask;
        }
    }


    [AibotItem("Lsit-添加并去重", ActionType = ActionType.CommonServer)]
    public class Listx : BaseAibotAction, IAibotAction
    {
        [AibotProperty("原始数据Json(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty BaseList { get; set; }

        [AibotProperty("输入Json(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty InputJson { get; set; }

        [AibotProperty("结果Json(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty Result { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var baseList = BaseList.Value?.CastTo<List<string>>() ?? new();
            var input = InputJson.Value?.CastTo<List<string>>() ?? new();
            foreach (string item in input)
            {
                if (!baseList.Contains(item))
                {
                    baseList.Add(item);
                }
            }

            blackboard.Node!.Output.ForEach(node =>
            {
                if (node.PropertyName == "Result")
                    node.Value = baseList.ToJsonString();
            });
            return Task.CompletedTask;
        }
    }
}
