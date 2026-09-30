using FastEndpoints;
using FluentValidation;

namespace CarDealershipApi.Features.Cars.UpdateCar
{
    public class Validator : Validator<Request>
    {
        public Validator()
        {
            RuleFor(x => x.Make).NotEmpty().MaximumLength(50).When(x => x.Make is not null);
            RuleFor(x => x.Model).NotEmpty().MaximumLength(50).When(x => x.Model is not null);
            RuleFor(x => x.Year).InclusiveBetween(1886, DateTime.UtcNow.Year + 1).When(x => x.Year is not null);
            RuleFor(x => x.Color).NotEmpty().MaximumLength(30).When(x => x.Color is not null);
            RuleFor(x => x.Price).GreaterThan(0).When(x => x.Price is not null);
            RuleFor(x => x.Stock).GreaterThanOrEqualTo(0).When(x => x.Stock is not null);

            RuleFor(x => x)
                .Must(x => x.Make is not null || x.Model is not null || x.Year is not null
                        || x.Color is not null || x.Price is not null || x.Stock is not null)
                .WithMessage("Provide at least one field to update.");
        }
    }
}