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

        public static IEnumerable<object[]> GetSampleValidData()
        {
            var list = new List<RegisterUserDto>
            {
                new RegisterUserDto()
                {
                    Email = "test@test.pl",
                    Password = "password",
                    ConfirmPassword = "password",
                },
                new RegisterUserDto()
                {
                    Email = "test5@test.pl",
                    Password = "password",
                    ConfirmPassword = "password",
                },
                new RegisterUserDto()
                {
                    Email = "tes7@test.pl",
                    Password = "password",
                    ConfirmPassword = "password",
                },
            };

            return list.Select(q => new object[] { q });
        }

        public static IEnumerable<object[]> GetSampleInvalidData()
        {
            var list = new List<RegisterUserDto>
            {
                new RegisterUserDto()
                {
                    Email = "test2@test.com",
                    Password = "password",
                    ConfirmPassword = "password",
                },
                new RegisterUserDto()
                {
                    Email = "test@test.pl",
                    Password = "pass2word",
                    ConfirmPassword = "password",
                },
                new RegisterUserDto()
                {
                    Email = "tes7@test.pl",
                    Password = "p",
                    ConfirmPassword = "p",
                },
            };

            return list.Select(q => new object[] { q });
        }

        [Theory]
        [MemberData(nameof(GetSampleValidData))]
        public void Validate_ForValidModel_ReturnSuccess(RegisterUserDto model)
        {
            // arrange
            var validator = new RegisterUserDtoValidator(_dbContext);

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldNotHaveAnyValidationErrors();

        }

        [Theory]
        [MemberData(nameof(GetSampleInvalidData))]
        public void Validate_ForInvalidModel_ReturnFailure(RegisterUserDto model)
        {
            // arrange
            var validator = new RegisterUserDtoValidator(_dbContext);

            // act
            var result = validator.TestValidate(model);

            // assert
            result.ShouldHaveAnyValidationError();
        }
    }
}
