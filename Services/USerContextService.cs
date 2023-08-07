using System.Security.Claims;
using Microsoft.AspNetCore.Http;

namespace RestaurantAPI.Services
{
    public interface IUserContextService
    {
        int? GetUserId { get; }
        ClaimsPrincipal User { get; }
    }

    public class UserContextService : IUserContextService
    {
        private readonly IHttpContextAccessor _contextAccessor;

        public UserContextService(IHttpContextAccessor httpContextAccessor)
        {
            _contextAccessor = httpContextAccessor;
        }

        public ClaimsPrincipal User => _contextAccessor.HttpContext?.User;
        public int? GetUserId => User is null ? null : (int?)int.Parse(User.FindFirst(claim => claim.Type == ClaimTypes.NameIdentifier).Value);
    }
}
