using Microsoft.AspNetCore.Mvc;
using usercenter.Api.Models;
using usercenter.Contracts.user;

namespace usercenter.Api.Services.Users
{
    public interface IUserService
    {
        Task CreateUser(User user);
        Task<User> GetUser(int id);
        Task<List<User>> GetAllUsers();
        Task<int> UpsertUser(User user, UpsertUserRequest request);
        Task<bool> DeleteUser(int id);
    }

}
