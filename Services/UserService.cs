using RestaurantAPI.Models.User;

namespace RestaurantAPI.Services
{
    public interface IUserService
    {
        int Create(CreateUserDto createUserDto);
        void Delete(int userId);
        int Update(UpdateUserDto updateUserDto);
    }

    public class UserService : IUserService
    {

    }
}
