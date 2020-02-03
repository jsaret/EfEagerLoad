using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using Microsoft.EntityFrameworkCore;

namespace EfEagerLoad.Testing.Repository
{
    public class EntityFrameworkRepository : IRepository
    {
        private readonly DbContext _dbContext;

        public EntityFrameworkRepository(DbContext dbContext)
        {
            _dbContext = dbContext;
        }

        public async Task<IList<TEntity>> GetAll<TEntity>(bool eagerLoad = true) where TEntity : class
        {
            return await _dbContext.Set<TEntity>().EagerLoad(_dbContext, eagerLoad).ToListAsync();
        }

        public async Task<IList<TEntity>> GetAllMatching<TEntity>(Expression<Func<TEntity, bool>> predicate, bool eagerLoad = true) where TEntity : class
        {
            return await _dbContext.Set<TEntity>().Where(predicate).EagerLoad(_dbContext, eagerLoad).ToListAsync();
        }

        public async Task<TEntity> GetFirstMatching<TEntity>(Expression<Func<TEntity, bool>> predicate, bool eagerLoad = true) where TEntity : class
        {
            return await _dbContext.Set<TEntity>().Where(predicate).EagerLoad(_dbContext, eagerLoad).FirstOrDefaultAsync();
        }

        public IQueryable<TEntity> Query<TEntity>(bool eagerLoad = false) where TEntity : class
        {
            return _dbContext.Set<TEntity>().EagerLoad(_dbContext, eagerLoad).AsQueryable();
        }
    }
}
