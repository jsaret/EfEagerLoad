using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;

namespace EfEagerLoad.IncludeStrategies
{
    public abstract class IncludeStrategy : IIncludeStrategy
    {
        public abstract bool ShouldIncludeNavigation(EagerLoadContext context, string navigationPath);

        public void FilterNavigationPathsBeforeInclude(EagerLoadContext context)
        {
            foreach (var navigationPath in context.NavigationPathsToInclude.ToArray())
            {
                if (context.NavigationPathsToIgnore.Any(nav => navigationPath.StartsWith(nav)))
                {
                    context.NavigationPathsToInclude.Remove(navigationPath);
                }
            }
        }

        public void ExecuteBeforeInclude(EagerLoadContext context)
        {
            //foreach (var item in context.NavigationPathsFoundToInclude) { Console.WriteLine(item); }
        }
    }
}
