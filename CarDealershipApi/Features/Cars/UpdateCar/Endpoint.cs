using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using FastEndpoints;

namespace CarDealershipApi.Features.Cars.UpdateCar
{
    public class Endpoint(ICarRepository cars) : Endpoint<Request, CarResponse>
    {
        public override void Configure()
        {
            Patch("/cars/{Id}");
            Claims(AppClaims.DealershipId);
        }

        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var updated = await cars.UpdateAsync(
                id: request.Id,
                dealershipId: User.GetDealershipId()!.Value,
                make: request.Make?.Trim(),
                model: request.Model?.Trim(),
                year: request.Year,
                color: request.Color?.Trim(),
                price: request.Price,
                stock: request.Stock);

            // Null when no row matched: wrong id, another dealership's car, or deleted.
            if (updated is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            await Send.OkAsync(CarResponse.FromCar(updated), ct);
        }
    }
}