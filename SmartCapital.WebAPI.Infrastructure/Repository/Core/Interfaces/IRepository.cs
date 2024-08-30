using System.Linq.Expressions;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Core.Interfaces
{
    /// <summary>
    /// Define os contratos para um repositório genérico que opera com entidades do tipo <typeparamref name="TEntity"/>.
    /// </summary>
    /// <typeparam name="TEntity">O tipo da entidade que o repositório gerencia. Deve ser uma classe.</typeparam>
    public interface IRepository<TEntity> where TEntity : class
    {
        /// <summary>
        /// Obtém uma coleção de entidades que correspondem ao filtro fornecido.
        /// </summary>
        /// <param name="filter">Uma expressão que define o critério de filtragem das entidades. Pode ser <c>null</c>.</param>
        /// <param name="orderBy">Uma função que define a ordenação das entidades. Pode ser <c>null</c>.</param>
        /// <param name="includeProperties">Uma string que especifica as propriedades de navegação, separadas por virgula, para incluir na consulta. Se não houver propriedades para incluir, deve ser uma string vazia.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona. O resultado é uma coleção de entidades que atendem ao critério de filtro e ordenação especificados.</returns>
        public Task<IEnumerable<TEntity>> GetAsync(
            Expression<Func<TEntity, bool>>? filter = null,
            Func<IQueryable<TEntity>, IOrderedQueryable<TEntity>>? orderBy = null,
            string includeProperties = "");

        /// <summary>
        /// Insere uma nova entidade no repositório.
        /// </summary>
        /// <param name="entity">A entidade a ser inserida.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task InsertAsync(TEntity entity);

        /// <summary>
        /// Insere um conjunto de entidades no repositório.
        /// </summary>
        /// <param name="entities">A coleção de entidades a ser inserida.</param>
        /// <returns>Uma tarefa que representa a operação assíncrona.</returns>
        public Task InsertRangeAsync(IEnumerable<TEntity> entities);

        /// <summary>
        /// Atualiza uma entidade existente no repositório.
        /// </summary>
        /// <param name="entity">A entidade a ser atualizada.</param>
        public void Update(TEntity entity);

        /// <summary>
        /// Atualiza um conjunto de entidades existentes no repositório.
        /// </summary>
        /// <param name="entities">A coleção de entidades a ser atualizada.</param>
        public void UpdateRange(IEnumerable<TEntity> entities);

        /// <summary>
        /// Remove uma entidade existente do repositório.
        /// </summary>
        /// <param name="entity">A entidade a ser removida.</param>
        public void Delete(TEntity entity);

        /// <summary>
        /// Remove um conjunto de entidades existentes do repositório.
        /// </summary>
        /// <param name="entities">A coleção de entidades a ser removida.</param>
        public void DeleteRange(IEnumerable<TEntity> entities);
    }
}
