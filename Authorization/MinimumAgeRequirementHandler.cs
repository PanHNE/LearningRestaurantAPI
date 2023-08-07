using System;
using System.Security.Claims;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Logging;

namespace RestaurantAPI.Authorization
{
    public class MinimumAgeRequirementHandler : AuthorizationHandler<MinimumAgeRequirement>
    {
        private readonly ILogger<MinimumAgeRequirementHandler> _logger;

        public MinimumAgeRequirementHandler(ILogger<MinimumAgeRequirementHandler> logger)
        {
            this._logger = logger;
        }

        protected override Task HandleRequirementAsync(AuthorizationHandlerContext context, MinimumAgeRequirement requirement)
        {
            var dateOfBirth = DateTime.Parse(context.User.FindFirst(c => c.Type == "DateOfBirth").Value);
            var userEmail = context.User.FindFirst(u => u.Type == ClaimTypes.Name).Value;

            _logger.LogInformation($"User: {userEmail} with date of birth: [{dateOfBirth}]");

            if (dateOfBirth.AddYears(requirement.MinAge) < DateTime.Today)
            {
                _logger.LogInformation("Athorization succed");
                context.Succeed(requirement);
            } else
            {
                _logger.LogInformation("Athorization failed");
            }

            return Task.CompletedTask;
        }
    }
}
