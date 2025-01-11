using LocationApp.Model.Core;
using LocationApp.Repository.Base;
using LocationApp.Repository.Contracts;

namespace LocationApp.Repository.Implementations
{
    internal class LogRepository : RepositoryBase<RequestResponseLog>, ILogRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public LogRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task CreateLog(RequestResponseLog log)
        {
            Create(log);
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
