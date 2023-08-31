using Microsoft.AspNetCore.Authorization.Policy;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using RestaurantAPI.Models.User;
using RestaurantAPI.IntegrationTests.Helpers;
using Moq;
using RestaurantAPI.Services;

namespace RestaurantAPI.IntegrationTests.Controllers
{
    public class AccountControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private Mock<IAccountService> _mockAccountService = new Mock<IAccountService>();

        public AccountControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IAccountService>(_mockAccountService.Object);

                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    });
                });

            _client = _factory.CreateClient();
        }

        [Fact]
        public async Task Register_WithValidModel_ReturnsCreated()
        {
            // arrange
            var model = new RegisterUserDto()
            {
                Email = "Test@test.pl",
                Password = "test12#123",
                ConfirmPassword = "test12#123",
                Nationality = "Polish",
                DateOfBirth = new DateTime(1993, 5, 14),
                RoleId = 1
            };

            var httpContext = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("api/account/register", httpContext);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Fact]
        public async Task Register_WithInValidModel_ReturnsBadRequest()
        {
            // arrange
            var model = new RegisterUserDto()
            {
                Email = "Test-test.pl",
                Password = "test12#123",
                ConfirmPassword = "test12#123",
                Nationality = "Polish",
                DateOfBirth = new DateTime(1993, 5, 14),
                RoleId = 1
            };

            var httpContext = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("api/account/register", httpContext);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Login_ForRegisteredUser_ReturnsOk()
        {
            // arrange
            _mockAccountService
                .Setup(e => e.GenerateJwt(It.IsAny<LoginUserDto>()))
                .Returns("jwt");

            var loginModel = new LoginUserDto()
            {
                Email = "test@tet.pl",
                Password = "test123"
            };
            var httpContent = loginModel.ToJsonHttpContent();

            // act
            var response = await  _client.PostAsync("api/account/login", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }
    }
}
