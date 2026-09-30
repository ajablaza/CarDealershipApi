using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using FastEndpoints;

namespace CarDealershipApi.Features.Dealerships.CreateDealership
{
    public class Endpoint(
        IUserRepository users,
        IDealershipRepository dealerships,
        TokenService tokens) : Endpoint<Request, Response>
    {
        public override void Configure()
        {
            Post("/dealerships");
        }

        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var userId = User.GetUserId();

            var dealership = await dealerships.CreateWithAdminAsync(request.Name.Trim(), userId);
            if (dealership is null)
            {
                ThrowError("You already belong to a dealership.", 409);
            }

            var user = await users.GetByIdAsync(userId);
            var (token, expiresAt) = tokens.CreateToken(user!);

            await Send.ResponseAsync(new Response
            {
                Id = dealership.Id,
                Name = dealership.Name,
                CreatedAt = dealership.CreatedAt,
                Token = token,
                ExpiresAt = expiresAt
            }, 201, ct);
        }
    }
}