using Microsoft.AspNetCore.Mvc;
using RestaurantAPI.Models.User;
using RestaurantAPI.Services;

namespace RestaurantAPI.Controllers
{
    [Route("api/account")]
    [ApiController]
    public class AccountController : ControllerBase
    {
        private readonly IAccountService _accountService;

        public AccountController(IAccountService accountService)
        {
            _accountService = accountService;
        }

        [HttpPost("register")]
        public ActionResult Register([FromBody] RegisterUserDto createUserDto)
        {
            _accountService.RegisterUser(createUserDto);

            return Created("New user created", null);
        }

        [HttpPost("login")]
        public ActionResult Login([FromBody] LoginUserDto loginUserDto)
        {
            string token = _accountService.GenerateJwt(loginUserDto);

            return Ok(token);
        }
    }
}
