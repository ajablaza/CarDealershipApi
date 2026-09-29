using FastEndpoints;
using FluentValidation;

namespace CarDealershipApi.Features.Auth.SignIn
{
    public class Validator : Validator<Request>
    {
        public Validator() 
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
            RuleFor(x => x.Password).NotEmpty();
        }
    }
}
