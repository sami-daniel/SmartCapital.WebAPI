using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        public IProfileRepository ProfileRepository { get; }
        public Task<int> CompleteAsync();
    }
}
