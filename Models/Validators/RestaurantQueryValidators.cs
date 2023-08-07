using System.Linq;
using FluentValidation;
using RestaurantAPI.Models.Queries;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidators : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSizes = new []{ 5, 10, 15, 20 }; 
        public RestaurantQueryValidators()
        {
            RuleFor(r => r.PageNumber).GreaterThanOrEqualTo(1);
            RuleFor(r => r.PageSize).Custom((value, context) =>
            {
                if (!allowedPageSizes.Contains(value))
                {
                    context.AddFailure("PageSize", $"PageSize must in [{string.Join(",", allowedPageSizes)}]");
                }
            });
        }
    }
}
