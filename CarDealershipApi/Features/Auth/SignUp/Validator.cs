using FastEndpoints;
using FluentValidation;

namespace CarDealershipApi.Features.Auth.SignUp
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Username)
                .NotEmpty()
                .Length(3, 30)
                .Matches("^[a-zA-Z0-9._-]+$")
                .WithMessage("Username can only contain letters, numbers, dots, underscores and hyphens.");

            RuleFor(x => x.Email).NotEmpty().EmailAddress().MaximumLength(100);

            RuleFor(x => x.Password).NotEmpty().MinimumLength(8).MaximumLength(100);
        }
    }
}