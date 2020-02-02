using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;

namespace EfEagerLoad.Testing.Repository
{
    public interface IRepository
    {
        Task<IList<TEntity>> GetAll<TEntity>(bool eagerLoad = true, bool includeDeleted = false) where TEntity : class;

        Task<IList<TEntity>> GetAllMatching<TEntity>(Expression<Func<TEntity, bool>> predicate, bool eagerLoad = true,
                                                        bool includeDeleted = false) where TEntity : class;

        IQueryable<TEntity> Query<TEntity>(bool eagerLoad = true, bool includeDeleted = false) where TEntity : class;
    }
}
