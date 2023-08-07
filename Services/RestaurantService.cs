using AutoMapper;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authorization;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using RestaurantAPI.Authorization;
using RestaurantAPI.Entities;
using RestaurantAPI.Exceptions;
using RestaurantAPI.Models.Restaurant;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;

namespace RestaurantAPI.Services
{
    public interface IRestaurantService
    {
        int Create(CreateRestaurantDto createRestaurantDto, int userId);
        void Delete(int id, ClaimsPrincipal user);
        IEnumerable<RestaurantDto> GetAll();
        RestaurantDto GetById(int id);
        void Update(int id, UpdateRestaurantDto updateRestaurantDto, ClaimsPrincipal user);
    }

    public class RestaurantService : IRestaurantService
    {
        private readonly IAuthorizationService _authorizationService;
        private readonly ILogger<RestaurantService> _logger;
        private readonly IMapper _mapper;
        private readonly RestaurantDBContext _dbContext;

        public RestaurantService(IAuthorizationService authorizationService, ILogger<RestaurantService> logger, IMapper mapper, RestaurantDBContext dBContext)
        {
            _authorizationService = authorizationService;
            _dbContext = dBContext;
            _logger = logger;
            _mapper = mapper;
        }


        public int Create(CreateRestaurantDto createDto, int userId)
        {
            var restaurant = _mapper.Map<Restaurant>(createDto);
            restaurant.CreatedById = userId;

            _dbContext.Add(restaurant);
            _dbContext.SaveChanges();

            return restaurant.Id;
        }

        public void Delete(int id, ClaimsPrincipal user)
        {
            _logger.LogWarning($"Restaurant with id:{id} DELETE action invoke!");

            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant == null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Delete)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidExcepetion();
            }

            _dbContext.Remove(restaurant);
            _dbContext.SaveChanges();
        }

        public IEnumerable<RestaurantDto> GetAll()
        {
            var restaurants = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .ToList();

            var restaurantsDto = _mapper.Map<List<RestaurantDto>>(restaurants);

            return restaurantsDto;
        }

        public RestaurantDto GetById(int id)
        {

            var restaurant = _dbContext
                .Restaurants
                .Include(r => r.Address)
                .Include(r => r.Dishes)
                .FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var restaurantDTO = _mapper.Map<RestaurantDto>(restaurant);

            return restaurantDTO;
        }

        public void Update(int id, UpdateRestaurantDto updateRestaurantDto, ClaimsPrincipal user)
        {
            var restaurant = _dbContext.Restaurants.FirstOrDefault(r => r.Id == id);

            if (restaurant is null)
                throw new NotFoundException("Restaurant not found");

            var authorizationResult = _authorizationService.AuthorizeAsync(user, restaurant, new ResourceOperationRequirement(ResourceOperation.Update)).Result;

            if (!authorizationResult.Succeeded)
            {
                throw new ForbidExcepetion();
            }

            restaurant.Name = updateRestaurantDto.Name;
            restaurant.Description = updateRestaurantDto.Description;
            restaurant.HasDelivery = updateRestaurantDto.HasDelivery;

            _dbContext.Update(restaurant);
            _dbContext.SaveChanges();
        }
    }
}
