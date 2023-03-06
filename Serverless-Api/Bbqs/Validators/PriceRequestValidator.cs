using FluentValidation;
using Serverless_Api.Bbqs.Functions.ShoppingList;

namespace Serverless_Api.Bbqs.Validators
{
    public class PriceRequestValidator : AbstractValidator<PriceRequest>
    {
        public PriceRequestValidator()
        {
            RuleFor(x => x.VegetablesPricePerKg)
                .GreaterThan(0)
                .WithMessage("VegetablesPricePerKg must be greater than 0");

            RuleFor(x => x.MeatPricePerKg)
                .GreaterThan(0)
                .WithMessage("MeatPricePerKg must be greater than 0");
        }
    }
}
