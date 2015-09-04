using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity;
using System.Data.Entity.Infrastructure;
using System.Linq;
using System.Linq.Expressions;
using System.Reflection;
using MsgBlaster.Domain;
using MsgBlaster.Repo.Interfaces;
using MsgBlaster.Domain.Interfaces;

namespace MsgBlaster.Repo
{
    /// <summary>
    /// The Generic repository will implement basic CRUD behaviour. 
    /// Insert will manage new and existing entries.
    /// Update will only update the scalar properties.
    /// </summary>
    /// <typeparam name="TEntity"></typeparam>
    public abstract class GenericRepository<TEntity> : IDisposable, IGenericRepository<TEntity>where TEntity : class, IEntity
    {
         #region INTERNALS
        internal readonly MsgBlasterContext _context;
        protected readonly DbSet<TEntity> _dbSet;
        protected readonly UnitOfWork _uow;

        //protected delegate void BeforeDelete(TEntity entity);
        //protected BeforeDelete _beforeDelete;

        //protected delegate void BeforeUpdate(TEntity entity);
        //protected BeforeUpdate _beforeUpdate;

        //protected delegate void BeforeInsert(TEntity entity);
        //protected BeforeInsert _beforeInsert;
        #endregion

        protected abstract void BeforeInsert(TEntity entity);
        protected abstract void BeforeUpdate(TEntity entity);
        protected abstract void BeforeDelete(TEntity entity);
        
        protected GenericRepository(UnitOfWork uow)
        {
            _context = uow.MsgBlasterContext;
            _dbSet = _context.Set<TEntity>();
            _uow = uow;
        }

        internal UnitOfWork UnitOfWork { get { return _uow; } }

        public virtual IEnumerable<TEntity> Get(
            Expression<Func<TEntity, bool>> filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>> orderBy = null,
            string includeProperties = "", int skip = 0, int take = 0)
        {
            IEnumerable<TEntity> result;
            IQueryable<TEntity> query = _dbSet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                query = orderBy(query);
            }
            else
            {
                query = query.OrderBy(e => e.Id);
            }

            if (take == 0)
            {
                result = query.AsNoTracking().ToList();
            }
            else
            {
                result = query.AsNoTracking().Skip(skip).Take(take).ToList();
            }
            return result;
        }

        public virtual int Count(Expression<Func<TEntity, bool>> filter = null)
        {
            int i;
            if (filter == null) i = _dbSet.Count();
            else i = _dbSet.Count(filter);
            return i;
        }

        public virtual IEnumerable<TEntity> GetAll(int skip = 0, int take = 0)
        {
            //IQueryable<TEntity> query = _dbSet;
            //return query.ToList();

            return Get(null, null, "", skip, take);
        }

        public virtual TEntity GetById(int id)
        {
            var entity = _dbSet.Find(id);
            if (entity == null) return null;
            return entity;
        }

        public virtual TEntity GetById(int id, bool loadAll)
        {
            var entity = GetById(id);
            if (entity == null) return null;
            if (loadAll)
            {
                // load all related properties (refereneces and collections)
                var entry = _context.Entry(entity);
                var references = entry.GetReferences();
                var collections = entry.GetCollections();
                foreach (var item in references)
                {
                    entry.Reference(item).Load();
                }
                if (entry.State != EntityState.Added)
                {
                    //Cannot load collections when entry is in Added state because the FK values are not yet set. They get set when context.SaveChanges() is called.
                    foreach (var item in collections)
                    {                       
                        if (!entry.Collection(item).IsLoaded) entry.Collection(item).Load();
                    }
                }
            }

            return entity;
        }
        public virtual TEntity GetById(int id, string includeProperties)
        {
            IQueryable<TEntity> query = _dbSet.Where(e => e.Id == id);
            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
            var result = query.FirstOrDefault();
            return result;
        }

