using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using FastEndpoints;

namespace CarDealershipApi.Features.Cars.GetCar
{
    public class Endpoint (ICarRepository cars) : Endpoint<Request, CarResponse>
    {
        public override void Configure()
        {
            Get("/cars/{Id}");
            Claims(AppClaims.DealershipId);
        }

        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var dealershipId = User.GetDealershipId()!.Value;
            var car = await cars.GetByIdAsync(id: request.Id, dealershipId: dealershipId);
            if (car is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }
            await Send.OkAsync(CarResponse.FromCar(car), ct);
        }
    }
}
