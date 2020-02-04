using System;
using System.Linq;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadMatchingPredicateExtensions
    {

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, includeStrategyPredicate, true, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, bool eagerLoad, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, includeStrategyPredicate, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip),
                navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            var includeStrategy = new PredicateIncludeStrategy(includeStrategyPredicate);
            return originalQuery.EagerLoadMatching(dbContext, includeStrategy, includeExecution, navigationPropertiesToIgnore);
        }

    }
}
