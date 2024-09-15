﻿// none

using System.Linq.Expressions;
using Microsoft.EntityFrameworkCore;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Core.Implementations;

/// <summary>
/// Implementa um repositório genérico para a entidade do tipo <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">O tipo da entidade gerenciada pelo repositório. Deve ser uma classe.</typeparam>
public abstract class Repository<TEntity> : IRepository<TEntity> where TEntity : class
{
    private readonly DbSet<TEntity> _entitySet;
    private readonly DapperContext _dapperContext;

    /// <summary>
    /// Inicializa uma nova instância da classe <see cref="Repository{TEntity}"/> com o contexto de banco de dados fornecido.
    /// </summary>
    /// <param name="context">O contexto de banco de dados usado para acessar a fonte de dados.</param>
    /// <param name="dapperContext">O contexto Dapper (<see cref="DapperContext"/>) para acessar a fonte de dados.</param>
    protected Repository(DbContext context, DapperContext dapperContext)
    {
        _entitySet = context.Set<TEntity>();
        _dapperContext = dapperContext;
    }

    public virtual async Task<IEnumerable<TEntity>> GetAsync(Expression<Func<TEntity, bool>>? filter = null, Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null, string includeProperties = "", bool nonTrackableEntities = false)
    {
        IQueryable<TEntity> query = _entitySet;

        if (filter != null)
        {
            query = query.Where(filter);
        }

        foreach (string includeProperty in includeProperties.Split
            (new char[] { ',' }, StringSplitOptions.RemoveEmptyEntries))
        {
            query = query.Include(includeProperty);
        }

        if (orderBy == null)
        {
            if (nonTrackableEntities)
            {
                return await query.AsNoTracking().ToListAsync();
            }
            else
            {
                return await query.ToListAsync();
            }
        }
        else
        {
            if (nonTrackableEntities)
            {
                return await orderBy(query).AsNoTracking().ToListAsync();
            }
            else
            {
                return await orderBy(query).ToListAsync();
            }
        }
    }

    public virtual async Task InsertAsync(TEntity entity)
    {
        _ = await _entitySet.AddAsync(entity);
    }

    public virtual void Update(TEntity entity)
    {
        _ = _entitySet.Update(entity);
    }

    public virtual void Delete(TEntity entity)
    {
        _ = _entitySet.Remove(entity);
    }

    public virtual async Task InsertRangeAsync(IEnumerable<TEntity> entities)
    {
        await _entitySet.AddRangeAsync(entities);
    }

    public virtual void UpdateRange(IEnumerable<TEntity> entities)
    {
        _entitySet.UpdateRange(entities);
    }

    public virtual void DeleteRange(IEnumerable<TEntity> entities)
    {
        _entitySet.RemoveRange(entities);
    }
}
