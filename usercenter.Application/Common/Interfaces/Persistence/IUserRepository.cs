using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using usercenter.Domain.Entities;

namespace usercenter.Application.Common.Interfaces.Persistence
{
    public interface IUserRepository
    {
        Task<User> GetUserByUserAccount(string userAccount);
        Task<int> CreateUser(User user);
        Task<int> DeleteUser(User user);
        Task<int> UpdateUser(User user, User updateUser);
        Task<bool> CheckUserPassword(User user, string userPassword);
        Task<User> GetSafetyUser(User user);
        Task<string> EncryptPassword(string userPassword);
        Task<bool> CheckPlanetCodeIsExists(string planetCode, User? existingUser);
        Task<User> GetUser(int id);
        Task<List<User>> GetSimilarUserByUserName(string userName);
        Task<List<User>> SearchUserByFilter(IQueryable<User>? query);
    }
}
