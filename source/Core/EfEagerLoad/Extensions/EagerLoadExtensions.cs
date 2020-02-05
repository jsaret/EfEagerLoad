using System;
using System.Linq;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadExtensions
    {
        private static readonly EagerLoadAttributeIncludeStrategy CachedEagerLoadAttributeIncludeStrategy = new EagerLoadAttributeIncludeStrategy();

        
        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, params string[] ignoredNavigationProperties) 
                                                            where TEntity : class
        {
            return originalQuery.EagerLoad(dbContext, true, ignoredNavigationProperties);
        }


        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, bool eagerLoad,
                                                        params string[] ignoredNavigationProperties) where TEntity : class
        {
            return originalQuery.EagerLoad(dbContext, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), ignoredNavigationProperties);
        }


        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, IncludeExecution includeExecution,
                                                                params string[] ignoredNavigationProperties) where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, CachedEagerLoadAttributeIncludeStrategy, includeExecution, ignoredNavigationProperties);
        }

    }
}
