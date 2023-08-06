using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models.User;

namespace RestaurantAPI.Controllers
{
    [Route("api/user")]
    [ApiController]
    public class UserController : ControllerBase
    {
        public UserController()
        {
            
        }
        public ActionResult Registration([FromBody] CreateUserDto createUserDto)
        {
            return Created("New user created", null);
        }
    }
}
