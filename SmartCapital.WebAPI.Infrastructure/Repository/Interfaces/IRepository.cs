using System.Linq.Expressions;

namespace SmartCapital.WebAPI.Domain.Repository.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task<IEnumerable<TEntity>> GetAsync (Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
        public Task<TEntity?> GetByIDAsync (object ID);

        public Task InsertAsync(TEntity entity);

        public Task InsertRangeAsync(IEnumerable<TEntity> entities);

        public Task UpdateAsync(TEntity entity);

        public Task UpdateRangeAsync(IEnumerable<TEntity> entities);

        public Task DeleteAsync(TEntity entity);

        public Task DeleteRangeAsync(IEnumerable<TEntity> entities);
    }
}
