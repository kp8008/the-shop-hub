using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class PaymentValidator : AbstractValidator<PaymentDTO>
    {
        public PaymentValidator()
        {
            RuleFor(x => x.PaymentID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("PaymentID must be >= 0.");

            RuleFor(x => x.OrderID)
                .GreaterThan(0).WithMessage("OrderID must be a valid id.");

            RuleFor(x => x.PaymentModeID)
                .GreaterThan(0).WithMessage("PaymentModeID must be a valid id.");

            RuleFor(x => x.TotalPayment)
                .GreaterThan(0).WithMessage("TotalPayment must be greater than 0.");

            RuleFor(x => x.PaymentReference)
                .MaximumLength(100).WithMessage("PaymentReference must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.PaymentReference));

            RuleFor(x => x.PaymentStatus)
                .NotEmpty().WithMessage("PaymentStatus is required.")
                .MaximumLength(50).WithMessage("PaymentStatus must not exceed 50 characters.");

            RuleFor(x => x.TransactionID)
                .MaximumLength(100).WithMessage("TransactionID must not exceed 100 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.TransactionID));
        }
    }
}

