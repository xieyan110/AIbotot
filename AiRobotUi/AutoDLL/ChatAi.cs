using OpenAI_API;
using OpenAI_API.Chat;
using OpenAI_API.Models;
using System.Text;

namespace AutoDLL
{
    public enum MessageOwner
    {
        Customer,
        Self,
        Unknown
    }

    public class ChatMessage
    {
        public bool IsCustomer { get; set; }
        public string Text { get; set; }
    }

    public class ChatAi
    {
        public static List<Chat> GetChat(ChatInfo info, ref string userSpeak)
        {

            var openAiApiKey = info.ApiKey;

            APIAuthentication aPIAuthentication = new APIAuthentication(openAiApiKey);
            OpenAIAPI api = new OpenAIAPI(aPIAuthentication);
            api.ApiUrlFormat = info.ApiUrl;


            var chat = api.Chat.CreateConversation();

            chat.Model = Model.GPT4;
            Random random = new Random();
            double randomNumber = random.Next(7, 9) * 0.1;

            chat.RequestParameters.Temperature = randomNumber;
            chat.RequestParameters.TopP = randomNumber;

            /// give instruction as System
            chat.AppendSystemMessage(info.System);

            if ((info.ChatExample?.Count ?? 0) > 0)
            {
                foreach (var example in info.ChatExample!)
                {
                    switch (example.Role)
                    {
                        case ChatRole.Assistant:
                            chat.AppendExampleChatbotOutput(example.Text);
                            break;
                        case ChatRole.User:
                            chat.AppendUserInput(example.Text);
                            break;
                        default:
                            break;
                    }
                }
            }
            info.ChatHistorical ??= new();

            if ((info.ChatHistorical?.Count ?? 0) > 0)
            {
                foreach (var example in info.ChatHistorical!)
                {
                    switch (example.Role)
                    {
                        case ChatRole.Assistant:
                            chat.AppendExampleChatbotOutput(example.Text);
                            break;
                        case ChatRole.User:
                            chat.AppendUserInput(example.Text);
                            break;
                        default:
                            break;
                    }
                }
            }

            if (!string.IsNullOrWhiteSpace(userSpeak))
            {
                chat.AppendUserInput(userSpeak);


                info.ChatHistorical!.Add(new Chat
                {
                    Role = ChatRole.User,
                    Text = userSpeak
                });
            }

            //userSpeak = chat.GetResponseFromChatbotAsync().Result;

            var result = new StringBuilder();
            var task = Task.Run(async() =>
            {
                await foreach (var res in chat.StreamResponseEnumerableFromChatbotAsync())
                {
                    result.Append(res.ToString());
                }
            });

            task.Wait();

            userSpeak = result.ToString();
            info.ChatHistorical!.Add(new Chat
            {
                Role = ChatRole.Assistant,
                Text = userSpeak
            });
            
            return info.ChatHistorical;
        }


        public static List<ChatMessage> ParseChatRecord(List<PPOcrResult> records, int customerLeftX, int customerRightX, int assistantLeftX, int assistantRightX)
        {

            List<ChatMessage> messages = new List<ChatMessage>();
            int largeFrameLeftX = Math.Min(customerLeftX, assistantLeftX) - 10;
            int largeFrameRightX = Math.Max(customerRightX, assistantRightX) + 10;

            int center = (largeFrameLeftX + largeFrameRightX) / 2;
            PPOcrResult? before = null;

            foreach (var record in records)
            {
                if (record.LeftCenterPoint.X < largeFrameLeftX || record.RightCenterPoint.X > largeFrameRightX)
                    continue; // 信息超过大框范围,pass掉

                if (before != null)
                {
                    var adsY = Math.Abs(before.CenterPoint.Y - record.CenterPoint.Y);
                    if (adsY < 50)
                    {
                        messages[^1].Text += $"\n{record.Text}";
                        before = record;
                        continue;
                    }
                }
                // 在客户的框里，不在自己的框里，判断为 客户的信息
                if ((record.LeftCenterPoint.X >= customerLeftX && record.RightCenterPoint.X <= customerRightX)
                    && !(record.LeftCenterPoint.X >= assistantLeftX && record.RightCenterPoint.X <= assistantRightX))
                {
                    messages.Add(new ChatMessage { IsCustomer = true, Text = record.Text });
                    before = record;
                    continue;
                }
                if (!(record.LeftCenterPoint.X >= customerLeftX && record.RightCenterPoint.X <= customerRightX)
                    && (record.LeftCenterPoint.X >= assistantLeftX && record.RightCenterPoint.X <= assistantRightX))
                {
                    messages.Add(new ChatMessage { IsCustomer = false, Text = record.Text });
                    before = record;
                    continue;
                }

                int distanceFromCustomerLeft = Math.Abs((int)record.LeftCenterPoint.X - customerLeftX);
                int distanceFromCustomerRight = Math.Abs((int)record.RightCenterPoint.X - assistantRightX);
                // 过滤中心的信息
                if (!(distanceFromCustomerLeft < 100 || distanceFromCustomerRight < 100))
                    continue;

                int cha = (int)record.CenterPoint.X - center;


                // 中心靠右的是自己的
                if (cha > 100)
                {
                    messages.Add(new ChatMessage { IsCustomer = false, Text = record.Text });
                    before = record;
                    continue;
                }
                else if (cha < -100)
                {
                    messages.Add(new ChatMessage { IsCustomer = true, Text = record.Text });
                    before = record;
                    continue;
                }

                if (Math.Abs(customerLeftX - record.LeftCenterPoint.X) < 10)
                {
                    // 离客户边框距离较近,判定为客户的信息
                    messages.Add(new ChatMessage { IsCustomer = true, Text = record.Text });
                    before = record;
                    continue;
                }
                else
                {
                    // 离自己边框距离较近,判定为自己的信息
                    messages.Add(new ChatMessage { IsCustomer = false, Text = record.Text });
                    before = record;
                    continue;
                }
            }

            return messages;
        }
    }
}
