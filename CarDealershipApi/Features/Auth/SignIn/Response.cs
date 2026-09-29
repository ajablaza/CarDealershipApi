namespace CarDealershipApi.Features.Auth.SignIn
{
    public class Response
    {
        public string Token { get; set; } = "";
        public DateTime ExpiresAt { get; set; }
    }
}
