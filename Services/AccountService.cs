using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using AutoMapper;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models.User;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using System.Text;
using System;
using System.IdentityModel.Tokens.Jwt;

namespace RestaurantAPI.Services
{
    public interface IAccountService
    {
        void RegisterUser(RegisterUserDto createUserDto);
        string GenerateJwt(LoginUserDto loginUserDto);
    }

    public class AccountService : IAccountService
    {
        private readonly RestaurantDBContext _dbContext;
        private readonly ILogger<AccountService> _logger;
        private readonly IMapper _mapper;
        private readonly IPasswordHasher<User> _passwordHasher;
        private readonly AuthenticationsSettings _authenticationSettings;

        public AccountService(
            RestaurantDBContext restaurantDBContext,
            IMapper mapper,
            ILogger<AccountService> logger,
            IPasswordHasher<User> passwordHasher,
            AuthenticationsSettings authenticationsSettings
            ){
            _dbContext = restaurantDBContext;
            _mapper = mapper;
            _logger = logger;
            _passwordHasher = passwordHasher;
            _authenticationSettings = authenticationsSettings;
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

        public string GenerateJwt(LoginUserDto loginUserDto)
        {
            var user = _dbContext.Users
                .Include(u => u.Role)
                .FirstOrDefault( u => u.Email == loginUserDto.Email );

            if (user is null) throw new BadRequestException("Invalid username or password");

            var result = _passwordHasher.VerifyHashedPassword(user, user.PasswordHash, loginUserDto.Password);

            if (result == PasswordVerificationResult.Failed) throw new BadRequestException("Invalid username or password");

            var claims = new List<Claim>(){
                new Claim(ClaimTypes.NameIdentifier, user.Id.ToString()),
                new Claim(ClaimTypes.Name, $"{user.FirstName} {user.LastName}"),
                new Claim(ClaimTypes.Role, $"{user.Role.Name}"),
                new Claim("DateOfBirth", user.DateOfBirth.Value.ToString("yyyy-MM-dd"))
            };

            if (!string.IsNullOrEmpty(user.Nationality))
            {
                claims.Add(
                    new Claim("Nationality", user.Nationality)
                    );
            }

            var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_authenticationSettings.JwtKey));
            var cred = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);
            var expires = DateTime.Now.AddDays(_authenticationSettings.JwtExpireDays);

            var token = new JwtSecurityToken(
                _authenticationSettings.JwtIssuer,
                _authenticationSettings.JwtIssuer,
                claims,
                expires: expires,
                signingCredentials: cred
                );

            var tokenHandler = new JwtSecurityTokenHandler();

            return tokenHandler.WriteToken(token);
        }
    }
}
