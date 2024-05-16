
using usercenter.Domain.Entities;

namespace usercenter.Api.Services.Users
{
    public interface IUserService
    {
        //Task<string> EncryptPassword(string userPassword);
        //Task<int> CreateUser(User user);
        Task<User> GetCurrentUser(string userState);
        Task<List<User>> SearchUser(string username);
        //Task<User> GetUser(int id);
        //Task<User> GetUserByUserAccount(string userAccount);
        //Task<bool> CheckPlanetCodeIsExists(string planetCode);
        //Task<List<User>> GetAllUsers();
        //Task<int> UpsertUser(User user, UpsertUserRequest request);
        Task<int> DeleteUser(int id);
        //Task<bool> CheckUserPassword(User user, string userPassword);
        //Task<User> GetSafetyUser(User user);

    }

}
