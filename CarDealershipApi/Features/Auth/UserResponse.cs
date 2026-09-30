using CarDealershipApi.Domain;

namespace CarDealershipApi.Features.Auth
{
    public class UserResponse
    {
        public long Id { get; set; }
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Role { get; set; } = "";
        public long? DealershipId { get; set; }
        public DateTime CreatedAt { get; set; }

        public static UserResponse FromUser(User user) => new()
        {
            Id = user.Id,
            Username = user.Username,
            Email = user.Email,
            Role = user.Role,
            DealershipId = user.DealershipId,
            CreatedAt = user.CreatedAt
        };
    }
}