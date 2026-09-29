namespace CarDealershipApi.Domain
{
    public class User
    {
        public long Id { get; set; }
        public long? DealershipId { get; set; }
        public string Username { get; set; } = null!;
        public string Email { get; set; } = null!;
        public string PasswordHash { get; set; } = null!;
        public string Role { get; set; } = Roles.Staff;
        public DateTime CreatedAt { get; set; }
    }
}
