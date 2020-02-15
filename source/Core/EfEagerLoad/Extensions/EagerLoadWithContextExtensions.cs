using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadWithContextExtensions
    {
        private static readonly IncludeEngine CachedIncludeEngine = new IncludeEngine();

        internal static IQueryable<TEntity> EagerLoadWithContext<TEntity>(this IQueryable<TEntity> query, EagerLoadContext context)
            where TEntity : class
        {
            return CachedIncludeEngine.RunIncludesForType(query, context);
        }
    }
}
