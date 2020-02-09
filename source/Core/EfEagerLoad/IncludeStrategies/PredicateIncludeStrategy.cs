using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategies
{
    public class PredicateIncludeStrategy : IncludeStrategy
    {
        private readonly Predicate<EagerLoadContext> _strategy;

        public PredicateIncludeStrategy(Predicate<EagerLoadContext> strategy)
        {
            Guard.IsNotNull(nameof(strategy), strategy);
            _strategy = strategy;
        }

        public PredicateIncludeStrategy(Predicate<string> strategy)
        {
            Guard.IsNotNull(nameof(strategy), strategy);
            _strategy = (context) => strategy(context.CurrentIncludePath);
        }

        public override bool ShouldIncludeCurrentNavigation(EagerLoadContext context)
        {
            return _strategy(context);
        }
    }
}
