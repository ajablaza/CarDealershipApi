using FastEndpoints;

namespace CarDealershipApi.Features.Health;

public class Endpoint : EndpointWithoutRequest
{
    public override void Configure()
    {
        Get("/health");
        AllowAnonymous();
    }

    public override async Task HandleAsync(CancellationToken ct)
    {
        await Send.OkAsync(new { status = "Healthy" }, ct);
    }
}