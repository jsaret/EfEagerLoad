using System;
using EfEagerLoad.Common;

namespace EfEagerLoad.IncludeStrategies
{
    public class AllNavigationsIncludeStrategy : IncludeStrategy
    {
        public override bool ShouldIncludeNavigation(EagerLoadContext context, string navigationPath)
        {
            return true;
        }
    }
}
