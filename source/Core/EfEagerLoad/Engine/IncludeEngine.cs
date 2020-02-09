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
        private static readonly ConcurrentDictionary<Type, IList<string>> CachedIncludePaths = new ConcurrentDictionary<Type, IList<string>>();

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

            IList<string> includeStatements;

            switch (context.IncludeExecution)
            {
                case IncludeExecution.Cached:
                {
                    includeStatements = CachedIncludePaths.GetOrAdd(context.RootType, (type) =>
                        _includeFinder.BuildIncludePathsForRootType(context));
                    break;
                }
                case IncludeExecution.UseOnlyCache:
                {
                    if (CachedIncludePaths.ContainsKey(context.RootType))
                    {
                        includeStatements = CachedIncludePaths.GetValueOrDefault(context.RootType);
                        break;
                    }

                    includeStatements = _includeFinder.BuildIncludePathsForRootType(context);
                    break;
                }
                case IncludeExecution.NoCache:
                {
                    includeStatements = _includeFinder.BuildIncludePathsForRootType(context);
                    break;
                }
                case IncludeExecution.Recache:
                {
                    includeStatements = _includeFinder.BuildIncludePathsForRootType(context);
                    CachedIncludePaths.TryAdd(context.RootType, includeStatements);
                    break;
                }
                case IncludeExecution.Skip:
                {
                    return query;
                }
                default: throw new ArgumentOutOfRangeException();
            }

            return GetQueryableWithIncludePaths(query, context, includeStatements);
        }

        private static IQueryable<TEntity> GetQueryableWithIncludePaths<TEntity>(IQueryable<TEntity> query, EagerLoadContext context,
                                                                            IList<string> includeStatements) where TEntity : class
        {
            if (context.IncludePathsToInclude.Count == 0) { return query; }

            context.IncludeStrategy.FilterIncludePathsBeforeInclude(context);
            context.IncludeStrategy.ExecuteBeforeInclude(context);

            return includeStatements.Aggregate(query, (current, navigationPath) => current.Include(navigationPath));
        }
    }
}
