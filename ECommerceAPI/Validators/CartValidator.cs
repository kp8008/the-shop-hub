using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class CartValidator : AbstractValidator<CartDTO>
    {
        public CartValidator()
        {
            RuleFor(x => x.CartID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CartID must be >= 0.");

            RuleFor(x => x.UserID)
                .GreaterThan(0).WithMessage("UserID must be a valid id.");
        }
    }
}

