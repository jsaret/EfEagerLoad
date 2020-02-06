using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategy
{
    public class PredicateIncludeStrategy : IncludeStrategy
    {
        private readonly Predicate<EagerLoadContext> _strategy;

        public PredicateIncludeStrategy(Predicate<EagerLoadContext> strategy)
        {
            Guard.IsNotNull(nameof(strategy), strategy);
            _strategy = strategy;
        }

        public override bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            return _strategy(context);
        }
    }
}
