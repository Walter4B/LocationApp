using LocationApp.Model.Core;

namespace LocationApp.Repository.Contracts
{
    public interface IUserRepository
    {
        /// <summary>
        /// Check if user exists
        /// </summary>
        /// <param name="userName"></param>
        /// <returns></returns>
        Task<bool> CheckUserExists(string userName);

        /// <summary>
        /// Get user by api key
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        Task<User?> GetUser(string apiKey);

        /// <summary>
        /// Get user by login data
        /// </summary>
        /// <param name="username"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        Task<User?> GetUser(string username, string password);

        /// <summary>
        /// Get user with favorite locations
        /// </summary>
        /// <param name="apiKey"></param>
        /// <returns></returns>
        Task<User?> GetUserWithLocations(string apiKey);

        /// <summary>
        /// Update user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task UpdateUser(User user);

        /// <summary>
        /// Create user
        /// </summary>
        /// <param name="user"></param>
        /// <returns></returns>
        Task CreateUser(User user);
    }
}
