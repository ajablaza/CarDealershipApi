using System.Security.Claims;

namespace CarDealershipApi.Common
{
    public static class AppClaims
    {
        public const string UserId = "UserId";
        public const string DealershipId = "DealershipId";
    }
    public static class ClaimsPrincipalExtensions
    {
        public static long GetUserId(this ClaimsPrincipal user)
        {
            var userIdClaim = user.FindFirst(AppClaims.UserId);
            if (userIdClaim == null || !long.TryParse(userIdClaim.Value, out var userId))
            {
                throw new InvalidOperationException("User ID claim is missing or invalid.");
            }
            return long.Parse(userIdClaim.Value);
        }

        public static long? GetDealershipId(this ClaimsPrincipal user)
        {
            var dealershipIdClaim = user.FindFirst(AppClaims.DealershipId);
            if (dealershipIdClaim == null || !long.TryParse(dealershipIdClaim.Value, out var dealershipId))
            {
                return null;
            }
            return long.Parse(dealershipIdClaim.Value);
        }
    }
}
