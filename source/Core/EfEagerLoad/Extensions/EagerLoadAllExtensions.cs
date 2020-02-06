using System;
using System.Linq;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadAllExtensions
    {

        private static readonly AllNavigationsIncludeStrategy NoShadowNavigationIncludeStrategy = new AllNavigationsIncludeStrategy();

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> query, DbContext dbContext) where TEntity : class
        {
            return query.EagerLoadAll(dbContext, true);
        }

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, bool eagerLoad,
                                                                params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return query.EagerLoadAll(dbContext, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, IncludeExecution includeExecution,
                                                        params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, NoShadowNavigationIncludeStrategy, includeExecution, navigationPropertiesToIgnore);
        }
        
    }
}
