 using FluentValidation;
 using ECommerceAPI.Models;
 
 namespace ECommerceAPI.Validators
 {
     public class FavoriteValidator : AbstractValidator<FavoriteDTO>
     {
         public FavoriteValidator()
         {
             RuleFor(x => x.FavoriteID)
                 .GreaterThanOrEqualTo(0);
 
             RuleFor(x => x.UserID)
                 .GreaterThan(0);
 
             RuleFor(x => x.ProductID)
                 .GreaterThan(0);
         }
     }
 }
