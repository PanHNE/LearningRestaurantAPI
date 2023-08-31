using Castle.Components.DictionaryAdapter.Xml;
using Microsoft.AspNetCore.Mvc.Testing;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;
using Xunit.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using RestaurantAPI.Models.Restaurant;
using Newtonsoft.Json;
using System.Text;
using Microsoft.AspNetCore.Authorization.Policy;
using RestaurantAPI.IntegrationTests.Helpers;
using RestaurantAPI.Models;
using System.Globalization;
using System.Xml.Linq;

namespace RestaurantAPI.IntegrationTests.Controllers
{
    public class RestaurantControllerTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly HttpClient _client;
        private readonly WebApplicationFactory<Program> _factory;

        public RestaurantControllerTests(WebApplicationFactory<Program> factory)
        {
            _factory = factory
                .WithWebHostBuilder(builder =>
                {
                    builder.ConfigureServices(services =>
                    {
                        var dbContextOptions = services.SingleOrDefault(service => service.ServiceType == typeof(DbContextOptions<RestaurantDbContext>));

                        services.Remove(dbContextOptions);

                        services.AddSingleton<IPolicyEvaluator, FakePolicyEvaluator>();

                        services.AddMvc(option => option.Filters.Add(new FakeUserFilter()));

                        services.AddDbContext<RestaurantDbContext>(options => options.UseInMemoryDatabase("RestaurantDb"));
                    });
                });

            _client = _factory.CreateClient();
        }

        [Theory]
        [InlineData("pageSize=5&pageNumber=1&sortBy=Name&SortDirection=ASC")]
        [InlineData("pageSize=15&pageNumber=1&sortBy=Name&SortDirection=DESC")]
        [InlineData("pageSize=20&pageNumber=1&sortBy=Name&SortDirection=ASC")]
        public async Task GetAll_WithQueryParameters_ReturnsOkResult(string queryParameters)
        {
            // act
            var response = await _client.GetAsync("/api/restaurant?" + queryParameters);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.OK);
        }

        [Theory]
        [InlineData("pageSize=45&pageNumber=2&sortBy=Name&SortDirection=DESC")]
        [InlineData("pageSize=45&pageNumber=2&sortBy=SName&SortDirection=DESC")]
        [InlineData("pageSize=45&pageNumber=2&sortBy=Name&SortDirection=AAAA")]
        [InlineData("")]
        [InlineData(null)]
        public async Task GetAll_WithInvalidQueryParameters_ReturnsBadRequestResult(string queryParameters)
        {
            // act
            var response = await _client.GetAsync("/api/restaurant?" + queryParameters);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task CreateRestaurant_WithValidModel_ReturnCreatedStatus()
        {
            // arrange
            var model = new CreateRestaurantDto()
            {
                Name = "TestRestaurant",
                City = "Białystok",
                Street = "Zagumienna"
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Created);
            response.Headers.Location.Should().NotBeNull();
        }

        [Fact]
        public async Task CreateRestaurant_WithInvalidModel_ReturnBadRequest()
        {
            // arrange
            var model = new CreateRestaurantDto()
            {
                ContactEmail = "test@test.pl",
                Description = "Description",
                ContactNumber = "888 999 765"
            };

            var httpContent = model.ToJsonHttpContent();

            // act
            var response = await _client.PostAsync("/api/restaurant", httpContent);

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.BadRequest);
        }

        [Fact]
        public async Task Delete_ForRestaurantOwner_ReturnNoContent()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 1,
                Name = "Test",
                Id = 20
            };
            SeedRestaurant(restaurant);

            // act
            var response = await _client.DeleteAsync($"/api/restaurant/{restaurant.Id}");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NoContent);
        }

        [Fact]
        public async Task Delete_ForNotExistingRestauran_ReturnNotFound()
        {
            // act
            var response = await _client.DeleteAsync("/api/restaurant/987");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.NotFound);
        }

        [Fact]
        public async Task Delete_ForNonRestaurantOwner_ReturnForbiden()
        {
            // arrange
            var restaurant = new Restaurant()
            {
                CreatedById = 900,
                Name = "Test",
                Id = 20
            };
            SeedRestaurant(restaurant);

            // act
            var response = await _client.DeleteAsync($"/api/restaurant/{restaurant.Id}");

            // assert
            response.StatusCode.Should().Be(System.Net.HttpStatusCode.Forbidden);
        }

        private void SeedRestaurant(Restaurant restaurant)
        {
            // seed
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();
            var _dbContext = scope.ServiceProvider.GetService<RestaurantDbContext>();

            _dbContext.Restaurants.Add(restaurant);
            _dbContext.SaveChanges();
        }
    }
}
