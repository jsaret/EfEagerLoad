using System;
using System.Linq;
using EfEagerLoad.Common;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadMatchingPredicateExtensions
    {

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includeStrategyPredicate, true, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Func<EagerLoadContext, string, bool> funcIncludeStrategy, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, funcIncludeStrategy, true, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, bool eagerLoad, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, includeStrategyPredicate, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), 
                                            navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Func<EagerLoadContext, string, bool> funcIncludeStrategy, bool eagerLoad, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, funcIncludeStrategy, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip),
                navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Predicate<EagerLoadContext> includeStrategyPredicate, IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            return query.EagerLoadMatching(dbContext, (context, path) => includeStrategyPredicate(context),
                                            includeExecution, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> query, DbContext dbContext,
            Func<EagerLoadContext, string, bool> funcIncludeStrategy, IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            var includeStrategy = new PredicateIncludeStrategy(funcIncludeStrategy);
            return query.EagerLoadMatching(dbContext, includeStrategy, includeExecution, navigationPropertiesToIgnore);
        }
    }
}
