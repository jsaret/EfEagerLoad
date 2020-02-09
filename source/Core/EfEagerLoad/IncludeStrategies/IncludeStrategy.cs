using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;

namespace EfEagerLoad.IncludeStrategies
{
    public abstract class IncludeStrategy : IIncludeStrategy
    {
        public abstract bool ShouldIncludeNavigation(EagerLoadContext context);

        public virtual void FilterIncludePathsBeforeInclude(EagerLoadContext context)
        {
            foreach (var navigationPath in context.IncludePathsToInclude.ToArray())
            {
                if (context.IncludePathsToIgnore.Any(nav => navigationPath.StartsWith(nav)))
                {
                    context.IncludePathsToInclude.Remove(navigationPath);
                }
            }
        }

        public virtual void ExecuteBeforeInclude(EagerLoadContext context)
        {
            //foreach (var item in context.NavigationPathsToInclude) { Console.WriteLine(item); } //or logger plugin etc...
        }
    }
}
