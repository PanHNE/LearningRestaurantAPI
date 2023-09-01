using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using RestaurantAPI.Controllers;
using RestaurantAPI.Entities;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models.Dish;
using RestaurantAPI.Services;

namespace RestaurantAPI.IntegrationTests.Controllers
{
    public class DishControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;
        private Mock<IDishService> _dishService = new Mock<IDishService>();

        public DishControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IDishService>(_dishService.Object);

                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDbTest"));
                    });
                });

            _client = _factory.CreateClient();
        }

        public static IEnumerable<object[]> GetDishData()
        {
            var dishes = new List<CreateDishDto>()
            {
                new CreateDishDto()
                {
                    Name = "Test Name Dish 1"
                },
                new CreateDishDto()
                {
                    Name = "Test Name Dish 2",
                    Description = "Test Description Dish 2"
                },
                new CreateDishDto()
                {
                    Name = "Test Name Dish 3",
                    Description = "Test Description Dish 3",
                    Price = 3
                },
                new CreateDishDto()
                {
                    Name = "Test Name Dish 4",
                    Description = "Test Description Dish 4",
                    Price = 4,
                    RestaurantId = 4
                }
            };

            return dishes.Select( d => new object[] { d });
        }

        public static IEnumerable<object[]> GetWrongDishData()
        {
            var dishes = new List<CreateDishDto>()
            {
                new CreateDishDto()
                {
                },
                new CreateDishDto()
                {
                    Description = "Test Description Dish 2"
                },
                new CreateDishDto()
                {
                    Description = "Test Description Dish 3",
                    Price = 3
                },
                new CreateDishDto()
                {
                    Description = "Test Description Dish 4",
                    Price = 4,
                    RestaurantId = 4
                }
            };

            return dishes.Select( d => new object[] { d });
        }

        [Theory]
        [MemberData(nameof(GetDishData))]
        public async void Create_WithValidModel_ReturnsCreated(CreateDishDto createDishDto)
        {
            // arrange
            var httpContext = createDishDto.ToJsonHttpContent();

            // act
            var result = await _client.PostAsync("api/restaurant/8/dish", httpContext);

            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
        }

        [Theory]
        [MemberData(nameof(GetWrongDishData))]
        public async void Create_WithInvalidModel_ReturnsBadRequest(CreateDishDto createDishDto)
        {
            // arrange
            var httpContext = createDishDto.ToJsonHttpContent();

            // act
            var result = await _client.PostAsync("api/restaurant/8/dish", httpContext);

            // assert
            result.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }
    }
}
