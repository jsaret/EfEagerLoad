using System;
using System.Linq;
using EfEagerLoad.IncludeStrategy;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadForAttributeExtensions
    {

        private static readonly AllNavigationsIncludeStrategy CachedNoShadowNavigationIncludeStrategy = new AllNavigationsIncludeStrategy();

        public static IQueryable<TEntity> EagerLoadForAttribute<TEntity, TAttribute>(this IQueryable<TEntity> originalQuery, DbContext dbContext, 
                                                                                params string[] navigationPropertiesToIgnore)
                                                                                where TEntity : class
                                                                                where TAttribute : Attribute
        {
            return originalQuery.EagerLoadForAttribute<TEntity, TAttribute>(dbContext, true, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadForAttribute<TEntity, TAttribute>(this IQueryable<TEntity> originalQuery, DbContext dbContext, bool eagerLoad,
                                                                                params string[] navigationPropertiesToIgnore)
                                                                                where TEntity : class
                                                                                where TAttribute : Attribute
        {
            return originalQuery.EagerLoadForAttribute<TEntity, TAttribute>(dbContext, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), 
                                                                            navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadForAttribute<TEntity, TAttribute>(this IQueryable<TEntity> originalQuery, DbContext dbContext, 
                                                                                IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore)
                                                                                where TEntity : class
                                                                                where TAttribute : Attribute
        {
            return originalQuery.EagerLoadMatching(dbContext, CachedNoShadowNavigationIncludeStrategy, includeExecution, navigationPropertiesToIgnore);
        }

    }
}
