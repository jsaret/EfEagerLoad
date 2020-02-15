using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using EfEagerLoad.Common;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Engine
{
    internal class QueryIncluder
    {
        internal virtual IQueryable<TEntity> GetQueryableWithIncludePaths<TEntity>(IQueryable<TEntity> query, EagerLoadContext context) 
            where TEntity : class
        {
            return context.IncludePathsToInclude.Aggregate(query, (current, navigationPath) => current.Include(navigationPath));
        }
    }
}
