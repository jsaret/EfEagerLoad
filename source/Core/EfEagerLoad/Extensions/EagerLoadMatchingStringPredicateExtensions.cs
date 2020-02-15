using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadMatchingStringPredicateExtensions
    {

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<string> includePathPredicate, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includePathPredicate, true, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<string> includePathPredicate, bool eagerLoad, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includePathPredicate, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip),
                navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<string> includePathPredicate, IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            var includeStrategy = new PredicateIncludeStrategy(includePathPredicate);
            return query.EagerLoadWithStrategy(dbContext, includeStrategy, includeExecution, navigationPropertiesToIgnore);
        }

    }
}
