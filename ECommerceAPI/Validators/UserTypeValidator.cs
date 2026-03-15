using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class UserTypeValidator : AbstractValidator<UserTypeDTO>
    {
        public UserTypeValidator()
        {
            RuleFor(x => x.UserTypeID)
                .GreaterThan(0).WithMessage("UserTypeID must be a valid id.");

            RuleFor(x => x.UserTypeName)
                .NotEmpty().WithMessage("UserTypeName is required.")
                .MaximumLength(50).WithMessage("UserTypeName must not exceed 50 characters.");
        }
    }
}

