using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using FastEndpoints;
using static FastEndpoints.Ep;

namespace CarDealershipApi.Features.Cars.DeleteCar
{
    public class Endpoint(ICarRepository cars) : Endpoint<Request, CarResponse>
    {
        public override void Configure()
        {
            Delete("/cars/{Id}");
            Claims(AppClaims.DealershipId);
            Roles(Domain.Roles.Admin);
        }
        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var deleted = await cars.DeleteAsync(request.Id, User.GetDealershipId()!.Value);
            if (deleted)
            {
                await Send.NoContentAsync(ct);
            }
            else
            {
                await Send.NotFoundAsync(ct);
            }
        }
    }
}
