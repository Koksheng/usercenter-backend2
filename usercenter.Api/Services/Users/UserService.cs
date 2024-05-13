using Azure.Core;
using Microsoft.EntityFrameworkCore;
using Microsoft.IdentityModel.Tokens;
using usercenter.Api.Common;
using usercenter.Api.Data;
using usercenter.Api.Models;
using usercenter.Contracts.user;

namespace usercenter.Api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        public UserService(DataContext context)
        {
            _context = context;
        }

        public async Task<string> EncryptPassword(string userPassword)
        {
            string hashedPassword = EncryptionService.HashPasswordWithKey(userPassword, "usercenter");
            return hashedPassword;
        }

        public async Task CreateUser(User user)
        {
            var abc = await _context.Users.AddAsync(user);
            var result = await _context.SaveChangesAsync();
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
                             .Where(u => !u.isDelete && u.Id == id)
                             .FirstOrDefaultAsync();
            return user;
        }

        public async Task<User> GetUserByUserAccount(string userAccount)
        {
            var user = await _context.Users
                             .Where(u => !u.isDelete && u.userAccount == userAccount)
                             .FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.Where(u => !u.isDelete).ToListAsync();
        }

        public async Task<int> UpsertUser(User user, UpsertUserRequest request)
        {
            user.userName = request.username;
            user.userAccount = request.userAccount;

            var result = await _context.SaveChangesAsync();
            return result;
        }

        public async Task<bool> DeleteUser(int id)
        {
            var user = await GetUser(id);
            if (user is null)
                return false;
            user.isDelete = true;
            await _context.SaveChangesAsync();
            return true;
        }

        public async Task<bool> CheckUserPassword(User user, string userPassword)
        {
            string hashedPassword = await EncryptPassword(userPassword);
            if (user.userPassword == hashedPassword)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        public async Task<User> GetSafetyUser(User user)
        {
            User safetyUser = new User(
                user.Id, 
                user.userName,
                user.userAccount,
                user.avatarUrl,
                user.gender,
                user.phone,
                user.email,
                user.userStatus,
                user.createTime,
                user.updateTime,
                user.isDelete,
                user.userRole,
                user.planetCode
                );

            return safetyUser;
        }
    }
}
