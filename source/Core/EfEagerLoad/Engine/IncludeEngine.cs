using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using EfEagerLoad.Common;
using EfEagerLoad.IncludeStrategy;

namespace EfEagerLoad.Engine
{
    public class IncludeEngine
    {
        private static readonly IncludeFinder CachedIncludeBuilder = new IncludeFinder();

        private readonly IncludeFinder _includeFinder;
        private static readonly ConcurrentDictionary<Type, IEnumerable<string>> CachedIncludeNavigationPaths = new ConcurrentDictionary<Type, IEnumerable<string>>();

        public IncludeEngine() : this(CachedIncludeBuilder) { }

        public IncludeEngine(IncludeFinder includeFinder)
        {
            _includeFinder = includeFinder;
        }

        public IEnumerable<string> BuildIncludesForRootType<TEntity>(EagerLoadContext context)
        {
            if (context.RootType == null)
            {
                context.RootType = typeof(TEntity);
            }

            return BuildIncludesForRootType(context);
        }

        public IEnumerable<string> BuildIncludesForRootType(EagerLoadContext eagerLoadContext)
        {
            Guard.IsNotNull(nameof(EagerLoadContext.RootType), eagerLoadContext.RootType);
            IEnumerable<string> includeStatements = new string[0];

            switch (eagerLoadContext.IncludeExecution)
            {
                case IncludeExecution.Cached:
                {
                    includeStatements = CachedIncludeNavigationPaths.GetOrAdd(eagerLoadContext.RootType, (type) =>
                        _includeFinder.BuildIncludesForRootType(eagerLoadContext));
                    break;
                }
                case IncludeExecution.NoCache:
                {
                    includeStatements = _includeFinder.BuildIncludesForRootType(eagerLoadContext);
                    break;
                }
                case IncludeExecution.Recache:
                {
                    includeStatements = _includeFinder.BuildIncludesForRootType(eagerLoadContext);
                    CachedIncludeNavigationPaths.TryAdd(eagerLoadContext.RootType, includeStatements);
                    break;
                }
            }

            return includeStatements;
        }
    }
}
