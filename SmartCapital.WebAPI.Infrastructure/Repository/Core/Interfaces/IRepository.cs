// none


using System.Linq.Expressions;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces;

/// <summary>
/// Defines the contracts for a generic repository that operates with entities of type <typeparamref name="TEntity"/>.
/// </summary>
/// <typeparam name="TEntity">The type of entity that the repository manages. Must be a class.</typeparam>
public interface IRepository<TEntity> where TEntity : class
{
    /// <summary>
    /// Gets a collection of entities that match the provided filter.
    /// </summary>
    /// <param name="filter">An expression that defines the filtering criteria for the entities. Can be <c>null</c>.</param>
    /// <param name="orderBy">A function that defines the ordering of the entities. Can be <c>null</c>.</param>
    /// <param name="includeProperties">A string that specifies the navigation properties, separated by commas, to include in the query. If there are no properties to include, it should be an empty string.</param>
    /// <returns>A task that represents the asynchronous operation. The result is a collection of entities that meet the specified filter and ordering criteria.</returns>
    public Task<IEnumerable<TEntity>> GetAsync(
        Expression<Func<TEntity, bool>>? filter = null,
        Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
        string includeProperties = "");

    /// <summary>
    /// Inserts a new entity into the repository.
    /// </summary>
    /// <param name="entity">The entity to be inserted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task InsertAsync(TEntity entity);

    /// <summary>
    /// Inserts a set of entities into the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to be inserted.</param>
    /// <returns>A task that represents the asynchronous operation.</returns>
    public Task InsertRangeAsync(IEnumerable<TEntity> entities);

    /// <summary>
    /// Updates an existing entity in the repository.
    /// </summary>
    /// <param name="entity">The entity to be updated.</param>
    public void Update(TEntity entity);

    /// <summary>
    /// Updates a set of existing entities in the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to be updated.</param>
    public void UpdateRange(IEnumerable<TEntity> entities);

    /// <summary>
    /// Removes an existing entity from the repository.
    /// </summary>
    /// <param name="entity">The entity to be removed.</param>
    public void Delete(TEntity entity);

    /// <summary>
    /// Removes a set of existing entities from the repository.
    /// </summary>
    /// <param name="entities">The collection of entities to be removed.</param>
    public void DeleteRange(IEnumerable<TEntity> entities);
}
