using System;
using System.Linq;
using EfEagerLoad.Engine;
using EfEagerLoad.IncludeStrategies;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Extensions
{
    public static class EagerLoadForAttributeExtensions
    {
        public static IQueryable<TEntity> EagerLoadForAttribute<TEntity, TAttribute>(this IQueryable<TEntity> query, DbContext dbContext, 
                                                                                params string[] navigationPropertiesToIgnore)
                                                                                where TEntity : class
                                                                                where TAttribute : Attribute
        {
            return query.EagerLoadForAttribute<TEntity, TAttribute>(dbContext, true, navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadForAttribute<TEntity, TAttribute>(this IQueryable<TEntity> query, DbContext dbContext, bool eagerLoad,
                                                                                params string[] navigationPropertiesToIgnore)
                                                                                where TEntity : class
                                                                                where TAttribute : Attribute
        {
            return query.EagerLoadForAttribute<TEntity, TAttribute>(dbContext, (eagerLoad ? IncludeExecution.Cached : IncludeExecution.Skip), 
                                                                            navigationPropertiesToIgnore);
        }

        public static IQueryable<TEntity> EagerLoadForAttribute<TEntity, TAttribute>(this IQueryable<TEntity> query, DbContext dbContext, 
                                                                                IncludeExecution includeExecution, params string[] navigationPropertiesToIgnore)
                                                                                where TEntity : class
                                                                                where TAttribute : Attribute
        {
            return query.EagerLoadWithStrategy(dbContext, new AttributeExistsIncludeStrategy<TAttribute>(), includeExecution, navigationPropertiesToIgnore);
        }

    }
}
