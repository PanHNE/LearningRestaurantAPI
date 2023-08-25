using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Authorization.Policy;

namespace RestaurantAPI.IntegrationTests
{
    public class FakePolicyEvaluator : IPolicyEvaluator
    {
        public Task<AuthenticateResult> AuthenticateAsync(AuthorizationPolicy policy, HttpContext context)
        {
            var claimsPrincipal = new ClaimsPrincipal();
            var tikcet = new AuthenticationTicket(claimsPrincipal, "Test");
            var result = AuthenticateResult.Success(tikcet);

            return Task.FromResult(result);
        }

        public Task<PolicyAuthorizationResult> AuthorizeAsync(AuthorizationPolicy policy, AuthenticateResult authenticationResult, HttpContext context, object? resource)
        {
            var result = PolicyAuthorizationResult.Success();

            return Task.FromResult(result);
        }
    }
}
