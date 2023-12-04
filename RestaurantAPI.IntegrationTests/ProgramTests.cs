using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Testing;
using RestaurantAPI.Controllers;

namespace RestaurantAPI.IntegrationTests
{
    public class ProgramTests : IClassFixture<WebApplicationFactory<Program>>
    {
        private List<Type> _controllerTypes;
        private readonly WebApplicationFactory<Program> _factory;

        public ProgramTests(WebApplicationFactory<Program> factory)
        {
            _controllerTypes = typeof(Program)
                .Assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(typeof(ControllerBase)))
                .ToList();

            _factory = factory.WithWebHostBuilder(builder =>
            {
                builder.ConfigureServices(services =>
                {
                    _controllerTypes.ForEach(c => services.AddScoped(c));
                });
            });
        }

        [Fact]
        public void ConfigureServices_ForControllers_RegisterAllDependencies()
        {
            var scopeFactory = _factory.Services.GetService<IServiceScopeFactory>();
            using var scope = scopeFactory.CreateScope();

            // assert 
            _controllerTypes.ForEach(ct =>
            {
                var controller = scope.ServiceProvider.GetService(ct);
                
                controller.Should().NotBeNull();
            });

        }
    }
}
