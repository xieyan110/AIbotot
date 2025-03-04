using System.Drawing;

namespace AutoDLL
{
    public enum ChatRole
    {
        System,
        User,
        Assistant,
    }
    public class ChatInfo
    {

        public string ApiUrl { get; set; } = "http://localhost:1234/{0}/{1}";
        public string ApiKey { get; set; } = "lm-studio";
        public string Model { get; set; } = "gemma";
        public string Embedding { get; set; } = "text-embedding-v2";
        public string System { get; set; } = "你正在和user聊天，你的名字叫莉莉";
        public string Document { get; set; } = "";
        public string WebPageUrl { get; set; } = "";

        public List<Chat> ChatExample { get; set; } = new();
        public List<Chat> ChatHistorical { get; set; } = new();
    }

    public class Chat
    {
        public ChatRole Role { get; set; }
        public string Text { get; set; } = string.Empty;
    }

}
