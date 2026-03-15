using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class CartItemValidator : AbstractValidator<CartItemDTO>
    {
        public CartItemValidator()
        {
            RuleFor(x => x.CartItemID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CartItemID must be >= 0.");

            RuleFor(x => x.CartID)
                .GreaterThan(0).WithMessage("CartID must be a valid id.");

            RuleFor(x => x.ProductID)
                .GreaterThan(0).WithMessage("ProductID must be a valid id.");

            RuleFor(x => x.Quantity)
                .GreaterThan(0).WithMessage("Quantity must be greater than 0.");

            RuleFor(x => x.Price)
                .GreaterThanOrEqualTo(0).WithMessage("Price must be >= 0.");
        }
    }
}

