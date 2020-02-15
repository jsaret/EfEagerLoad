using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Engine
{
    public class QueryIncluder
    {
        public IQueryable<TEntity> GetQueryableWithIncludePaths<TEntity>(IQueryable<TEntity> query, EagerLoadContext context) 
            where TEntity : class
        {
            if (context.IncludePathsToInclude.Count == 0) { return query; }

            context.IncludeStrategy.FilterIncludePathsBeforeInclude(context);
            context.IncludeStrategy.ExecuteBeforeInclude(context);

            return context.IncludePathsToInclude.Aggregate(query, (current, navigationPath) => current.Include(navigationPath));
        }
    }
}
