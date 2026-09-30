using CarDealershipApi.Domain;
namespace CarDealershipApi.Features.Cars.ListCars
{
    public class Request
    {
        public string? Make { get; set; }
        public string? Model { get; set; }
        public CarStatus? Status { get; set; }
    }
}
