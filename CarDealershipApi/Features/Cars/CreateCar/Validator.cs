using FastEndpoints;
using FluentValidation;

namespace CarDealershipApi.Features.Cars.CreateCar
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Make).NotEmpty();
            RuleFor(x => x.Model).NotEmpty();
            RuleFor(x => x.Year).InclusiveBetween(1886, DateTime.Now.Year);
            RuleFor(x => x.Price).GreaterThan(0);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0);
        }
    }
}
