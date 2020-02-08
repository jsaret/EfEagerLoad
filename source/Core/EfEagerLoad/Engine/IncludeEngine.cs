using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Engine
{
    public class IncludeEngine
    {
        private static readonly IncludeFinder CachedIncludeBuilder = new IncludeFinder();

        private readonly IncludeFinder _includeFinder;
        private static readonly ConcurrentDictionary<Type, IList<string>> CachedIncludeNavigationPaths = new ConcurrentDictionary<Type, IList<string>>();

        public IncludeEngine() : this(CachedIncludeBuilder) { }

        public IncludeEngine(IncludeFinder includeFinder)
        {
            Guard.IsNotNull(nameof(includeFinder), includeFinder);
            _includeFinder = includeFinder;
        }


        public IQueryable<TEntity> RunIncludesForType<TEntity>(IQueryable<TEntity> query, EagerLoadContext context) where TEntity : class
        {
            Guard.IsNotNull(nameof(query), query);
            Guard.IsNotNull(nameof(context), context);

            if (context.RootType == null)
            {
                context.RootType = typeof(TEntity);
            }

            IList<string> includeStatements = new string[0];

            switch (context.IncludeExecution)
            {
                case IncludeExecution.Cached:
                {
                    includeStatements = CachedIncludeNavigationPaths.GetOrAdd(context.RootType, (type) =>
                        _includeFinder.BuildIncludesForRootType(context));
                    break;
                }
                case IncludeExecution.NoCache:
                {
                    includeStatements = _includeFinder.BuildIncludesForRootType(context);
                    break;
                }
                case IncludeExecution.Recache:
                {
                    includeStatements = _includeFinder.BuildIncludesForRootType(context);
                    CachedIncludeNavigationPaths.TryAdd(context.RootType, includeStatements);
                    break;
                }
            }

            if (context.NavigationPathsToInclude.Count == 0)
            {
                context.NavigationPathsToInclude = new List<string>(includeStatements);
            }

            context.IncludeStrategy.FilterNavigationPathsBeforeInclude(context);
            context.IncludeStrategy.ExecuteBeforeInclude(context);

            return includeStatements.Aggregate(query, (current, navigationPath) => current.Include(navigationPath));
        }
    }
}
