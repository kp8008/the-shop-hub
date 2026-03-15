using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class ProductValidator : AbstractValidator<ProductDTO>
    {
        public ProductValidator()
        {
            RuleFor(x => x.ProductID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("ProductID must be >= 0.");

            RuleFor(x => x.ProductName)
                .NotEmpty().WithMessage("ProductName is required.")
                .MaximumLength(200).WithMessage("ProductName must not exceed 200 characters.");

            RuleFor(x => x.ProductCode)
                .NotEmpty().WithMessage("ProductCode is required.")
                .MaximumLength(50).WithMessage("ProductCode must not exceed 50 characters.");

            RuleFor(x => x.CategoryID)
                .GreaterThan(0).WithMessage("CategoryID must be a valid id.");

            RuleFor(x => x.UserID)
                .GreaterThan(0).WithMessage("UserID must be a valid id.")
                .When(x => x.UserID.HasValue);

            RuleFor(x => x.Price)
                .GreaterThan(0).WithMessage("Price must be greater than 0.");

            RuleFor(x => x.StockQuantity)
                .GreaterThanOrEqualTo(0).WithMessage("StockQuantity must be >= 0.");

            RuleFor(x => x.Image)
                .MaximumLength(300).WithMessage("Image must not exceed 300 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Image));
        }
    }
}

