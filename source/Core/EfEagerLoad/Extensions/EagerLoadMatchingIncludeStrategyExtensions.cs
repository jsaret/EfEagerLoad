using System;
using System.Linq;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadMatchingIncludeStrategyExtensions
    {

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, IIncludeStrategy includeStrategy,
                                         params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, includeStrategy, true, navigationPropertiesToIgnore);
        }


        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, IIncludeStrategy includeStrategy,
                                                                    bool eagerLoad, params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, includeStrategy, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), 
                                                    navigationPropertiesToIgnore);
        }


        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, IIncludeStrategy includeStrategy, 
                                        IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            var eagerLoadContext = new EagerLoadContext(dbContext, includeStrategy,navigationPropertiesToIgnore, includeExecution, typeof(TEntity));
            return originalQuery.EagerLoadWithContext(eagerLoadContext);
        }

    }
}
