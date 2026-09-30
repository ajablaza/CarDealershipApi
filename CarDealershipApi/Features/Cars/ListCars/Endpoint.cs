using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using FastEndpoints;

namespace CarDealershipApi.Features.Cars.ListCars
{
    public class Endpoint(ICarRepository cars) : Endpoint<Request, IEnumerable<CarResponse>>
    {
        public override void Configure()
        {
            Get("/cars");
            Claims(AppClaims.DealershipId);
        }

        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var dealershipId = User.GetDealershipId()!.Value;
            var results = await cars.ListAsync(dealershipId, request.Make, request.Model, request.Status?.ToString());
            await Send.OkAsync(results.Select(CarResponse.FromCar), ct);
        }
    }
}
