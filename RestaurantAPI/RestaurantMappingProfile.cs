using AutoMapper;
using RestaurantAPI.Entities;
using RestaurantAPI.Models.Dish;
using RestaurantAPI.Models.Restaurant;
using RestaurantAPI.Models.User;

namespace RestaurantAPI
{
    public class RestaurantMappingProfile : Profile
    {
        public RestaurantMappingProfile()
        {
            CreateMap<Restaurant, RestaurantDto>()
                .ForMember( rDTO => rDTO.City, c => c.MapFrom(r => r.Address.City))
                .ForMember( rDTO => rDTO.Street, c => c.MapFrom(r => r.Address.Street))
                .ForMember( rDTO => rDTO.PostalCode, c => c.MapFrom(r => r.Address.PostalCode));

            CreateMap<CreateRestaurantDto, Restaurant>()
                .ForMember(r => r.Address, c => c.MapFrom(crDto => new Address()
                {
                    City = crDto.City, PostalCode = crDto.PostalCode, Street = crDto.Street
                }));

            CreateMap<UpdateRestaurantDto, Restaurant>();

            CreateMap<Dish, DishDto>();

            CreateMap<CreateDishDto, Dish>();

            CreateMap<RegisterUserDto, User>();
        }
    }
}
