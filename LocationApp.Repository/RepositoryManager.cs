using LocationApp.Model.Core;
using LocationApp.Repository.Contracts;
using LocationApp.Repository.Implementations;

namespace LocationApp.Repository
{
    public class RepositoryManager : IRepositoryManager
    {
        private readonly RepositoryContext _repositoryContext;

        private readonly Lazy<IUserRepository> _userRepositroy;
        private readonly Lazy<ILocationRepository> _locationRepositroy;
        private readonly Lazy<ICategoryRepository> _categoryRepositroy;
        private readonly Lazy<ILogRepository> _logRepositroy;

        public RepositoryManager(RepositoryContext repositoryContext)
        {
            _repositoryContext = repositoryContext;

            _userRepositroy = new Lazy<IUserRepository>(() => new UserRepository(repositoryContext));
            _locationRepositroy = new Lazy<ILocationRepository>(() => new LocationRepository(repositoryContext));
            _categoryRepositroy = new Lazy<ICategoryRepository>(() => new CategoryRepository(repositoryContext));
            _logRepositroy = new Lazy<ILogRepository>(() => new LogRepository(repositoryContext));
        }

        public IUserRepository User => _userRepositroy.Value;
        public ILocationRepository Location => _locationRepositroy.Value;
        public ICategoryRepository Category => _categoryRepositroy.Value;
        public ILogRepository Log => _logRepositroy.Value;

        public async Task SaveAsync() => await _repositoryContext.SaveChangesAsync();
    }
}
