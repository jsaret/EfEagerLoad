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






        /*p


        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
                                                                    Predicate<EagerLoadContext> navigationMatchPredicate) where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, true, navigationMatchPredicate);
        }

        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, 
                                                            params string[] ignoredNavigationProperties) where TEntity : class
        {
            return originalQuery.EagerLoad(dbContext, true, ignoredNavigationProperties);
        }



        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
            params string[] ignoredNavigationProperties) where TEntity : class
        {
            return originalQuery.EagerLoadAll(dbContext, true, ignoredNavigationProperties);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
                            Predicate<EagerLoadContext> shouldIncludeNavigationPredicate, params string[] ignoredNavigationProperties) 
                            where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, true, shouldIncludeNavigationPredicate, ignoredNavigationProperties);
        }

        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, bool eagerLoad,
                                                            params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return originalQuery.EagerLoadMatching(dbContext, eagerLoad, CachedEagerLoadLoadStrategy, navigationPropertiesToIgnore);
        }

        



        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, IncludeExecution includeExecution,
                                Predicate<EagerLoadContext> shouldIncludeNavigationPredicate, params string[] navigationPropertiesToIgnore) 
                                where TEntity : class
        {
            if (!eagerLoad) { return originalQuery; }

            Guard.IsNotNull(nameof(originalQuery), originalQuery);

            var eagerLoadContext = new EagerLoadContext(typeof(TEntity), dbContext, (navigationPropertiesToIgnore ?? new string[0]).ToList(),
                                                            new PredicateEagerLoadStrategy(shouldIncludeNavigationPredicate));

            var includeFunction = EfQueryableInternalFunctionBuilder.GetIncludeFunction<TEntity>(eagerLoadContext);
            return includeFunction(originalQuery);

            return EagerLoadMatching(originalQuery, dbContext, eagerLoad, eagerLoadContext)
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, IncludeExecution includeExecution,
                                                                    IIncludeStrategy includeStrategy, params string[] navigationPropertiesToIgnore)
                                                                    where TEntity : class
        {
            if (!eagerLoad) { return originalQuery; }

            Guard.IsNotNull(nameof(originalQuery), originalQuery);

            var eagerLoadContext = new EagerLoadContext(typeof(TEntity), dbContext, (navigationPropertiesToIgnore ?? new string[0]).ToList(),
                                                        includeStrategy);

            var includeFunction = EfQueryableInternalFunctionBuilder.GetIncludeFunction<TEntity>(eagerLoadContext);
            return includeFunction(originalQuery);
        }*/

    }
}
