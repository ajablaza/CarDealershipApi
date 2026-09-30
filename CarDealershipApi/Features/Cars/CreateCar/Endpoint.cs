using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using CarDealershipApi.Domain;
using FastEndpoints;

namespace CarDealershipApi.Features.Cars.CreateCar
{
    public class Endpoint(ICarRepository cars) : Endpoint<Request, CarResponse>
    {
        public override void Configure()
        {
            Post("/cars");
            Claims(AppClaims.DealershipId);
        }

        public override async Task HandleAsync (Request req, CancellationToken ct)
        {
            var now = DateTime.UtcNow;

            var car = await cars.CreateAsync(new Car
            {   
                DealershipId = (int)User.GetDealershipId()!.Value,
                Make = req.Make.Trim(),
                Model = req.Model.Trim(),
                Year = req.Year,
                Color = req.Color.Trim(),
                Price = req.Price,
                Stock = req.Stock,
                CreatedAt = now,
                UpdatedAt = now
            });

            await Send.ResponseAsync(CarResponse.FromCar(car), 201, ct);
        }
    }
}
