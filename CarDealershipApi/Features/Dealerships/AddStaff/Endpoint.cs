using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using CarDealershipApi.Features.Auth;
using FastEndpoints;

namespace CarDealershipApi.Features.Dealerships.AddStaff
{
    public class Endpoint(IUserRepository users) : Endpoint<Request, UserResponse>
    {
        public override void Configure()
        {
            Post("/dealerships/staff");
            Claims(AppClaims.DealershipId);
            Roles(Domain.Roles.Admin);
        }

        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var dealershipId = User.GetDealershipId()!.Value;

            var target = await users.GetByEmailAsync(request.Email);
            if (target is null)
            {
                ThrowError("No user with that email.", 404);
            }

            if (target.DealershipId is not null)
            {
                ThrowError("That user already belongs to a dealership.", 409);
            }

            var assigned = await users.AssignToDealershipAsync(target.Id, dealershipId, Domain.Roles.Staff);
            if (!assigned)
            {
                ThrowError("That user already belongs to a dealership.", 409);
            }

            var updated = await users.GetByIdAsync(target.Id);
            await Send.OkAsync(UserResponse.FromUser(updated!), ct);
        }
    }
}