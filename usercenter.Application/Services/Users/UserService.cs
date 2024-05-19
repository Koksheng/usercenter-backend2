using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
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

        public async Task<List<User>> SearchUserList(User user)
        {
            var query = _context.Users.Where(u => !u.isDelete);

            if (!string.IsNullOrEmpty(user.userName))
            {
                query = query.Where(u => u.userName.Contains(user.userName));
            }

            if (!string.IsNullOrEmpty(user.userAccount))
            {
                query = query.Where(u => u.userAccount.Contains(user.userAccount));
            }

            if (!string.IsNullOrEmpty(user.avatarUrl))
            {
                query = query.Where(u => u.avatarUrl.Contains(user.avatarUrl));
            }

            if (user.gender != 0)
            {
                query = query.Where(u => u.gender == user.gender);
            }

            if (!string.IsNullOrEmpty(user.phone))
            {
                query = query.Where(u => u.phone.Contains(user.phone));
            }

            if (!string.IsNullOrEmpty(user.email))
            {
                query = query.Where(u => u.email.Contains(user.email));
            }

            if (user.userStatus != 0)
            {
                query = query.Where(u => u.userStatus == user.userStatus);
            }

            if (!string.IsNullOrEmpty(user.planetCode))
            {
                query = query.Where(u => u.planetCode.Contains(user.planetCode));
            }

            if (user.userRole != 0)
            {
                query = query.Where(u => u.userRole == user.userRole);
            }

            // Check if createTime has been set (not default)
            if (user.createTime != default(DateTime))
            {
                query = query.Where(u => u.createTime == user.createTime);
            }

            var users = await _userRepository.SearchUserByFilter(query);

            // Create a list to store simplified user objects
            List<User> safetyUsersList = new List<User>();

            // Loop through each user and call getSafetyUser to get simplified user object
            foreach (var u in users)
            {
                var safetyUser = await _userRepository.GetSafetyUser(u);
                safetyUsersList.Add(safetyUser);

            }

            return safetyUsersList;
        }

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

        public async Task<int> UpdateUser(User updateUser)
        {
            var user = await _userRepository.GetUser(updateUser.Id);
            // 1. verify
            if (user is null)
                throw new BusinessException(ErrorCode.NULL_ERROR, "用户不存在");

            if (string.IsNullOrWhiteSpace(updateUser.userAccount) || string.IsNullOrWhiteSpace(updateUser.planetCode))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
            }
            if (updateUser.userAccount.Length < 4)
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户过短");
            }

            if (updateUser.planetCode.Length > 5)
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "星球编号过长");

            // userAccount cant contain special character
            string pattern = @"[^a-zA-Z0-9\s]";
            if (Regex.IsMatch(updateUser.userAccount, pattern))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户有特殊字符");
            }

            // planetCode cant existed
            var planetCodeExists = await _userRepository.CheckPlanetCodeIsExists(updateUser.planetCode, user);
            if (planetCodeExists)
            {
                throw new BusinessException(ErrorCode.NULL_ERROR, "星球编号已有注册记录");
            }

            // Update it
            int result = await _userRepository.UpdateUser(user, updateUser);

            if (result == 0)
                throw new BusinessException(ErrorCode.STSTEM_ERROR, "删除数据失败");
            return result;
        }

    }
}
