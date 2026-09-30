using CarDealershipApi.Domain;
using Dapper;

namespace CarDealershipApi.Data.Repositories
{
    public interface IUserRepository
    {
        Task<User> GetByEmailAsync(string email);
        Task<User?> GetByIdAsync(long id);
        Task<bool> EmailExistsAsync(string email);
        Task<bool> UsernameExistsAsync(string username);
        Task<User> CreateAsync(User user);
        Task<bool> AssignToDealershipAsync(long userId, long dealershipId, string role);
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
        public async Task<User?> GetByIdAsync(long id)
        {
            using var connection = db.Create;
            await connection.OpenAsync();
            var user = await connection.QuerySingleOrDefaultAsync<User>(
                """
                SELECT 
                Id, 
                DealershipId, 
                Username, 
                Email, 
                PasswordHash, 
                Role, 
                CreatedAt 
                FROM Users WHERE Id = @Id
                """,
                new { Id = id }
            );
            return user;
        }
        public async Task<bool> EmailExistsAsync(string email)
        {
            using var connection = db.Create;
            await connection.OpenAsync();
            var count = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Users WHERE Email = @Email",
                new { Email = email.Trim() }
            );
            return count > 0;
        }
        public async Task<bool> UsernameExistsAsync(string username)
        {
            using var connection = db.Create;
            await connection.OpenAsync();
            var count = await connection.ExecuteScalarAsync<int>(
                "SELECT COUNT(1) FROM Users WHERE Username = @Username",
                new { Username = username.Trim() }
            );
            return count > 0;
        }
        public async Task<User> CreateAsync(User user)
        {
            using var connection = db.Create;
            await connection.OpenAsync();
            var createdUser = await connection.QuerySingleAsync<User>(
                """
                INSERT INTO Users (DealershipId, Username, Email, PasswordHash, Role, CreatedAt)
                VALUES (@DealershipId, @Username, @Email, @PasswordHash, @Role, @CreatedAt)
                RETURNING Id, Username, Email, Role, CreatedAt
                """,
                new
                {
                    DealershipId = user.DealershipId,
                    Username = user.Username.Trim(),
                    Email = user.Email.Trim(),
                    PasswordHash = user.PasswordHash,
                    Role = user.Role,
                    CreatedAt = DateTime.UtcNow.ToString("O")
                }
            );
            return createdUser;
        }
        public async Task<bool> AssignToDealershipAsync(long userId, long dealershipId, string role)
        {
            using var connection = db.Create;
            await connection.OpenAsync();
            var rowsAffected = await connection.ExecuteAsync(
                """
                UPDATE Users
                SET DealershipId = @DealershipId, Role = @Role
                WHERE Id = @UserId AND DealershipId IS NULL
                """,
                new { UserId = userId, DealershipId = dealershipId, Role = role }
            );
            return rowsAffected > 0;
        }
    }
}
