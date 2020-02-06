using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadWithContextExtensions
    {
        public static readonly IncludeEngine CachedIncludeEngine = new IncludeEngine();

        internal static IQueryable<TEntity> EagerLoadWithContext<TEntity>(this IQueryable<TEntity> originalQuery, EagerLoadContext eagerLoadContext)
            where TEntity : class
        {
            Guard.IsNotNull(nameof(originalQuery), originalQuery);
            Guard.IsNotNull(nameof(eagerLoadContext), eagerLoadContext);

            //if (!(originalQuery.Provider is EntityQueryProvider)) { return originalQuery; }

            var includeNavigationPaths = CachedIncludeEngine.BuildIncludesForRootType<TEntity>(eagerLoadContext);
            foreach (var item in includeNavigationPaths) { Console.WriteLine(item); }
            return includeNavigationPaths.Aggregate(originalQuery, (current, navigationPath) => current.Include(navigationPath));
        }
    }
}
