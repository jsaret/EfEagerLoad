using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using Microsoft.Extensions.Logging;

namespace EfEagerLoad.IncludeStrategies
{
    public abstract class IncludeStrategy : IIncludeStrategy
    {
        private const string IncludeLogMessage = "EfEagerLoad is about to include the following paths: ";

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
            var logger = context.ServiceProvider?.GetService(typeof(ILogger<IncludeEngine>)) as ILogger<IncludeEngine>;
            logger?.LogDebug(IncludeLogMessage, context.IncludePathsToInclude);
        }
    }
}
