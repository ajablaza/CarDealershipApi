using CarDealershipApi.Domain;

namespace CarDealershipApi.Features.Cars
{
    public class CarResponse
    {
        public long Id { get; set; }
        public string Make { get; set; } = "";
        public string Model { get; set; } = "";
        public int Year { get; set; }
        public string Color { get; set; } = "";
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }

        public static CarResponse FromCar(Car car) => new()
        {
            Id = car.Id,
            Make = car.Make,
            Model = car.Model,
            Year = car.Year,
            Color = car.Color,
            Price = car.Price,
            Stock = car.Stock,
            CreatedAt = car.CreatedAt,
            UpdatedAt = car.UpdatedAt
        };
    }
}
