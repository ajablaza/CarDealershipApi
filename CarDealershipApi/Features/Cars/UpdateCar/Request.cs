namespace CarDealershipApi.Features.Cars.UpdateCar
{
    public class Request
    {
        public long Id { get; set; }
        public string? Make { get; set; }
        public string? Model { get; set; }
        public int? Year { get; set; }
        public string? Color { get; set; }
        public decimal? Price { get; set; }
        public int? Stock { get; set; }
    }
}
