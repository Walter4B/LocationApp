using LocationApp.Model.Core;

namespace LocationApp.Repository.Contracts
{
    public interface ILogRepository
    {
        /// <summary>
        /// Create log
        /// </summary>
        /// <param name="log"></param>
        /// <returns></returns>
        Task CreateLog(RequestResponseLog log);
    }
}
