using FluentValidation;
using ECommerceAPI.Models;
using System.Text.RegularExpressions;

namespace ECommerceAPI.Validators
{
    public class UserValidator : AbstractValidator<UserDTO>
    {
        public UserValidator()
        {
            RuleFor(x => x.UserID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("UserID must be >= 0.");

            RuleFor(x => x.UserName)
                .NotEmpty().WithMessage("UserName is required.")
                .MinimumLength(3).WithMessage("UserName must be at least 3 characters.")
                .MaximumLength(100).WithMessage("UserName must not exceed 100 characters.");

            RuleFor(x => x.Address)
                .MaximumLength(300).WithMessage("Address must not exceed 300 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Address));

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^\d{10,15}$").WithMessage("Phone must be 10 to 15 digits.");

            RuleFor(x => x.Email)
                .NotEmpty().WithMessage("Email is required.")
                .EmailAddress().WithMessage("Invalid email format.")
                .MaximumLength(150).WithMessage("Email must not exceed 150 characters.");

            RuleFor(x => x.UserTypeID)
                .GreaterThan(0).WithMessage("UserTypeID must be a valid id.");

        }
    }
}
