using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using usercenter.Application.Common;
using usercenter.Application.Common.Interfaces.Authentication;
using usercenter.Application.Common.Interfaces.Persistence;
using usercenter.Application.Data;
using usercenter.Contracts.Common;
using usercenter.Domain.Entities;
using usercenter.Domain.Exception;

namespace usercenter.Api.Services.Users
{
    public class UserService : IUserService
    {
        private readonly DataContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        public UserService(DataContext context, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
        }

        //public async Task<string> EncryptPassword(string userPassword)
        //{
        //    string hashedPassword = EncryptionService.HashPasswordWithKey(userPassword, "usercenter");
        //    return hashedPassword;
        //}

        //public async Task<int> CreateUser(User user)
        //{
        //    // Check if user already exists
        //    var newUser = await _context.Users.AddAsync(user);
        //    var result = await _context.SaveChangesAsync();

        //    // Access the newly generated user ID
        //    int newUserId = newUser.Entity.Id;
        //    string userName = newUser.Entity.userName;
        //    var token = _jwtTokenGenerator.GenerateToken(newUserId, userName);
        //    return result;
        //}

        public async Task<User> GetCurrentUser(string userState)
        {
            // 1. get user by id
            var loggedInUser = JsonConvert.DeserializeObject<User>(userState);
            var user = await _userRepository.GetUser(loggedInUser.Id);

            if (user == null || user.isDelete)
            {
                //return null;
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户");
            }
            var safetyUser = await _userRepository.GetSafetyUser(user);
            //safetyUser.IsAdmin = await verifyIsAdminRoleAsync();
            //return safetyUser;
            return safetyUser;
        }

        public async Task<List<User>> SearchUser(string userName)
        {
            if (string.IsNullOrWhiteSpace(userName))
            {
                userName = "";
            }

            var users = await _userRepository.GetSimilarUserByUserName(userName);

            // Create a list to store simplified user objects
            List<User> safetyUsersList = new List<User>();

            // Loop through each user and call getSafetyUser to get simplified user object
            foreach (var user in users)
            {
                var safetyUser = await _userRepository.GetSafetyUser(user);
                safetyUsersList.Add(safetyUser);

            }

            return safetyUsersList;
        }

        //public async Task<User> GetUser(int id)
        //{
        //    var user = await _context.Users
        //                     .Where(u => !u.isDelete && u.Id == id)
        //                     .FirstOrDefaultAsync();
        //    return user;
        //}

        //public async Task<User> GetUserByUserAccount(string userAccount)
        //{
        //    var user = await _context.Users
        //                     .Where(u => !u.isDelete && u.userAccount == userAccount)
        //                     .FirstOrDefaultAsync();
        //    return user;
        //}

        //public async Task<bool> CheckPlanetCodeIsExists(string planetCode)
        //{
        //    var user = await _context.Users
        //                     .Where(u => !u.isDelete && u.planetCode == planetCode)
        //                     .FirstOrDefaultAsync();
        //    if(user != null)
        //        return true;
        //    else
        //        return false;
        //}

        //public async Task<List<User>> GetAllUsers()
        //{
        //    return await _context.Users.Where(u => !u.isDelete).ToListAsync();
        //}

        //public async Task<int> UpsertUser(User user, UpsertUserRequest request)
        //{
        //    user.userName = request.username;
        //    user.userAccount = request.userAccount;

        //    var result = await _context.SaveChangesAsync();
        //    return result;
        //}

        public async Task<int> DeleteUser(int id)
        {
            var user = await _userRepository.GetUser(id);
            if (user is null)
                throw new BusinessException(ErrorCode.NULL_ERROR, "用户不存在");

            // Soft Delete it
            int result = await _userRepository.DeleteUser(user); 

            if (result == 0)
                throw new BusinessException(ErrorCode.STSTEM_ERROR, "删除数据失败");
            return result;
        }

        //public async Task<bool> CheckUserPassword(User user, string userPassword)
        //{
        //    string hashedPassword = await EncryptPassword(userPassword);
        //    if (user.userPassword == hashedPassword)
        //    {
        //        return true;
        //    }
        //    else
        //    {
        //        return false;
        //    }
        //}

        //public async Task<User> GetSafetyUser(User user)
        //{
        //    User safetyUser = new User(
        //        user.Id, 
        //        user.userName,
        //        user.userAccount,
        //        user.avatarUrl,
        //        user.gender,
        //        user.phone,
        //        user.email,
        //        user.userStatus,
        //        user.createTime,
        //        user.updateTime,
        //        user.isDelete,
        //        user.userRole,
        //        user.planetCode
        //        );

        //    // Access the newly generated user ID
        //    int newUserId = user.Id;
        //    string userName = user.userName;
        //    var token = _jwtTokenGenerator.GenerateToken(newUserId, userName);

        //    return safetyUser;
        //}
    }
}
