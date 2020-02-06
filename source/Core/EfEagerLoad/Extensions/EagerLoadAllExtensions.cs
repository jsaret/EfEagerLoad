using System;
using System.Linq;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadAllExtensions
    {

        private static readonly AllNavigationsIncludeStrategy NoShadowNavigationIncludeStrategy = new AllNavigationsIncludeStrategy();

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext) where TEntity : class
        {
            return originalQuery.EagerLoadAll(dbContext, true);
        }

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, bool eagerLoad,
                                                                params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return originalQuery.EagerLoadAll(dbContext, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, IncludeExecution includeExecution,
                                                        params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, NoShadowNavigationIncludeStrategy, includeExecution, navigationPropertiesToIgnore);
        }
        
    }
}
