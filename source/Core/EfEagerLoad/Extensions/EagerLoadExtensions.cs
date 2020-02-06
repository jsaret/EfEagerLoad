using System;
using System.Linq;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadExtensions
    {
        private static readonly EagerLoadAttributeIncludeStrategy CachedEagerLoadAttributeIncludeStrategy = new EagerLoadAttributeIncludeStrategy();

        
        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, params string[] ignoredNavigationProperties) 
                                                            where TEntity : class
        {
            return query.EagerLoad(dbContext, true, ignoredNavigationProperties);
        }


        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, bool eagerLoad,
                                                        params string[] ignoredNavigationProperties) where TEntity : class
        {
            return query.EagerLoad(dbContext, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), ignoredNavigationProperties);
        }


        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, IncludeExecution includeExecution,
                                                                params string[] ignoredNavigationProperties) where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, CachedEagerLoadAttributeIncludeStrategy, includeExecution, ignoredNavigationProperties);
        }

    }
}
