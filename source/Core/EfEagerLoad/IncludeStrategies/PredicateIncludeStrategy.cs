using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategies
{
    public class PredicateIncludeStrategy : IncludeStrategy
    {
        private readonly Func<EagerLoadContext, string, bool> _strategy;

        public PredicateIncludeStrategy(Func<EagerLoadContext, string, bool> strategy)
        {
            Guard.IsNotNull(nameof(strategy), strategy);
            _strategy = strategy;
        }

        public override bool ShouldIncludeNavigation(EagerLoadContext context, string navigationPath)
        {
            return _strategy(context, navigationPath);
        }
    }
}
