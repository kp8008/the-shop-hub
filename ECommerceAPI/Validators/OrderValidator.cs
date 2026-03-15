using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class OrderValidator : AbstractValidator<OrderDTO>
    {
        public OrderValidator()
        {
            RuleFor(x => x.OrderID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("OrderID must be >= 0.");

            RuleFor(x => x.UserID)
                .GreaterThan(0).WithMessage("UserID must be a valid id.");

            RuleFor(x => x.OrderNo)
                .NotEmpty().WithMessage("OrderNo is required.")
                .MaximumLength(50).WithMessage("OrderNo must not exceed 50 characters.");

            RuleFor(x => x.AddressID)
                .GreaterThan(0).WithMessage("AddressID must be a valid id.")
                .When(x => x.AddressID.HasValue);

            RuleFor(x => x.TotalAmount)
                .GreaterThan(0).WithMessage("TotalAmount must be greater than 0.");

            RuleFor(x => x.CouponDiscount)
                .GreaterThanOrEqualTo(0).WithMessage("CouponDiscount must be >= 0.")
                .When(x => x.CouponDiscount.HasValue);

            RuleFor(x => x.NetAmount)
                .GreaterThanOrEqualTo(0).WithMessage("NetAmount must be >= 0.")
                .When(x => x.NetAmount.HasValue);

            RuleFor(x => x.Status)
                .NotEmpty().WithMessage("Status is required.")
                .MaximumLength(20).WithMessage("Status must not exceed 20 characters.")
                .When(x => x.OrderID > 0);
        }
    }
}

