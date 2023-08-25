using System.Collections.Generic;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using RestaurantAPI.Entities;

namespace RestaurantAPI
{
    public class RestaurantSeeder
    {
        private readonly RestaurantDbContext _dbContext;

        public RestaurantSeeder(RestaurantDbContext dBContext)
        {
            _dbContext = dBContext;
        }

        public void Seed()
        {
            if (_dbContext.Database.CanConnect()) 
            {
                if (_dbContext.Database.IsRelational())
                {
                    var pendingMigrations = _dbContext.Database.GetPendingMigrations();
                    if (pendingMigrations != null || pendingMigrations.Any())
                    {
                        _dbContext.Database.Migrate();
                    }
                }

                if (!_dbContext.Restaurants.Any())
                {
                    var restaurants = GetRestaurants();
                    _dbContext.Restaurants.AddRange(restaurants);
                    _dbContext.SaveChanges();
                }

                if (!_dbContext.Roles.Any())
                {
                    var roles = GetRoles();
                    _dbContext.Roles.AddRange(roles);
                    _dbContext.SaveChanges();
                }
            }
        }

        private IEnumerable<Role> GetRoles()
        {
            var roles = new List<Role>() 
            {
                new Role()
                {
                    Name = "Admin",
                },
                new Role()
                {
                    Name = "Manager",
                },
                new Role()
                {
                    Name = "User",
                }
            };

            return roles;
        }

        private IEnumerable<Restaurant> GetRestaurants()
        {
            var restaurants = new List<Restaurant>()
            {
                new Restaurant ()
                {
                    Name = "KFC",
                    Category = "Fast Food",
                    Description = "We have fresh, good and cheap",
                    ContactEmail = "contact@kfc.pl",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish ()
                        {
                            Name = "Chicken Nuggets",
                            Price = 5.40M,
                        },
                        new Dish ()
                        {
                            Name = "Hot Chicken Nuggets",
                            Price = 6.40M,
                        },
                        new Dish ()
                        {
                            Name = "Wraper",
                            Price = 10M,
                        },
                    },
                    Address = new Address ()
                    {
                        City = "Białystok",
                        Street = "Długa 5",
                        PostalCode = "15-877"
                    }
                },
                new Restaurant ()
                {
                    Name = "MC Donald",
                    Category = "Fast Food",
                    Description = "We have fresh, good and cheap",
                    ContactEmail = "contact@mcD.pl",
                    HasDelivery = true,
                    Dishes = new List<Dish>()
                    {
                        new Dish ()
                        {
                            Name = "Big mac",
                            Price = 13.99M,
                        },
                        new Dish ()
                        {
                            Name = "Weiś mac",
                            Price = 16.40M,
                        },
                        new Dish ()
                        {
                            Name = "Hamburger",
                            Price = 4M,
                        },
                    },
                    Address = new Address ()
                    {
                        City = "Białystok",
                        Street = "Długa 5",
                        PostalCode = "15-877"
                    }
                },
            };

            return restaurants;
        }
    }
}
