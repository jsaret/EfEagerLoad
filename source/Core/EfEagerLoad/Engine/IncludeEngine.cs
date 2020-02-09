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

            if (context.IncludeExecution == IncludeExecution.Skip) { return query; }

            // Bit of a cheat as will stop the code from doing any Includes without Entity Framework being attached to a Query or going through missions ...
            // Was impeding various evaluations & comparisons so out for now.
            // Maybe worth while doing a build directive to include for Published builds?
            //if (!(query.Provider is EntityQueryProvider)) { return query; }

            if (context.RootType == null)
            {
                context.RootType = typeof(TEntity);
            }
            
            var includePaths = GetPreFilteredIncludePaths(context);
            
            return GetQueryableWithIncludePaths(query, context, includePaths);
        }

        private IEnumerable<string> GetPreFilteredIncludePaths(EagerLoadContext context)
        {
            switch (context.IncludeExecution)
            {
                case IncludeExecution.Cached:
                {
                    return CachedIncludePaths.GetOrAdd(context.RootType, (type) =>
                        _includeFinder.BuildIncludePathsForRootType(context));
                }
                case IncludeExecution.UseOnlyCache:
                {
                    return CachedIncludePaths.ContainsKey(context.RootType) ? CachedIncludePaths.GetValueOrDefault(context.RootType) : 
                            _includeFinder.BuildIncludePathsForRootType(context);
                }
                case IncludeExecution.NoCache:
                {
                    return _includeFinder.BuildIncludePathsForRootType(context);
                }
                case IncludeExecution.Recache:
                {
                    var includeStatements = _includeFinder.BuildIncludePathsForRootType(context);
                    CachedIncludePaths.TryAdd(context.RootType, includeStatements);
                    return includeStatements;
                }
                default: throw new ArgumentOutOfRangeException();
            }
        }

        private static IQueryable<TEntity> GetQueryableWithIncludePaths<TEntity>(IQueryable<TEntity> query, 
            EagerLoadContext context, IEnumerable<string> includeStatements) where TEntity : class
        {
            if (context.IncludePathsToInclude.Count == 0) { return query; }

            context.IncludeStrategy.FilterIncludePathsBeforeInclude(context);
            context.IncludeStrategy.ExecuteBeforeInclude(context);

            return includeStatements.Aggregate(query, (current, navigationPath) => current.Include(navigationPath));
        }
    }
}
