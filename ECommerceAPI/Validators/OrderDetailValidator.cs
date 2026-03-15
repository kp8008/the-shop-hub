using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class OrderDetailValidator : AbstractValidator<OrderDetailDTO>
    {
        public OrderDetailValidator()
        {
            RuleFor(x => x.OrderDetailID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("OrderDetailID must be >= 0.");

            RuleFor(x => x.OrderID)
                .GreaterThan(0).WithMessage("OrderID must be a valid id.");

            RuleFor(x => x.ProductID)
                .GreaterThan(0).WithMessage("ProductID must be a valid id.");

            RuleFor(x => x.AddressID)
                .GreaterThan(0).WithMessage("AddressID must be a valid id.")
                .When(x => x.AddressID.HasValue);

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Amount)
                .GreaterThan(0).WithMessage("Amount must be greater than 0.");

            RuleFor(x => x.Discount)
                .GreaterThanOrEqualTo(0).WithMessage("Discount must be >= 0.");

            RuleFor(x => x.NetAmount)
                .GreaterThan(0).WithMessage("NetAmount must be greater than 0.");
        }
    }
}

