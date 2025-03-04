using Nodify;
using OpenCvSharp;
using System;
using System.IO;
using System.Threading.Tasks;
using AutoDLL;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Documents;
using Microsoft.KernelMemory;


namespace Aibot
{
    [AibotItem("OpenAi-聊天", ActionType = ActionType.CommonServer)]
    public class AiChat : BaseAibotAction, IAibotAction
    {
        [AibotProperty("ApiUrlFormat(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiUrl { get; set; }

        [AibotProperty("ApiKey(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiKey { get; set; }

        [AibotProperty("Model(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Model { get; set; }

        [AibotProperty("系统提示词(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty System { get; set; }

        [AibotProperty("用户输入(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty UserSpeak { get; set; }

        [AibotProperty("聊天记录(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty OutputChatHistorical { get; set; }

        [AibotProperty("Ai回复(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty AiSpeak { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            try
            {

                var apiUrl = ApiUrl.Value?.ToString() ?? "";
                var apiKey = ApiKey.Value?.ToString() ?? "";
                var model = Model.Value?.ToString() ?? "";
                var system = System.Value?.ToString() ?? "";
                var speak = UserSpeak.Value?.ToString() ?? "";

                var chatInfo = new ChatInfo()
                {
                    ApiUrl = apiUrl,
                    ApiKey = apiKey,
                    Model = model,
                    System = system,
                    ChatExample = new(),
                    ChatHistorical = new(),
                };

                var result = ChatAi.GetChat(chatInfo, ref speak);

                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = speak;
                    if (output.PropertyName == "OutputChatHistorical")
                        output.Value = result.ToJsonString();
                });
            }
            catch
            {
                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = "";
                    if (output.PropertyName == "OutputChatHistorical")
                        output.Value = "{}".ToJsonString();
                });
            }
            return Task.CompletedTask;
        }
    }



    [AibotItem("OpenAi-文档", ActionType = ActionType.CommonServer)]
    public class AiDocument : BaseAibotAction, IAibotAction
    {
        [AibotProperty("ApiUrl(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiUrl { get; set; }

        [AibotProperty("ApiKey(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiKey { get; set; }
        [AibotProperty("模型(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Model { get; set; }
        [AibotProperty("Embedding模型(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Embedding { get; set; }


        [AibotProperty("文字文档(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Document { get; set; }

        [AibotProperty("用户输入(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty UserSpeak { get; set; }

        [AibotProperty("Ai回复(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty AiSpeak { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            try
            {

                var apiUrl = ApiUrl.Value?.ToString() ?? "";
                var apiKey = ApiKey.Value?.ToString() ?? "";
                var document = Document.Value?.ToString() ?? "";
                var speak = UserSpeak.Value?.ToString() ?? "";

                var chatInfo = new ChatInfo()
                {
                    ApiUrl = apiUrl,
                    ApiKey = apiKey,
                    Model = Model.Value?.ToString() ?? "",
                    Embedding = Embedding.Value?.ToString() ?? "",
                    System = "",
                    Document = document,
                };

                var result = await ChatService.GetChatDocumentText(chatInfo, speak);

                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = result;
                });
            }
            catch
            {
                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = "";
                });
            }
        }
    }



    [AibotItem("OpenAi-文件查询", ActionType = ActionType.CommonServer)]
    public class AiDocumentFile : BaseAibotAction, IAibotAction
    {
        [AibotProperty("ApiUrl(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiUrl { get; set; }

        [AibotProperty("ApiKey(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiKey { get; set; }

        [AibotProperty("模型(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Model { get; set; }
        [AibotProperty("Embedding模型(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Embedding { get; set; }


        [AibotProperty("文件路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FilePath { get; set; }

        [AibotProperty("用户输入(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty UserSpeak { get; set; }

        [AibotProperty("Ai回复(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty AiSpeak { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            try
            {

                var apiUrl = ApiUrl.Value?.ToString() ?? "";
                var apiKey = ApiKey.Value?.ToString() ?? "";
                var filePath = FilePath.Value?.ToString() ?? "";
                var speak = UserSpeak.Value?.ToString() ?? "";

                var chatInfo = new ChatInfo()
                {
                    ApiUrl = apiUrl,
                    ApiKey = apiKey,
                    Model = Model.Value?.ToString() ?? "",
                    Embedding = Embedding.Value?.ToString() ?? "",
                    System = "",
                    Document = filePath,
                    WebPageUrl = "",
                };

                var result = await ChatService.GetChatDocumentFiLe(chatInfo, speak);

                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = result;
                });
            }
            catch
            {
                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = "";
                });
            }
        }
    }


    [AibotItem("OpenAi-网页文档", ActionType = ActionType.CommonServer)]
    public class AiDocumentWeb : BaseAibotAction, IAibotAction
    {
        [AibotProperty("ApiUrl(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiUrl { get; set; }

        [AibotProperty("ApiKey(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty ApiKey { get; set; }
        [AibotProperty("模型(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Model { get; set; }
        [AibotProperty("Embedding模型(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Embedding { get; set; }

        [AibotProperty("网页地址(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty WebUrl { get; set; }

        [AibotProperty("用户输入(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty UserSpeak { get; set; }

        [AibotProperty("Ai回复(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty AiSpeak { get; set; }

        public new async Task Execute(AibotV blackboard)
        {
            try
            {

                var apiUrl = ApiUrl.Value?.ToString() ?? "";
                var apiKey = ApiKey.Value?.ToString() ?? "";
                var webUrl = WebUrl.Value?.ToString() ?? "";
                var speak = UserSpeak.Value?.ToString() ?? "";

                var chatInfo = new ChatInfo()
                {
                    ApiUrl = apiUrl,
                    ApiKey = apiKey,
                    Model = Model.Value?.ToString() ?? "",
                    Embedding = Embedding.Value?.ToString() ?? "",
                    System = "",
                    Document = "",
                    WebPageUrl = webUrl,
                };

                var result = await ChatService.GetChatWebPage(chatInfo, speak);

                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = result;
                });
            }
            catch
            {
                blackboard.Node!.Output.ForEach(output =>
                {
                    if (output.PropertyName == "AiSpeak")
                        output.Value = "";
                });
            }
        }
    }

    [AibotItem("服务-1.0解析聊天", ActionType = ActionType.CommonServer)]
    public class ParseChatRecord : BaseAibotAction, IAibotAction
    {
        [AibotProperty("详细记录(JsonData)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty Records { get; set; }

        [AibotProperty("客户LeftX(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty CustomerLeftX { get; set; }

        [AibotProperty("客户RightX(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty CustomerRightX { get; set; }

        [AibotProperty("咱们Left(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty AssistantLeftX { get; set; }

        [AibotProperty("咱们Right(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty AssistantRightX { get; set; }

        [AibotProperty("聊天记录(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty ChatMessage { get; set; }

        [AibotProperty("客户说(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty CustomerSpeak { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var records = Records.Value?.CastTo<List<PPOcrResult>>() ?? new();
            var customerLeftX = CustomerLeftX.Value?.TryInt() ?? 0;
            var customerRightX = CustomerRightX.Value?.TryInt() ?? 0;
            var assistantLeftX = AssistantLeftX.Value?.TryInt() ?? 0;
            var assistantRightX = AssistantRightX.Value?.TryInt() ?? 0;
            var result = ChatAi.ParseChatRecord(records, customerLeftX, customerRightX, assistantLeftX, assistantRightX);

            string customerSpeak = "";
            if (result.Count > 0 && result[^1].IsCustomer)
            {
                customerSpeak = result[^1].Text;
            }

            blackboard.Node!.Output.ForEach(output =>
            {
                if (output.PropertyName == "CustomerSpeak")
                    output.Value = customerSpeak;
                if (output.PropertyName == "ChatMessage")
                    output.Value = result.ToJsonString();
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("服务-2.0解析聊天", ActionType = ActionType.CommonServer)]
    public class ParseSupporChatRecord : BaseAibotAction, IAibotAction
    {
        [AibotProperty("图片路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FilePath { get; set; }

        [AibotProperty("客户气泡颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty CustomerColorStr { get; set; }

        [AibotProperty("咱们气泡颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty AssistantColorStr { get; set; }
        
        [AibotProperty("颜色容差[默认70](Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty ColorTolerance { get; set; }
        
        [AibotProperty("垂直位置容差[默认70](Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty YTolerance { get; set; }


        [AibotProperty("聊天记录(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty ChatMessage { get; set; }

        [AibotProperty("客户说(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty CustomerSpeak { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var colorTolerance = ColorTolerance.Value!.TryInt();
            var yTolerance = YTolerance.Value!.TryInt();
            if (colorTolerance == 0) colorTolerance = 70; 
            if (yTolerance == 0) yTolerance = 70;

            var ppOcr = new PPOcr();
            var result = ppOcr.GetChatMessages(
                FilePath.Value?.ToString() ?? "",
                customerColorStr: CustomerColorStr.Value?.ToString() ?? "",  // 白色
                assistantColorStr: AssistantColorStr.Value?.ToString() ?? "",  // 浅绿色
                colorTolerance: colorTolerance,  // 颜色容差
                yTolerance: yTolerance  // 垂直位置容差
            );

            string customerSpeak = "";
            if (result.Count > 0 && result[^1].IsCustomer)
            {
                customerSpeak = result[^1].Text;
            }

            blackboard.Node!.Output.ForEach(output =>
            {
                if (output.PropertyName == "CustomerSpeak")
                    output.Value = customerSpeak;
                if (output.PropertyName == "ChatMessage")
                    output.Value = result.ToJsonString();
            });

            return Task.CompletedTask;
        }
    }

    [AibotItem("服务-2.1解析聊天", ActionType = ActionType.CommonServer)]
    public class ParseSupporPlusChatRecord : BaseAibotAction, IAibotAction
    {
        [AibotProperty("图片路径(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty FilePath { get; set; }
        public AibotProperty Records { get; set; }

        [AibotProperty("聊天框左侧X(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty LeftX { get; set; }

        [AibotProperty("聊天框右侧X(Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty RightX { get; set; }

        [AibotProperty("客户气泡颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty CustomerColorStr { get; set; }

        [AibotProperty("咱们气泡颜色(String)", AibotKeyType.String, Usage = AibotKeyUsage.Input)]
        public AibotProperty AssistantColorStr { get; set; }

        [AibotProperty("颜色容差[默认70](Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty ColorTolerance { get; set; }

        [AibotProperty("垂直位置容差[默认70](Int)", AibotKeyType.Integer, Usage = AibotKeyUsage.Input)]
        public AibotProperty YTolerance { get; set; }


        [AibotProperty("聊天记录(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty ChatMessage { get; set; }

        [AibotProperty("客户说(String)", AibotKeyType.String, Usage = AibotKeyUsage.Output)]
        public AibotProperty CustomerSpeak { get; set; }

        public new Task Execute(AibotV blackboard)
        {
            var colorTolerance = ColorTolerance.Value!.TryInt();
            var yTolerance = YTolerance.Value!.TryInt();
            if (colorTolerance == 0) colorTolerance = 70;
            if (yTolerance == 0) yTolerance = 70;

            var ppOcr = new PPOcr();
            var result = ppOcr.GetChatMessages(
                LeftX.Value!.TryInt(),
                RightX.Value!.TryInt(),
                FilePath.Value?.ToString() ?? "",
                customerColorStr: CustomerColorStr.Value?.ToString() ?? "",  // 白色
                assistantColorStr: AssistantColorStr.Value?.ToString() ?? "",  // 浅绿色
                colorTolerance: colorTolerance,  // 颜色容差
                yTolerance: yTolerance  // 垂直位置容差
            );

            string customerSpeak = "";
            if (result.Count > 0 && result[^1].IsCustomer)
            {
                customerSpeak = result[^1].Text;
            }

            blackboard.Node!.Output.ForEach(output =>
            {
                if (output.PropertyName == "CustomerSpeak")
                    output.Value = customerSpeak;
                if (output.PropertyName == "ChatMessage")
                    output.Value = result.ToJsonString();
            });

            return Task.CompletedTask;
        }
    }

}