        public virtual void Insert(TEntity entity)
        {
            if (entity.Id != 0) throw new Exception();

            BeforeInsert(entity);

            try
            {
                _dbSet.Add(entity);

                //Following code is required only when you account for complex graphs where the top level object is new but child objects are existing
                var x = _context.ChangeTracker.Entries().Count();
                foreach (var entry in _context.ChangeTracker.Entries().Where(e => e.State == EntityState.Added))
                {
                    EntityState state = entry.CurrentValues.GetValue<int>("Id") == 0
                        ? EntityState.Added
                        : EntityState.Unchanged;
                    entry.State = state;
                }
            }
            catch (Exception e)
            {
                throw new Exception("Could not insert data", e);
            }
        }

        public virtual void InsertMany(IList<TEntity> entities)
        {
            foreach (var entity in entities)
            {
                Insert(entity);
            }
        }

        public virtual void Delete(int id)
        {
            TEntity entityToDelete = _dbSet.Find(id);
            Delete(entityToDelete);
        }
        public virtual void Delete(TEntity entityToDelete)
        {
            if (entityToDelete == null) throw new Exception();
            if (entityToDelete.Id == 0) throw new Exception();

            BeforeDelete(entityToDelete);

            try
            {
                if (_context.Entry(entityToDelete).State == EntityState.Detached)
                {
                    _dbSet.Attach(entityToDelete);
                }
                _dbSet.Remove(entityToDelete);
            }
            catch (Exception e)
            {
                throw new Exception("Could not delete data", e);
            }
        }
        public virtual void DeleteMany(IList<int> entitiesToDelete)
        {
            foreach (var id in entitiesToDelete)
            {
                Delete(id);
            }
        }
        public virtual void DeleteMany(IList<TEntity> entitiesToDelete)
        {
            var idsToDelete = new List<int>();
            foreach (var entityToDelete in entitiesToDelete) idsToDelete.Add(item: entityToDelete.Id);
            DeleteMany(idsToDelete);
        }

        public virtual void Update(TEntity entityToUpdate)
        {
            if (entityToUpdate.Id == 0) throw new Exception();

            BeforeUpdate(entityToUpdate);

            try
            {
                _dbSet.Attach(entityToUpdate);
                _context.Entry(entityToUpdate).State = EntityState.Modified;
            }
            catch (Exception e)
            {
                throw new Exception("Could not update data", e);
            }
        }

        //Disposing stuff
        private bool _disposed = false;

        protected virtual void Dispose(bool disposing)
        {
            if (!_disposed)
            {
                if (disposing)
                {
                    _context.Dispose();
                }
            }
            _disposed = true;
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
    }

    public static class ContextExtensions
    {
        public static IEnumerable<string> GetCollections<TEntity>(this DbEntityEntry<TEntity> o) where TEntity : class
        {
            var result = new List<string>();
            var e = o.Entity;
            var properties = e.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (typeof(IEnumerable<dynamic>).IsAssignableFrom(prop.PropertyType))
                {
                    if(!Attribute.IsDefined(prop,typeof(NotMappedAttribute)))
                    result.Add(prop.Name);
                }
            }
            return result;
        }

        public static IEnumerable<string> GetCollections<TEntity>(this DbEntityEntry<TEntity> o, Type childType) where TEntity : class
        {
            var result = new List<string>();
            var e = o.Entity;
            var properties = e.GetType().GetProperties(BindingFlags.Public | BindingFlags.NonPublic | BindingFlags.Instance);
            foreach (var prop in properties)
            {
                if (typeof(IEnumerable<dynamic>).IsAssignableFrom(prop.PropertyType))
                {
                    if (!Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                        if (prop.PropertyType.GetGenericArguments().First() == childType) result.Add(prop.Name);
                }
            }
            return result;
        }

        public static IEnumerable<string> GetReferences<TEntity>(this DbEntityEntry<TEntity> o) where TEntity : class
        {
            var result = new List<string>();
            var e = o.Entity;
            foreach (var prop in e.GetType().GetProperties())
            {
                if (typeof(IEntity).IsAssignableFrom(prop.PropertyType))
                {
                    if (!Attribute.IsDefined(prop, typeof(NotMappedAttribute)))
                        result.Add(prop.Name);
                }
            }
            return result;
        }
    }
    }

