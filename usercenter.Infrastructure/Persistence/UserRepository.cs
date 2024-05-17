using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using usercenter.Application.Common;
using usercenter.Application.Common.Interfaces.Persistence;
using usercenter.Application.Data;
using usercenter.Domain.Entities;

namespace usercenter.Infrastructure.Persistence
{
    public class UserRepository : IUserRepository
    {

        private readonly DataContext _context;

        public UserRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<User> GetUserByUserAccount(string userAccount)
        {
            var user = await _context.Users
                             .Where(u => !u.isDelete && u.userAccount == userAccount)
                             .FirstOrDefaultAsync();
            return user;
        }

        public async Task<int> CreateUser(User user)
        {
            // Check if user already exists
            var newUser = await _context.Users.AddAsync(user);
            var result = await _context.SaveChangesAsync();

            return result;
        }
        public async Task<int> DeleteUser(User user)
        {
            // Check if user already exists
            user.isDelete = true;
            user.updateTime = DateTime.Now;
            var result = await _context.SaveChangesAsync();

            return result;
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

            //// Access the newly generated user ID
            //int newUserId = user.Id;
            //string userName = user.userName;
            //var token = _jwtTokenGenerator.GenerateToken(newUserId, userName);

            return safetyUser;
        }

        public async Task<string> EncryptPassword(string userPassword)
        {
            string hashedPassword = EncryptionService.HashPasswordWithKey(userPassword, "usercenter");
            return hashedPassword;
        }

        public async Task<bool> CheckPlanetCodeIsExists(string planetCode)
        {
            var user = await _context.Users
                             .Where(u => !u.isDelete && u.planetCode == planetCode)
                             .FirstOrDefaultAsync();
            if (user != null)
                return true;
            else
                return false;
        }

        public async Task<User> GetUser(int id)
        {
            var user = await _context.Users
                             .Where(u => !u.isDelete && u.Id == id)
                             .FirstOrDefaultAsync();
            return user;
        }

        public async Task<List<User>> GetSimilarUserByUserName(string userName)
        {
            var users = await _context.Users.Where(u => u.userName.Contains(userName) && u.isDelete == false)
            .ToListAsync();

            return users;
        }
    }
}
