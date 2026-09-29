using CarDealershipApi.Common;
using CarDealershipApi.Data.Repositories;
using FastEndpoints;

namespace CarDealershipApi.Features.Auth.SignIn;

public class Endpoint(IUserRepository users, TokenService tokens) : Endpoint<Request, Response>
{
    public override void Configure()
    {
        Post("/auth/signin");
        AllowAnonymous();
    }

    public override async Task HandleAsync(Request req, CancellationToken ct)
    {
        var user = await users.GetByEmailAsync(req.Email);
        if (user is null || !BCrypt.Net.BCrypt.Verify(req.Password, user.PasswordHash))
        {
            ThrowError("Invalid email or password.", 401);
        }

        var (token, expiresAt) = tokens.CreateToken(user);

        await Send.OkAsync(new Response { Token = token, ExpiresAt = expiresAt }, ct);
    }
}