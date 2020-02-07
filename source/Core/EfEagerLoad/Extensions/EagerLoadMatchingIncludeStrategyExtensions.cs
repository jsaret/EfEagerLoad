using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadMatchingIncludeStrategyExtensions
    {

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, IIncludeStrategy includeStrategy,
                                         params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includeStrategy, true, navigationPropertiesToIgnore);
        }


        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, IIncludeStrategy includeStrategy,
                                                                    bool eagerLoad, params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includeStrategy, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), 
                                                    navigationPropertiesToIgnore);
        }


        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext, IIncludeStrategy includeStrategy, 
                                        IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            var context = new EagerLoadContext(dbContext, includeStrategy,navigationPropertiesToIgnore, includeExecution, typeof(TEntity));
            return query.EagerLoadWithContext(context);
        }

    }
}
