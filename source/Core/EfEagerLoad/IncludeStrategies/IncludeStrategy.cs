using System;
using System.Linq;
using System.Text.Json;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using Microsoft.Extensions.Logging;

namespace EfEagerLoad.IncludeStrategies
{
    public abstract class IncludeStrategy : IIncludeStrategy
    {
        private const string IncludeLogMessage = "EfEagerLoad is including the following paths for ";

        public abstract bool ShouldIncludeCurrentNavigation(EagerLoadContext context);

        public virtual void FilterIncludePathsBeforeInclude(EagerLoadContext context)
        {
            //var 
            foreach (var pathToExclude in context.IncludePathsToIgnore)
            {
                for (var i = context.IncludePathsToInclude.Count - 1; i >= 0; i--)
                {
                    if (context.IncludePathsToInclude[i].StartsWith(pathToExclude, StringComparison.InvariantCultureIgnoreCase))
                    {
                        context.IncludePathsToInclude.Remove(context.IncludePathsToInclude[i]);
                    }
                }
            }

            //foreach (var navigationPath in context.IncludePathsToInclude.ToArray())
            //{
            //    if (context.IncludePathsToIgnore.Any(nav => navigationPath.StartsWith(nav)))
            //    {
            //        context.IncludePathsToInclude.Remove(navigationPath);
            //    }
            //}
        }

        public virtual void ExecuteBeforeInclude(EagerLoadContext context)
        { 
            var loggerInject = context.ServiceProvider?.GetService(typeof(ILogger<IncludeStrategy>)) ?? 
                               context.ServiceProvider?.GetService(typeof(ILogger));

            if (!(loggerInject is ILogger logger)) { return; }

            var includePaths = JsonSerializer.Serialize(context.IncludePathsToInclude);
            logger.LogInformation($"{IncludeLogMessage}{context.RootType.Name}: {includePaths}");


        }
    }
}
