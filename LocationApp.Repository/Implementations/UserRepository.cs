using LocationApp.Model.Core;
using LocationApp.Model.Extensions;
using LocationApp.Repository.Base;
using LocationApp.Repository.Contracts;
using Microsoft.EntityFrameworkCore;

namespace LocationApp.Repository.Implementations
{
    public class UserRepository : RepositoryBase<User>, IUserRepository
    {
        private readonly RepositoryContext _repositoryContext;

        public UserRepository(RepositoryContext repositoryContext) : base(repositoryContext)
        {
            _repositoryContext = repositoryContext;
        }

        public async Task<bool> CheckUserExists(string userName) => await _repositoryContext.Users.Where(u => u.Username.Equals(userName)).AnyAsync();

        public async Task<User?> GetUser(string apiKey) => await _repositoryContext.Users.Where(u => u.ApiKey.Equals(apiKey)).FirstOrDefaultAsync();

        public async Task<User?> GetUser(string username, string password) 
            => await _repositoryContext.Users.Where(u => u.Username.Equals(username) && u.Password.Equals(password.ToSHA512Hash())).FirstOrDefaultAsync();

        public async Task<User?> GetUserWithLocations(string apiKey)
            => await _repositoryContext.Users.Include(u => u.Locations).Where(u => u.ApiKey.Equals(apiKey)).FirstOrDefaultAsync(); 

        public async Task CreateUser(User user)
        {
            Create(user);
            await _repositoryContext.SaveChangesAsync();
        }

        public async Task UpdateUser(User user)
        {
            Update(user);
            await _repositoryContext.SaveChangesAsync();
        }
    }
}
