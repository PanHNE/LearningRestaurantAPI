using System.Linq;
using FluentValidation;
using RestaurantAPI.Entities;
using RestaurantAPI.Models.User;

namespace RestaurantAPI.Models.Validators
{
    public class RegisterUserDtoValidator : AbstractValidator<RegisterUserDto>
    {
        public RegisterUserDtoValidator(RestaurantDBContext restaurantDbContext)
        {
            RuleFor( x => x.Email)
                .NotEmpty()
                .EmailAddress();

            RuleFor(x => x.Password)
                .MinimumLength(6);

            RuleFor(x => x.ConfirmPassword)
                .Equal(e => e.Password);

            RuleFor(x => x.Email)
                .Custom((value, context) =>
                {
                    var emailInUse = restaurantDbContext.Users.Any( u => u.Email == value );

                    if (emailInUse)
                    {
                        context.AddFailure("Email", "That email is taken");
                    }
                });
        }
    }
}
