using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategy
{
    public class PredicateIncludeStrategy : IIncludeStrategy
    {
        private readonly Predicate<EagerLoadContext> _strategy;

        public PredicateIncludeStrategy(Predicate<EagerLoadContext> strategy)
        {
            Guard.IsNotNull(nameof(strategy), strategy);
            _strategy = strategy;
        }

        public bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            return _strategy(context);
        }
    }
}
