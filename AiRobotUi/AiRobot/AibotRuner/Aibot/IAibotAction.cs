using System.Threading.Tasks;

namespace Aibot
{
    public interface IAibotAction
    {
        Task Execute(AibotV blackboard);
    }
}
