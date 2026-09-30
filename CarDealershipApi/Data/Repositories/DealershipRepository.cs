using CarDealershipApi.Domain;
using Dapper;

namespace CarDealershipApi.Data.Repositories
{
    public interface IDealershipRepository
    {
        Task<Dealership?> CreateWithAdminAsync(string name, long userId);
    }

    public class DealershipRepository(DbConnectionFactory db) : IDealershipRepository
    {
        public async Task<Dealership?> CreateWithAdminAsync(string name, long userId)
        {
            using var conn = db.Create;
            await conn.OpenAsync();                     
            using var tx = conn.BeginTransaction();

            var dealership = await conn.QuerySingleAsync<Dealership>("""
                INSERT INTO Dealerships (Name, CreatedAt)
                VALUES (@Name, @CreatedAt)
                RETURNING Id, Name, CreatedAt;
                """,
                new { Name = name, CreatedAt = DateTime.UtcNow.ToString("O") },
                tx);

            var rows = await conn.ExecuteAsync("""
                UPDATE Users
                SET DealershipId = @DealershipId, Role = @Role
                WHERE Id = @UserId AND DealershipId IS NULL;
                """,
                new { DealershipId = dealership.Id, Role = Roles.Admin, UserId = userId },
                tx);

            if (rows == 0)
            {
                tx.Rollback();                         
                return null;
            }

            tx.Commit();
            return dealership;
        }
    }
}