using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;

namespace EfEagerLoad.Engine
{
    public class IncludeEngine
    {
        private static readonly ConcurrentDictionary<Type, IList<string>> CachedIncludePaths = new ConcurrentDictionary<Type, IList<string>>();

        private static readonly IncludeFinder CachedIncludeFinder = new IncludeFinder();
        private static readonly QueryIncluder CachedQueryIncluder = new QueryIncluder();

        private readonly IncludeFinder _includeFinder;
        private readonly QueryIncluder _queryIncluder;


        public IncludeEngine() : this(CachedIncludeFinder) { }

        public IncludeEngine(IncludeFinder includeFinder) : this(includeFinder, CachedQueryIncluder) { }

        public IncludeEngine(IncludeFinder includeFinder, QueryIncluder queryIncluder)
        {
            Guard.IsNotNull(nameof(includeFinder), includeFinder);
            Guard.IsNotNull(nameof(queryIncluder), queryIncluder);
            _includeFinder = includeFinder;
            _queryIncluder = queryIncluder;
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
            context.IncludePathsToInclude = includePaths.ToArray();
            
            return _queryIncluder.GetQueryableWithIncludePaths(query, context);
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
    }
}
