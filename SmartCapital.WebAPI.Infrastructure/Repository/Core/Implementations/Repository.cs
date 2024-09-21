// none

using System.Linq.Expressions;
using EFCore.BulkExtensions;
using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Core.Implementations;

/// <summary>
/// Implements a generic repository for the entity of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity managed by the repository. Must be a class.</typeparam>
public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _entitySet;
    private readonly DbContext _context;
    /// <summary>
    /// Initializes a new instance of the <see cref="Repository{TEntity}"/> class with the provided database context.
    /// </summary>
    /// <param name="context">The database context used to access the data source.</param>
    protected Repository(DbContext context)
    {
        _entitySet = context.Set<TEntity>();
        _context = context;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(
    Expression<Func<TEntity, bool>>? filter = null,
    Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
    string includeProperties = "")
    {
        IQueryable<TEntity> query = _entitySet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        if (!string.IsNullOrWhiteSpace(includeProperties))
        {
            foreach (var includeProperty in includeProperties.Split(new[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
            {
                query = query.Include(includeProperty);
            }
        }

        return orderBy != null ? await orderBy(query).ToListAsync() : await query.ToListAsync();
    }

    public virtual async Task InsertAsync(TEntity entity)
    {
        await _entitySet.AddAsync(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _entitySet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        _entitySet.Remove(entity);
    }

    public virtual async Task InsertRangeAsync(IEnumerable<TEntity> entities)
    {
        await _context.BulkInsertAsync(entities);
    }

    public virtual async void UpdateRange(IEnumerable<TEntity> entities)
    {
        await _context.BulkUpdateAsync(entities);
    }

    public virtual async void DeleteRange(IEnumerable<TEntity> entities)
    {
        await _context.BulkDeleteAsync(entities);
    }
}
