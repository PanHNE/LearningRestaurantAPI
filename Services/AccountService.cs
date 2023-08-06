using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models.User;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto createUserDto);
    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDBContext _dbContext;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;

        public AccountService(RestaurantDBContext restaurantDBContext, IMapper mapper, ILogger<AccountService> logger, IPasswordHasher<User> passwordHasher)
        {
            _dbContext = restaurantDBContext;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
        }

        public void RegisterUser(RegisterUserDto createUserDto)
        {
            var foundUser = _dbContext.Users.FirstOrDefault( u => u.Email == createUserDto.Email );

            if (foundUser != null) 
                throw new AlreadyExists($"User with {foundUser.Email} exist in database");

            var newUser = new User()
            {
                Email = createUserDto.Email,
                DateOfBirth = createUserDto.DateOfBirth,
                Nationality = createUserDto.Nationality,
                RoleId = createUserDto.RoleId
            };

            var hashedPassword = _passwordHasher.HashPassword(newUser, createUserDto.Password);

            newUser.PasswordHash = hashedPassword;
            _dbContext.Add(newUser);
            _dbContext.SaveChanges();
        }
    }
}
