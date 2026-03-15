using FluentValidation;
using ECommerceAPI.Models;

namespace ECommerceAPI.Validators
{
    public class AddressValidator : AbstractValidator<AddressDTO>
    {
        public AddressValidator()
        {
            RuleFor(x => x.AddressID)
                .GreaterThanOrEqualTo(0)
                .WithMessage("AddressID must be >= 0.");

            RuleFor(x => x.UserID)
                .GreaterThan(0).WithMessage("UserID must be a valid id.");

            RuleFor(x => x.ReceiverName)
                .NotEmpty().WithMessage("ReceiverName is required.")
                .MaximumLength(100).WithMessage("ReceiverName must not exceed 100 characters.");

            RuleFor(x => x.Phone)
                .NotEmpty().WithMessage("Phone is required.")
                .Matches(@"^\d{10,15}$").WithMessage("Phone must be 10 to 15 digits.");

            RuleFor(x => x.AddressLine1)
                .NotEmpty().WithMessage("AddressLine1 is required.")
                .MaximumLength(200).WithMessage("AddressLine1 must not exceed 200 characters.");

            RuleFor(x => x.Landmark)
                .MaximumLength(150).WithMessage("Landmark must not exceed 150 characters.")
                .When(x => !string.IsNullOrWhiteSpace(x.Landmark));

            RuleFor(x => x.City)
                .NotEmpty().WithMessage("City is required.")
                .MaximumLength(100).WithMessage("City must not exceed 100 characters.");

            RuleFor(x => x.State)
                .NotEmpty().WithMessage("State is required.")
                .MaximumLength(100).WithMessage("State must not exceed 100 characters.");

            RuleFor(x => x.Country)
                .NotEmpty().WithMessage("Country is required.")
                .MaximumLength(100).WithMessage("Country must not exceed 100 characters.");

            RuleFor(x => x.Pincode)
                .NotEmpty().WithMessage("Pincode is required.")
                .MaximumLength(10).WithMessage("Pincode must not exceed 10 characters.");
        }
    }
}

