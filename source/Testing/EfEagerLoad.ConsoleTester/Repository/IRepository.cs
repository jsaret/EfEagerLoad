﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading.Tasks;
using EfEagerLoad.ConsoleTester.Model;

namespace EfEagerLoad.ConsoleTester.Repository
{
    public interface IRepository
    {
        Task<TEntity> GetById<TEntity>(long id, bool eagerLoad = true) where TEntity : class, IEntity;

        Task<IList<TEntity>> GetAll<TEntity>(bool eagerLoad = true) where TEntity : class;

        Task<IList<TEntity>> GetAllMatching<TEntity>(Expression<Func<TEntity, bool>> predicate, bool eagerLoad = true) where TEntity : class;

        Task<TEntity> GetFirstMatching<TEntity>(Expression<Func<TEntity, bool>> predicate, bool eagerLoad = true) where TEntity : class;

        IQueryable<TEntity> Query<TEntity>(bool eagerLoad = true) where TEntity : class;
    }
}
