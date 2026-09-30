using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using FastEndpoints;

namespace CarDealershipApi.Features.Auth.Me
{
    public class Endpoint(IUserRepository users) : EndpointWithoutRequest<UserResponse>
    {
        public override void Configure()
        {
            Get("/auth/me");
        }

        public override async Task HandleAsync(CancellationToken ct)
        {
            var user = await users.GetByIdAsync(User.GetUserId());

            if (user is null)
            {
                await Send.NotFoundAsync(ct);
                return;
            }

            await Send.OkAsync(UserResponse.FromUser(user), ct);
        }
    }
}