using LocationApp.Repository.Contracts;

namespace LocationApp.Repository
{
    public interface IRepositoryManager
    {
        public IUserRepository User { get; }
        public ILocationRepository Location { get; }
        public ICategoryRepository Category { get; }
        public ILogRepository Log { get; }

        Task SaveAsync();
    }
}
