using FastEndpoints;
using FluentValidation;

namespace CarDealershipApi.Features.Dealerships.AddStaff
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Email).NotEmpty().EmailAddress();
        }
    }
}