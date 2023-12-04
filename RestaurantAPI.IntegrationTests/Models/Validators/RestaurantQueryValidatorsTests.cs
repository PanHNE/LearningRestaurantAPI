using System.Drawing.Printing;
using FluentValidation.TestHelper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models.Queries;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTests.Models.Validators
{
    public class RestaurantQueryValidatorsTests
    {
        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<RestaurantQuery>()
            {
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    PageSize = 10,
                },
                new RestaurantQuery()
                {
                    PageNumber = 2,
                    PageSize = 10,
                },
                new RestaurantQuery()
                {
                    PageNumber = 1,
                    PageSize = 15,
                    SortBy = nameof(Restaurant.Name)
                },
                new RestaurantQuery()
                {
                    PageNumber = 22,
                    PageSize = 15,
                    SortBy = nameof(Restaurant.Category)
                },
            };

            return list.Select(q => new object[] { q });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<RestaurantQuery>()
            {
                new RestaurantQuery()
                {
                    PageNumber = -1,
                    PageSize = 10,
                },
                new RestaurantQuery()
                {
                    PageNumber = 2,
                    PageSize = 11,
                },
                new RestaurantQuery()
                {
                    PageNumber = -11,
                    PageSize = -15,
                    SortBy = nameof(Restaurant.Name)
                },
                new RestaurantQuery()
                {
                    PageNumber = 22,
                    PageSize = 15,
                    SortBy = nameof(Restaurant.ContactEmail)
                },
            };

            return list.Select(q => new object[] { q });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public async void Validate_ForCorrectModel_ReturnSuccess(RestaurantQuery model)
        {
            // arrange
            var validator = new RestaurantQueryValidators();

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldNotHaveAnyValidationErrors();
        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public async void Validate_ForInCorrectModel_ReturnFailure(RestaurantQuery model)
        {
            // arrange
            var validator = new RestaurantQueryValidators();

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
