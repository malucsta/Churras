using Domain.Bbqs.UseCases;
using FluentValidation;

namespace Serverless_Api.Bbqs.Validators
{
    public class NewBbqRequestValidator : AbstractValidator<NewBbqRequest>
    {
        public NewBbqRequestValidator()
        {
            RuleFor(x => x.Reason)
                .NotEmpty()
                .NotNull()
                .WithMessage("Reason is required.");

            RuleFor(x => x.Date)
                .Must(x => x > DateTime.Now)
                .WithMessage("Date cannot be past now");

            RuleFor(x => x.IsTrincasPaying)
                .NotNull()
                .WithMessage("IsTrincasPaying is required");
        }
    }
}
