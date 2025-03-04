using Newtonsoft.Json.Linq;
using Nodify;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Media3D;

namespace Aibot
{

    [AibotItem("ForeachJson", ActionViewType = ActionViewType.ForJsonView, ActionType = ActionType.CommonServer)]
    public class ForeachJson : IAibotAction
    {
        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Input)]
        public AibotProperty InputRoot { get; set; }
        
        [AibotProperty("JsonData", AibotKeyType.Object, IsRoot = false, Usage = AibotKeyUsage.Input)]
        public AibotProperty JsonData { get; set; }

        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutputRoot { get; set; }

        public Task Execute(AibotV blackboard)
        {
            (blackboard.Node as Foreach)?.InitEnumerator();
            return Task.CompletedTask;
        }
    }

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

    [AibotItem("LoadJson", ActionViewType = ActionViewType.JsonView, ActionType = ActionType.CommonServer)]
    public class LoadJson : IAibotAction
    {
        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Input)]
        public AibotProperty InputRoot { get; set; }

        [AibotProperty("JsonData", AibotKeyType.Object, IsRoot = false, Usage = AibotKeyUsage.Input)]
        public AibotProperty JsonData { get; set; }

        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutputRoot { get; set; }

        public Task Execute(AibotV blackboard)
        {
            try
            {
                (blackboard.Node as JsonDataViewModel)?.InitLoad();
            }
            catch
            {
                // 小问题
            }

            return Task.CompletedTask;
        }
    }

    [AibotItem("Lsit-获取包含data", ActionViewType = ActionViewType.JsonView, ActionType = ActionType.CommonServer)]
    public class ListExist : IF, IAibotAction
    {
        [AibotProperty("JsonData", AibotKeyType.Object, IsRoot = false, Usage = AibotKeyUsage.Input)]
        public AibotProperty JsonData { get; set; }

        [AibotProperty("Json属性名称", AibotKeyType.Object, Usage = AibotKeyUsage.Input)]
        public AibotProperty Value1 { get; set; }

        [AibotProperty("Text(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Text { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            (blackboard.Node as JsonDataViewModel)?.InitLoad();
            (blackboard["IsSuccess"], blackboard["IsError"]) = (false, true);

            var template = JsonData.Value?.ToString() ?? "";
            string value1 = Value1.Value?.ToString() ?? "";
            string text = Text.Value?.ToString() ?? "";


            JArray jsonArray = JArray.Parse(template);
            JObject objData = new JObject();


            foreach (JObject obj in jsonArray)
            {
                var item = new Dictionary<string, string>();
                foreach (var property in obj.Properties())
                {
                    if(property.Name == value1)
                    {
                        if (property.Value.ToString().Contains(text) || text.Contains(property.Value.ToString()))
                        {
                            objData = obj;
                            (blackboard["IsSuccess"], blackboard["IsError"]) = (true, false);
                            break;
                        }
                    }
                }
            }

            blackboard.Node.Output.ForEach(output =>
            {
                foreach (var property in objData.Properties())
                {
                    if (output.PropertyName == property.Name)
                    {
                        output.Value = property.Value;
                        break;
                    }
                }
            });

            return Task.CompletedTask;

        }
    }


}
