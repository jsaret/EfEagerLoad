using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EfEagerLoad.Builder;
using EfEagerLoad.Common;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadWithContextExtensions
    {
        internal static IQueryable<TEntity> EagerLoadWithContext<TEntity>(this IQueryable<TEntity> originalQuery, EagerLoadContext eagerLoadContext)
            where TEntity : class
        {
            Guard.IsNotNull(nameof(originalQuery), originalQuery);
            Guard.IsNotNull(nameof(eagerLoadContext), eagerLoadContext);

            var includeFunction = EfQueryableInternalFunctionBuilder.GetIncludeFunction<TEntity>(eagerLoadContext);
            return includeFunction(originalQuery);
        }
    }
}
