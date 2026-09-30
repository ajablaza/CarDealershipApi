using CarDealershipApi.Domain;
using Dapper;

namespace CarDealershipApi.Data.Repositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> ListAsync(long dealershipId, string? make, string? model, string? status);
        Task<Car?> GetByIdAsync(long id, long dealershipId);
        Task<Car> CreateAsync(Car car);
        Task<Car?> UpdateAsync(Car car);
        Task<bool> DeleteAsync(long id, long dealershipId);
    }

    public class CarRepository(DbConnectionFactory db) : ICarRepository
    {
        public async Task<IEnumerable<Car>> ListAsync(long dealershipId, string? make, string? model, string? status)
        {
            using var conn = db.Create;

            return await conn.QueryAsync<Car>(
                @"SELECT * FROM Cars
                  WHERE DealershipId = @DealershipId
                  AND (@Make IS NULL OR Make LIKE @Make)
                  AND (@Model IS NULL OR Model LIKE @Model)
                  AND (@Status IS NULL OR Status = @Status)
                  ORDER BY Make, Model, Id",
                new 
                { 
                    DealershipId = dealershipId,
                    Make = string.IsNullOrWhiteSpace(make) ? null : $"%{make.Trim()}%",
                    Model = string.IsNullOrWhiteSpace(model) ? null : $"%{model.Trim()}%", 
                    Status = status 
                });
        }
        public async Task<Car?> GetByIdAsync(long id, long dealershipId)
        {
            using var conn = db.Create;
            return await conn.QuerySingleOrDefaultAsync<Car>(
                @"SELECT * FROM Cars
                  WHERE Id = @Id AND DealershipId = @DealershipId",
                new { Id = id, DealershipId = dealershipId });
        }
        public async Task<Car> CreateAsync(Car car)
        {
            using var conn = db.Create;
            return await conn.QuerySingleAsync<Car>("""
            INSERT INTO Cars (DealershipId, Make, Model, Year, Color, Price, Mileage, Status, CreatedAt, UpdatedAt)
            VALUES (@DealershipId, @Make, @Model, @Year, @Color, @Price, @Mileage, @Status, @CreatedAt, @UpdatedAt)
            RETURNING Id, DealershipId, Make, Model, Year, Color, Price, Mileage, Status, CreatedAt, UpdatedAt;
            """, ToParameters(car));
        }
        public async Task<Car?> UpdateAsync(Car car)
        {
            using var conn = db.Create;
            return await conn.QuerySingleAsync<Car>("""
            UPDATE Cars
            SET Make = @Make, Model = @Model, Year = @Year, Color = @Color,
                Price = @Price, Mileage = @Mileage, Status = @Status, UpdatedAt = @UpdatedAt
            WHERE Id = @Id AND DealershipId = @DealershipId
            RETURNING Id, DealershipId, Make, Model, Year, Color, Price, Mileage, Status, CreatedAt, UpdatedAt;
            """, ToParameters(car));
        }
        public async Task<bool> DeleteAsync(long id, long dealershipId)
        {
            using var conn = db.Create;
            var now = DateTime.UtcNow.ToString("O");
            var rowsAffected = await conn.ExecuteAsync("""
                UPDATE Cars
                Set DeletedAt = @Now, UpdatedAt = @Now
                WHERE Id = @Id AND DealershipId = @DealershipId AND DeletedAt IS NULL
                """,
                new { Id = id, DealershipId = dealershipId, Now = now });
            return rowsAffected > 0;
        }

        private static object ToParameters(Car car) => new
        {
            car.DealershipId,
            car.Make,
            car.Model,
            car.Year,
            car.Color,
            car.Price,
            car.Mileage,
            car.Status,
            car.CreatedAt,
            car.UpdatedAt
        };
    }
}
