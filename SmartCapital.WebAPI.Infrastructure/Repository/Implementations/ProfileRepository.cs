// none

using SmartCapital.WebAPI.Domain.Domain;
using SmartCapital.WebAPI.Infrastructure.Data.Contexts;
using SmartCapital.WebAPI.Infrastructure.Repository.Core.Implementations;
using SmartCapital.WebAPI.Infrastructure.Repository.Interfaces;

namespace SmartCapital.WebAPI.Infrastructure.Repository.Implementations
{
    /// <summary>
    /// Implements a specific repository for the entity <see cref="Profile"/>.
    /// </summary>
    public class ProfileRepository : Repository<Profile>, IProfileRepository
    {
        /// <summary>
        /// Initializes a new repository instance for the class <see cref="ProfileRepository"/> with the provided database context.
        /// </summary>
        /// <param name="applicationDbContext">The database context used to access the data source.</param>
        public ProfileRepository(ApplicationDbContext applicationDbContext) : base(applicationDbContext)
        {
        }
    }
}
