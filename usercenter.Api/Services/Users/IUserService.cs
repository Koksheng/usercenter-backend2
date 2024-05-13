using Microsoft.AspNetCore.Mvc;
using usercenter.Api.Models;
using usercenter.Contracts.user;

namespace usercenter.Api.Services.Users
{
    public interface IUserService
    {
        Task<string> EncryptPassword(string userPassword);
        Task CreateUser(User user);
        Task<User> GetUser(int id);
        Task<User> GetUserByUserAccount(string userAccount);
        Task<List<User>> GetAllUsers();
        Task<int> UpsertUser(User user, UpsertUserRequest request);
        Task<bool> DeleteUser(int id);
        Task<bool> CheckUserPassword(User user, string userPassword);
        Task<User> GetSafetyUser(User user);

    }

}
