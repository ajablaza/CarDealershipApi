using CarDealershipApi.Domain;
using Dapper;

namespace CarDealershipApi.Data.Repositories
{
    public interface ICarRepository
    {
        Task<IEnumerable<Car>> ListAsync(long dealershipId, string? make, string? model);
        Task<Car?> GetByIdAsync(long id, long dealershipId);
        Task<Car> CreateAsync(Car car);
        Task<Car?> UpdateAsync(long id, long dealershipId,
            string? make, string? model, int? year, string? color, decimal? price, int? stock);
        Task<bool> DeleteAsync(long id, long dealershipId);
    }

    public class CarRepository(DbConnectionFactory db) : ICarRepository
    {
        public async Task<IEnumerable<Car>> ListAsync(long dealershipId, string? make, string? model)
        {
            using var conn = db.Create;

            return await conn.QueryAsync<Car>(
                @"SELECT * FROM Cars
                  WHERE DealershipId = @DealershipId
                  AND (@Make IS NULL OR Make LIKE @Make)
                  AND (@Model IS NULL OR Model LIKE @Model)
                  AND DeletedAt IS NULL
                  ORDER BY Make, Model, Id",
                new 
                { 
                    DealershipId = dealershipId,
                    Make = string.IsNullOrWhiteSpace(make) ? null : $"%{make.Trim()}%",
                    Model = string.IsNullOrWhiteSpace(model) ? null : $"%{model.Trim()}%"
                });
        }
        public async Task<Car?> GetByIdAsync(long id, long dealershipId)
        {
            using var conn = db.Create;
            return await conn.QuerySingleOrDefaultAsync<Car>(
                @"SELECT * FROM Cars
                  WHERE Id = @Id AND DealershipId = @DealershipId AND DeletedAt IS NULL",
                new { Id = id, DealershipId = dealershipId });
        }
        public async Task<Car> CreateAsync(Car car)
        {
            using var conn = db.Create;
            return await conn.QuerySingleAsync<Car>("""
            INSERT INTO Cars (DealershipId, Make, Model, Year, Color, Price, Stock, CreatedAt, UpdatedAt)
            VALUES (@DealershipId, @Make, @Model, @Year, @Color, @Price, @Stock, @CreatedAt, @UpdatedAt)
            RETURNING Id, DealershipId, Make, Model, Year, Color, Price, Stock, CreatedAt, UpdatedAt;
            """, ToParameters(car));
        }
        public async Task<Car?> UpdateAsync(long id, long dealershipId, string? make, string? model, int? year, string? color, decimal? price, int? stock)
        {
            using var conn = db.Create;
            return await conn.QuerySingleOrDefaultAsync<Car>("""
                UPDATE Cars
                SET Make = COALESCE(@Make, Make),
                    Model = COALESCE(@Model, Model),
                    Year = COALESCE(@Year, Year),
                    Color = COALESCE(@Color, Color),
                    Price = COALESCE(@Price, Price),
                    Stock = COALESCE(@Stock, Stock),
                    UpdatedAt = @UpdatedAt
                WHERE Id = @Id AND DealershipId = @DealershipId AND DeletedAt IS NULL
                RETURNING Id, DealershipId, Make, Model, Year, Color, Price, Stock, CreatedAt, UpdatedAt;
                """,
                new
                {
                    Id = id,
                    DealershipId = dealershipId,
                    Make = make,
                    Model = model,
                    Year = year,
                    Color = color,
                    Price = price,
                    Stock = stock,
                    UpdatedAt = DateTime.UtcNow.ToString("O")
                });
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
            car.Stock,
            CreatedAt = car.CreatedAt.ToString("O"),
            UpdatedAt = car.UpdatedAt.ToString("O")
        };
    }
}
