using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class CategoryValidator : AbstractValidator<CategoryDTO>
    {
        public CategoryValidator()
        {
            RuleFor(x => x.CategoryID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("CategoryID must be >= 0.");

            RuleFor(x => x.CategoryName)
                .NotEmpty().WithMessage("CategoryName is required.")
                .MaximumLength(100).WithMessage("CategoryName must not exceed 100 characters.");

            RuleFor(x => x.UserID)
                .GreaterThan(0).WithMessage("UserID must be a valid id.")
                .When(x => x.UserID.HasValue);
        }
    }
}

