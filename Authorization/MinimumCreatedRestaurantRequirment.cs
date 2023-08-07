using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumCreatedRestaurantRequirment : IAuthorizationRequirement
    {
        public int MinimumCreatedRestaurant { get; }

        public MinimumCreatedRestaurantRequirment(int minimumCreatedRestaurant)
        {
            MinimumCreatedRestaurant = minimumCreatedRestaurant;
        }
    }
}
