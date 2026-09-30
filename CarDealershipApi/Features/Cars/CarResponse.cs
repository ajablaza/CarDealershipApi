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
        public int Mileage { get; set; }
        public CarStatus Status { get; set; }
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
            Mileage = car.Mileage,
            Status = car.Status,
            CreatedAt = car.CreatedAt,
            UpdatedAt = car.UpdatedAt
        };
    }
}
