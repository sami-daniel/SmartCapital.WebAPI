using Microsoft.EntityFrameworkCore.Storage;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;
using SmartCapital.WebAPI.Infrastructure.UnitOfWork.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.UnitOfWork.Implementations
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ApplicationDbContext _context;
        private bool disposedValue;

        public IProfileRepository ProfileRepository { get; }

        public UnitOfWork(IProfileRepository profileRepository, ApplicationDbContext applicationDbContext)
        {
            ProfileRepository = profileRepository;
            _context = applicationDbContext;
        }

        public async Task<int> CompleteAsync()
        {
            return await _context.SaveChangesAsync();
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    _context.Dispose();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }

        public async Task<IDbContextTransaction> StartTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }
    }
}
