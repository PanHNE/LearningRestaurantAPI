using System.Linq;
using FluentValidation;
using RestaurantAPI.Models.Queries;

namespace RestaurantAPI.Models.Validators
{
    public class RestaurantQueryValidators : AbstractValidator<RestaurantQuery>
    {
        private int[] allowedPageSizes = new []{ 5, 10, 15, 20 };
        private string[] allowedSortByColumnNames = new[] { nameof(Entities.Restaurant.Name), nameof(Entities.Restaurant.Description), nameof(Entities.Restaurant.Category)};
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
            RuleFor(r => r.SortBy)
                .Must(value => string.IsNullOrEmpty(value) || allowedSortByColumnNames.Contains(value))
                .WithMessage($"Sort by is optional, or must be in [{string.Join(",", allowedSortByColumnNames)}]");
        }
    }
}
