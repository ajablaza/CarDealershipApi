namespace CarDealershipApi.Domain
{
    public class Car
    {
        public int Id { get; set; }
        public int DealershipId { get; set; }
        public string Make { get; set; } = "";
        public string Model { get; set; } = "";
        public int Year { get; set; }
        public string Color { get; set; } = "";
        public decimal Price { get; set; }
        public int Mileage { get; set; }
        public CarStatus Status { get; set; } = CarStatus.Available;
        public DateTime CreatedAt { get; set; }
        public DateTime UpdatedAt { get; set; }
        public DateTime? DeletedAt { get; set; }
    }
}