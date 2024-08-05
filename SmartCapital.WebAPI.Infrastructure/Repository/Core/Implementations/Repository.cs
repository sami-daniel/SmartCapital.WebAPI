using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces;
using System.Collections.Generic;
using System.Linq.Expressions;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Core.Implementations
{
    public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
    {
        private readonly DbSet<TEntity> _entitySet;
        protected Repository(DbContext context)
        {
            _entitySet = context.Set<TEntity>();
        }

        public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "")
        {
            IQueryable<TEntity> query = _entitySet;

            if (filter != null)
            {
                query = query.Where(filter);
            }

            foreach (var includeProperty in includeProperties.Split
                (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }

            if (orderBy != null)
            {
                return await orderBy(query).ToListAsync();
            }

            return await query.ToListAsync();
        }

        public virtual async Task<TEntity?> GetByIDAsync(object ID)
        {
            return await _entitySet.FindAsync(ID);
        }

        public virtual async Task InsertAsync(TEntity entity)
        {
            await _entitySet.AddAsync(entity);
        }

        public virtual async Task UpdateAsync(TEntity entity)
        {
            await Task.Run(() =>
            {
                _entitySet.Update(entity);
            });
        }

        public virtual async Task DeleteAsync(TEntity entity)
        {
            await Task.Run(() =>
            {
                _entitySet.Remove(entity);
            });
        }

        public virtual async Task InsertRangeAsync(IEnumerable<TEntity> entities)
        {
            await _entitySet.AddRangeAsync(entities);
        }

        public virtual async Task UpdateRangeAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() =>
            {
                _entitySet.UpdateRange(entities);
            });
        }

        public virtual async Task DeleteRangeAsync(IEnumerable<TEntity> entities)
        {
            await Task.Run(() =>
            {
                _entitySet.RemoveRange(entities);
            });
        }
    }
}
