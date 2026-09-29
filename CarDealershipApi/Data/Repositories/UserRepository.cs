using CarDealershipApi.Domain;
using Dapper;

namespace CarDealershipApi.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
    }
    public class UserRepository(DbConnectionFactory db) : IUserRepository
    {
        public async Task<User> GetByEmailAsync (string email)
        {
            using var connection = db.Create;
            await connection.OpenAsync();
            var user = await connection.QuerySingleOrDefaultAsync<User>(
                "SELECT * FROM Users WHERE Email = @Email",
                new { Email = email.Trim() }
            );
            return user;
        }
    }
}
