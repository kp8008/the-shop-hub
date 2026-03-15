using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class ProductReviewValidator : AbstractValidator<ProductReviewDTO>
    {
        public ProductReviewValidator()
        {
            RuleFor(x => x.ProductID)
                .NotEmpty().WithMessage("ProductID is required.")
                .GreaterThan(0).WithMessage("ProductID must be greater than 0.");

            RuleFor(x => x.Rating)
                .NotEmpty().WithMessage("Rating is required.")
                .InclusiveBetween(1, 5).WithMessage("Rating must be between 1 and 5.");

            RuleFor(x => x.Title)
                .NotEmpty().WithMessage("Title is required.")
                .MaximumLength(100).WithMessage("Title must not exceed 100 characters.");

            RuleFor(x => x.Comment)
                .NotEmpty().WithMessage("Comment is required.")
                .MaximumLength(500).WithMessage("Comment must not exceed 500 characters.");

            RuleFor(x => x.Image)
                .MaximumLength(500).WithMessage("Image URL must not exceed 500 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Image));
        }
    }
}
