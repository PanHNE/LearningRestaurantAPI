using FluentValidation.TestHelper;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models.User;
using RestaurantAPI.Models.Validators;

namespace RestaurantAPI.IntegrationTests.Models.Validators
{
    public class RegisterUserDtoValidatorTests
    {
        private readonly RestaurantDbContext _dbContext;

        public RegisterUserDtoValidatorTests()
        {
            var builder = new DbContextOptionsBuilder<RestaurantDbContext>();
            builder.UseInMemoryDatabase("TestDb");

            _dbContext = new RestaurantDbContext(builder.Options);
            Seed();
        }

        public void Seed()
        {
            var testUsers = new List<User>()
            {
                new User()
                {
                    Email = "test2@test.com"
                },
                new User()
                {
                    Email = "test3@test.com"
                },
                new User()
                {
                    Email = "test4@test.com"
                },
            };

            _dbContext.Users.AddRange(testUsers);
            _dbContext.SaveChanges();
        }

        [Fact]
        public void Validate_ForValidModel_ReturnSuccess()
        {
            // arrange
            var model = new RegisterUserDto()
            {
                Email = "test@test.pl",
                Password = "password",
                ConfirmPassword = "password",
            };

            var validator = new RegisterUserDtoValidator(_dbContext);

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldNotHaveAnyValidationErrors();

        }

        [Fact]
        public void Validate_ForInvalidModel_ReturnFailure()
        {
            // arrange
            var model = new RegisterUserDto()
            {
                Email = "test2@test.com",
                Password = "password",
                ConfirmPassword = "password",
            };

            var validator = new RegisterUserDtoValidator(_dbContext);

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
