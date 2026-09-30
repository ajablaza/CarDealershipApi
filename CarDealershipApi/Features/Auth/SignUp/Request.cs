namespace CarDealershipApi.Features.Auth.SignUp
{
    public class Request
    {
        public string Username { get; set; } = "";
        public string Email { get; set; } = "";
        public string Password { get; set; } = "";
    }
}