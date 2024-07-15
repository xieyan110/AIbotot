using System.Threading.Tasks;

namespace Aibot
{
    [AibotItem("StartRoot",ActionType=ActionType.StartServer)]
    public class StartRoot : IAibotAction
    {
        [AibotProperty("执行", AibotKeyType.Object, IsRoot = true, Usage = AibotKeyUsage.Output)]
        public object? OutputRoot { get; set; }
        
        public Task Execute(AibotV Aibot)
        {
            return Task.CompletedTask;
        }
    }
}
