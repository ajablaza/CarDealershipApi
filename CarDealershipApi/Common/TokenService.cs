using CarDealershipApi.Domain;
using FastEndpoints.Security;
namespace CarDealershipApi.Common
{
    public class TokenService (IConfiguration configuration)
    {
        public (string token, DateTime ExpiresAt) CreateToken (User user)
        {
            var expiresAt = DateTime.UtcNow.AddMinutes(configuration.GetValue("Jwt:ExpiryMinutes", 60));

            var token = JwtBearer.CreateToken(o =>
            {
                o.SigningKey = configuration["Jwt:SigningKey"]!;
                o.ExpireAt = expiresAt;
                o.User.Roles.Add(user.Role);
                o.User.Claims.Add((AppClaims.UserId, user.Id.ToString()));

                if (user.DealershipId is not null)
                {
                    o.User.Claims.Add((AppClaims.DealershipId, user.DealershipId.Value.ToString()));
                }
            });
            
            return (token, expiresAt);
        }
    }
}
