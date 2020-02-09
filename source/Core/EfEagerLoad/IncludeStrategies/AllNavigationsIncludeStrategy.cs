using System;
using System.Linq;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategies
{
    public class AllNavigationsIncludeStrategy : IncludeStrategy
    {
        public override bool ShouldIncludeNavigation(EagerLoadContext context)
        {
            return !context.IncludePathsToIgnore.Any(context.CurrentIncludePath.StartsWith);
        }
    }
}
