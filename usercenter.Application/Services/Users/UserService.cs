using Microsoft.EntityFrameworkCore;
using usercenter.Application.Common;
using usercenter.Application.Common.Interfaces.Authentication;
using usercenter.Application.Data;
using usercenter.Domain.Entities;

namespace usercenter.Api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        public UserService(DataContext context, IJwtTokenGenerator jwtTokenGenerator)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
        }

        public async Task<string> EncryptPassword(string userPassword)
        {
            string hashedPassword = EncryptionService.HashPasswordWithKey(userPassword, "usercenter");
            return hashedPassword;
        }

        public async Task<int> CreateUser(User user)
        {
            // Check if user already exists
            var newUser = await _context.Users.AddAsync(user);
            var result = await _context.SaveChangesAsync();

            // Access the newly generated user ID
            int newUserId = newUser.Entity.Id;
            string userName = newUser.Entity.userName;
            var token = _jwtTokenGenerator.GenerateToken(newUserId, userName);
            return result;
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

        public async Task<bool> CheckPlanetCodeIsExists(string planetCode)
        {
            var user = await _context.Users
                             .Where(u => !u.isDelete && u.planetCode == planetCode)
                             .FirstOrDefaultAsync();
            if(user != null)
                return true;
            else
                return false;
        }

        public async Task<List<User>> GetAllUsers()
        {
            return await _context.Users.Where(u => !u.isDelete).ToListAsync();
        }

        //public async Task<int> UpsertUser(User user, UpsertUserRequest request)
        //{
        //    user.userName = request.username;
        //    user.userAccount = request.userAccount;

        //    var result = await _context.SaveChangesAsync();
        //    return result;
        //}

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

            // Access the newly generated user ID
            int newUserId = user.Id;
            string userName = user.userName;
            var token = _jwtTokenGenerator.GenerateToken(newUserId, userName);

            return safetyUser;
        }
    }
}
