using Microsoft.AspNetCore.Authorization;

namespace RestaurantAPI.Authorization
{
    public class MinimumAgeRequirement : IAuthorizationRequirement
    {
        public int MinAge { get; }

        public MinimumAgeRequirement(int minAge)
        {
            MinAge = minAge;
        }
    }
}
