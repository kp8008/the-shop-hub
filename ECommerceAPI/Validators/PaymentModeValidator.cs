using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class PaymentModeValidator : AbstractValidator<PaymentModeDTO>
    {
        public PaymentModeValidator()
        {
            RuleFor(x => x.PaymentModeID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("PaymentModeID must be >= 0.");

            RuleFor(x => x.PaymentModeName)
                .NotEmpty().WithMessage("PaymentModeName is required.")
                .MaximumLength(50).WithMessage("PaymentModeName must not exceed 50 characters.");
        }
    }
}

