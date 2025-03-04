using System;
using System.Threading.Tasks;

namespace Aibot
{
    public class Transition
    {
        public Transition(Guid from, Guid to, IAibotCondition? condition = default)
        {
            From = from;
            To = to;
            Condition = condition;
        }

        public Guid From { get; }
        public Guid To { get; }
        public IAibotCondition? Condition { get; }

        public virtual Task<bool> CanActivate(AibotV Aibot)
            => Condition?.Evaluate(Aibot) ?? Task.FromResult(true);
    }
}
