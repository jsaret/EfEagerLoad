using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadMatchingContextPredicateExtensions
    {

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includeStrategyPredicate, true, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, bool eagerLoad, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includeStrategyPredicate, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), 
                                            navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            var includeStrategy = new PredicateIncludeStrategy(includeStrategyPredicate);
            return query.EagerLoadWithStrategy(dbContext, includeStrategy, includeExecution, navigationPropertiesToIgnore);
        }

    }
}
