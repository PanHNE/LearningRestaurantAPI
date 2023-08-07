using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Services;

namespace RestaurantAPI.Authorization
{
    public class MinimumCreatedRestaurantsHandler : AuthorizationHandler<MinimumCreatedRestaurantRequirment>
    {
        private readonly ILogger<MinimumAgeRequirementHandler> _logger;
        private readonly RestaurantDBContext _dBContext;

        public MinimumCreatedRestaurantsHandler(ILogger<MinimumAgeRequirementHandler> logger, RestaurantDBContext dBContext)
        {
            _logger = logger;
            _dBContext = dBContext;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumCreatedRestaurantRequirment requirement)
        {
            var userId = int.Parse(context.User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier).Value);

            var numberOfCreatedRestaurant = _dBContext.Restaurants.Count(r => r.CreatedById == userId);

            if (numberOfCreatedRestaurant < requirement.MinimumCreatedRestaurant)
            {
                _logger.LogInformation("Authorization failed");
            } else
            {
                _logger.LogInformation("Athorization succed");
                context.Succeed(requirement);
            }

            return Task.CompletedTask;
        }
    }
}
