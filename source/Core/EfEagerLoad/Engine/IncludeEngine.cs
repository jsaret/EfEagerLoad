using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using EfEagerLoad.Common;

namespace EfEagerLoad.Engine
{
    public class IncludeEngine
    {
        internal static readonly ConcurrentDictionary<Type, IList<string>> CachedIncludePaths = new ConcurrentDictionary<Type, IList<string>>();

        private static readonly IncludeFinder CachedIncludeFinder = new IncludeFinder();
        private static readonly QueryIncluder CachedQueryIncluder = new QueryIncluder();

        private readonly IncludeFinder _includeFinder;
        private readonly QueryIncluder _queryIncluder;


        public IncludeEngine() : this(CachedIncludeFinder, CachedQueryIncluder) { }

        internal IncludeEngine(IncludeFinder includeFinder, QueryIncluder queryIncluder)
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

            SetupPreFilteredIncludePaths(context);

            if (context.IncludePathsToInclude.Count == 0) { return query; }

            context.IncludeStrategy.FilterIncludePathsBeforeInclude(context);
            context.IncludeStrategy.ExecuteBeforeInclude(context);

            return _queryIncluder.GetQueryableWithIncludePaths(query, context);
        }

        private void SetupPreFilteredIncludePaths(EagerLoadContext context)
        {
            switch (context.IncludeExecution)
            {
                case IncludeExecution.Cached:
                {
                    context.IncludePathsToInclude = CachedIncludePaths.GetOrAdd(context.RootType, (type) =>
                        _includeFinder.BuildIncludePathsForRootType(context)?.ToList() ?? new List<string>(0));
                    break;
                }
                case IncludeExecution.ReadOnlyCache:
                {
                    context.IncludePathsToInclude = CachedIncludePaths.ContainsKey(context.RootType) ? 
                            CachedIncludePaths.GetValueOrDefault(context.RootType) : 
                            _includeFinder.BuildIncludePathsForRootType(context);
                    break;
                }
                case IncludeExecution.NoCache:
                {
                    context.IncludePathsToInclude = _includeFinder.BuildIncludePathsForRootType(context);
                    break;
                }
                case IncludeExecution.Recache:
                {
                    context.IncludePathsToInclude = _includeFinder.BuildIncludePathsForRootType(context);
                    CachedIncludePaths.AddOrUpdate(context.RootType,
                        (t) => context.IncludePathsToInclude?.ToList() ?? new List<string>(0),
                        (tv, l) => context.IncludePathsToInclude?.ToList() ?? new List<string>(0));
                    break;
                }
                default: throw new ArgumentOutOfRangeException();
            }

            context.IncludePathsToInclude = (context.IncludePathsToInclude == null) ? 
                new List<string>(0) : 
                new List<string>(context.IncludePathsToInclude);
        }
    }
}
