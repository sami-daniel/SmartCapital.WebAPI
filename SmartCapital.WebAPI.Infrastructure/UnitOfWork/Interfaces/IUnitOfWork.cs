using Microsoft.EntityFrameworkCore.Storage;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces
{
    /// <summary>
    /// Define contratos para uma unidade de trabalho que gerencia repositórios e operações de banco de dados.
    /// </summary>
    public interface IUnitOfWork : IDisposable
    {
        /// <summary>
        /// Obtém o repositório para a entidade <see cref="Profile"/>.
        /// </summary>
        /// <value>
        /// Uma instância do repositório para a entidade <see cref="Profile"/>.
        /// </value>
        public IProfileRepository ProfileRepository { get; }

        /// <summary>
        /// Salva todas as alterações feitas no contexto atual de banco de dados.
        /// </summary>
        /// <returns>
        /// Uma tarefa que representa a operação assíncrona. O resultado é o número de alterações feitas no banco de dados.
        /// </returns>
        public Task<int> CompleteAsync();

        /// <summary>
        /// Inicia uma nova transação no contexto atual de banco de dados.
        /// </summary>
        /// <returns>
        /// Uma tarefa que representa a operação assíncrona. O resultado é a transação do contexto de banco de dados.
        /// </returns>
        public Task<IDbContextTransaction> StartTransactionAsync();
    }
}
