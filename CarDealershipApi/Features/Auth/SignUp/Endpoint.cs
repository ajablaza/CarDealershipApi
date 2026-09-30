using CarDealershipApi.Data.Repositories;
using FastEndpoints;

namespace CarDealershipApi.Features.Auth.SignUp
{
    public class Endpoint(IUserRepository users) : Endpoint<Request, UserResponse>
    {
        public override void Configure()
        {
            Post("/auth/signup");
            AllowAnonymous();
        }

        public override async Task HandleAsync(Request request, CancellationToken ct)
        {
            var username = request.Username.Trim();
            var email = request.Email.Trim();

            if (await users.EmailExistsAsync(email))
            {
                ThrowError("An account with that email already exists.", 409);
            }

            if (await users.UsernameExistsAsync(username))
            {
                ThrowError("That username is already taken.", 409);
            }

            var user = await users.CreateAsync(new Domain.User
            {
                Username = username,
                Email = email,
                PasswordHash = BCrypt.Net.BCrypt.HashPassword(request.Password),
                Role = Domain.Roles.Staff,
                DealershipId = null,
                CreatedAt = DateTime.UtcNow
            });

            await Send.ResponseAsync(UserResponse.FromUser(user), 201, ct);
        }
    }
}