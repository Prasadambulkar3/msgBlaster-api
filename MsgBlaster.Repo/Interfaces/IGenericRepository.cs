using MsgBlaster.Domain.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;

namespace MsgBlaster.Repo.Interfaces
{
    public interface IGenericRepository<TEntity> where TEntity : class, IEntity
    {
        IEnumerable<TEntity> Get(
           Expression<Func<TEntity, bool>> filter = null,
           Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
           string includeProperties = "", int skip = 0, int take = 0);

        IEnumerable<TEntity> GetAll(int skip = 0, int take = 0);
        TEntity GetById(int id);

        void Insert(TEntity entity);
        void Delete(int id);
        void Delete(TEntity entityToDelete);
        void DeleteMany(IList<TEntity> entitiesToDelete);
        void Update(TEntity entityToUpdate);
    }
}
