using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;

namespace EfEagerLoad.Extensions
{
    internal static class EagerLoadWithContextExtensions
    {
        internal static IncludeEngine CachedIncludeEngine = new IncludeEngine();

        internal static IQueryable<TEntity> EagerLoadWithContext<TEntity>(this IQueryable<TEntity> query, EagerLoadContext context)
            where TEntity : class
        {
            return CachedIncludeEngine.RunIncludesForType(query, context);
        }
    }
}
