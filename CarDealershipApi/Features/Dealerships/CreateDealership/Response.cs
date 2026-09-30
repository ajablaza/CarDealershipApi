namespace CarDealershipApi.Features.Dealerships.CreateDealership
{
    public class Response
    {
        public long Id { get; set; }
        public string Name { get; set; } = "";
        public DateTime CreatedAt { get; set; }
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
}