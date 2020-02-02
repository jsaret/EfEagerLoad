using System;
using System.Linq;
using EfEagerLoad.Attributes;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EfQueryableExtensions
    {
        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext) where TEntity : class
        {
            return originalQuery.EagerLoad(dbContext, true);
        }

        public static IQueryable<TEntity> EagerLoadMatchingAttribute<TEntity, TAttribute>(this IQueryable<TEntity> originalQuery, 
                                                                DbContext dbContext) where TEntity : class
                                                                                    where TAttribute : Attribute
        {
            return originalQuery.EagerLoadMatchingAttribute<TEntity, TAttribute>(dbContext, true);
        }

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext) where TEntity : class
        {
            return originalQuery.EagerLoadAll(dbContext, true);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, 
                                        Predicate<EfEagerLoadContext> navigationMatchPredicate) where TEntity : class
        {
            return originalQuery.EagerLoadAll(dbContext, true);
        }

        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, 
                                                            params string[] ignoredNavigationProperties) where TEntity : class
        {
            return originalQuery.EagerLoad(dbContext, true, ignoredNavigationProperties);
        }

        public static IQueryable<TEntity> EagerLoadMatchingAttribute<TEntity, TAttribute>(this IQueryable<TEntity> originalQuery, 
                                                DbContext dbContext, params string[] ignoredNavigationProperties) where TEntity : class
                                                                                                                where TAttribute : Attribute
        {
            return originalQuery.EagerLoadMatchingAttribute<TEntity, TAttribute>(dbContext, true, ignoredNavigationProperties);
        }

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
            params string[] ignoredNavigationProperties) where TEntity : class
        {
            return originalQuery.EagerLoadAll(dbContext, true, ignoredNavigationProperties);
        }

        public static IQueryable<TEntity> EagerLoadMatching<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext,
                    Predicate<EfEagerLoadContext> navigationMatchPredicate, params string[] ignoredNavigationProperties) where TEntity : class
        {
            return originalQuery.EagerLoad(dbContext, true, ignoredNavigationProperties);
        }

        public static IQueryable<TEntity> EagerLoad<TEntity>(this IQueryable<TEntity> originalQuery, DbContext dbContext, bool eagerLoad,
                                                            params string[] navigationPropertiesToIgnore) where TEntity : class
        {
            return originalQuery.EagerLoadMatchingAttribute<TEntity, EagerLoadAttribute>(dbContext, eagerLoad, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadMatchingAttribute<TEntity, TAttribute>(this IQueryable<TEntity> originalQuery, 
                                                        DbContext dbContext, bool eagerLoad, params string[] navigationPropertiesToIgnore) 
                                                        where TEntity : class
                                                        where TAttribute : Attribute
        {
            if (!eagerLoad) { return originalQuery; }

            Guard.IsNotNull(nameof(dbContext), dbContext);
            Guard.IsNotNull(nameof(dbContext), dbContext);

            if (dbContext == null) { throw new ArgumentNullException(nameof(dbContext)); }

            var eagerLoadContext = new EfEagerLoadContext
            {
                DbContext = dbContext,
                NavigationPropertiesToIgnore = (navigationPropertiesToIgnore ?? new string[0]).ToList(),
                NavigationsToIncludePredicate = (nav) => Attribute.IsDefined(nav.PropertyInfo, typeof(TAttribute))
            };

            var includeFunction = InternalEfQueryableHelper.GetIncludeFunction<TEntity>(eagerLoadContext);
            return includeFunction(originalQuery);
        }

        public static IQueryable<TEntity> EagerLoadAll<TEntity>(this IQueryable<TEntity> originalQuery,
            DbContext dbContext, bool eagerLoad, params string[] navigationPropertiesToIgnore)
            where TEntity : class
        {
            if (!eagerLoad) { return originalQuery; }

            Guard.IsNotNull(nameof(dbContext), dbContext);
            Guard.IsNotNull(nameof(dbContext), dbContext);

            if (dbContext == null) { throw new ArgumentNullException(nameof(dbContext)); }

            var eagerLoadContext = new EfEagerLoadContext
            {
                DbContext = dbContext,
                NavigationPropertiesToIgnore = (navigationPropertiesToIgnore ?? new string[0]).ToList(),
                NavigationsToIncludePredicate = (nav) => true
            };

            var includeFunction = InternalEfQueryableHelper.GetIncludeFunction<TEntity>(eagerLoadContext);
            return includeFunction(originalQuery);
        }


    }
}
