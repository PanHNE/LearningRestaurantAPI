using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RestaurantAPI.Controllers;

namespace RestaurantAPI.IntegrationTests
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private readonly WebApplicationFactory<Program> _factory;

        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            var controllerTypes = typeof(Program)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                .ToList();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    controllerTypes.ForEach(c => services.AddScoped(c));
                });
            });
        }
    }
}
