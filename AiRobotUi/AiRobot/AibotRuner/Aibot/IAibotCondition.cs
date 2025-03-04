using System.Threading.Tasks;

namespace Aibot
{
    public interface IAibotCondition
    {
        Task<bool> Evaluate(AibotV Aibot);
    }
}
