using System.Linq.Expressions;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces
{
    public interface IRepository<TEntity> where TEntity : class
    {
        public Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");
        public Task<TEntity?> GetByIDAsync(object ID);

        public Task InsertAsync(TEntity entity);

        public Task InsertRangeAsync(IEnumerable<TEntity> entities);

        public void Update(TEntity entity);

        public void UpdateRange(IEnumerable<TEntity> entities);

        public void Delete(TEntity entity);

        public void DeleteRange(IEnumerable<TEntity> entities);
    }
}
