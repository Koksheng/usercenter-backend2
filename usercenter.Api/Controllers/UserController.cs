using Azure.Core;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;
using System.Numerics;
using System.Reflection;
using System.Text.RegularExpressions;
using usercenter.Api.Common;
using usercenter.Api.Data;
using usercenter.Api.Exception;
using usercenter.Api.Models;
using usercenter.Api.Services.Users;
using usercenter.Contracts.user;

namespace usercenter.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class UserController : ControllerBase
    {
        private readonly DataContext _context;
        private readonly IUserService _userService;
        private const string USER_LOGIN_STATE = "userLoginState";

        public UserController(DataContext context, IUserService userService)
        {
            _context = context;
            _userService = userService;
        }


        [HttpPost]
        public async Task<BaseResponse<User>?> userLogin(UserLoginRequest userLoginRequest)
        {
            if (userLoginRequest == null)
            {
                //return null;
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
            }
            string userAccount = userLoginRequest.userAccount;
            string userPassword = userLoginRequest.userPassword;

            //logger.LogWarning($"{userAccount} trying to userLogin with password: {userPassword}");

            // 1. Verify
            if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
            {
                //return null;
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
            }
            if (userAccount.Length < 4)
            {
                //return null;
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户过短");
            }
            if (userPassword.Length < 8)
                //return null;
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码过短");

            // userAccount cant contain special character
            string pattern = @"[^a-zA-Z0-9\s]";
            if (Regex.IsMatch(userAccount, pattern))
            {
                //return null;
                //return ResultUtils.error<User>(ErrorCode.PARAMS_ERROR);
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户有特殊字符");
            }

            // 2. check user is exist
            //var user = await userManager.FindByNameAsync(userAccount);
            var user = await _userService.GetUserByUserAccount(userAccount);
            if (user == null)
            {
                //return null;
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户");
            }
            if (user.isDelete == true)
            {
                //return null;
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户 用户已被删除");
            }
            if (user.userStatus != 1)
            {
                //return null;
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户 用户已被锁定");
            }
            if (!await _userService.CheckUserPassword(user, userPassword))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码不对");
            }


            // 3. 用户脱敏 desensitization

            User safetyUser = await _userService.GetSafetyUser(user);

            // Convert user object to JSON string
            var serializedSafetyUser = JsonConvert.SerializeObject(user);

            // add user into session
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(USER_LOGIN_STATE)))
            {
                HttpContext.Session.SetString(USER_LOGIN_STATE, serializedSafetyUser);
            }

            //safetyUser.IsAdmin = await verifyIsAdminRoleAsync();
            //return safetyUser;
            return ResultUtils.success(safetyUser);
        }

        [HttpGet]
        public async Task<BaseResponse<User>?> getCurrentUser()
        {
            var userState = HttpContext.Session.GetString(USER_LOGIN_STATE);
            if (string.IsNullOrWhiteSpace(userState))
            {
                //return null;
                throw new BusinessException(ErrorCode.NOT_LOGIN);
            }


            // 1. get user by id
            var loggedInUser = JsonConvert.DeserializeObject<User>(userState);
            var user = await _userService.GetUser(loggedInUser.Id);
            if (user == null || user.isDelete)
            {
                //return null;
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户");
            }
            var safetyUser = await _userService.GetSafetyUser(user);
            //safetyUser.IsAdmin = await verifyIsAdminRoleAsync();
            //return safetyUser;
            return ResultUtils.success(safetyUser);
        }

        [HttpGet]
        public async Task<BaseResponse<List<User>>?> searchUsers(string? username)
        {

            //// 1. verify permission role
            //if (!await verifyIsAdminRoleAsync())
            //{
            //    //return null;
            //    throw new BusinessException(ErrorCode.NO_AUTH, "用户没权限");
            //}

            if (string.IsNullOrWhiteSpace(username))
            {
                username = "";
            }

            var users = await _context.Users.Where(u => u.userName.Contains(username) && u.isDelete == false)
            .ToListAsync();

            // Create a list to store simplified user objects
            List<User> safetyUsersList = new List<User>();

            // Loop through each user and call getSafetyUser to get simplified user object
            foreach (var user in users)
            {
                var safetyUser = await _userService.GetSafetyUser(user);
                //safetyUser.IsAdmin = await getIsAdmin(user);
                safetyUsersList.Add(safetyUser);
            }

            // Return the list of simplified user objects
            //return safetyUsersList;
            return ResultUtils.success(safetyUsersList);
        }


        [HttpPost]
        public async Task <IActionResult> CreateUser(CreateUserRequest request)
        {
            string hashedPassword = await _userService.EncryptPassword(request.userPassword);
            User user = new User(
                0,
                request.username,
                request.userAccount,
                request.avatarUrl,
                request.gender,
                hashedPassword,
                request.phone,
                request.email,
                1,
                DateTime.Now,
                DateTime.Now,
                false,
                request.userRole,
                request.planetCode);

            //_context.Users.Add(user);
            //var result = _context.SaveChanges();
            await _userService.CreateUser(user);

            // Use Task.Run to call GetUser asynchronously and await the result
            var createdUser = await Task.Run(() => _userService.GetUser(user.Id));

            var response = new UserResponse(
                    createdUser.Id,
                    createdUser.userName,
                    createdUser.userAccount,
                    createdUser.avatarUrl,
                    createdUser.gender,
                    createdUser.phone,
                    createdUser.email,
                    createdUser.userStatus,
                    createdUser.createTime,
                    createdUser.updateTime,
                    createdUser.isDelete,
                    createdUser.userRole,
                    createdUser.planetCode
                );

            return CreatedAtAction(
                    actionName : nameof(GetUser),
                    routeValues: new {id = user.Id},
                    value      : response
                );
        }

        [HttpGet]
        public async Task <ActionResult<User>> GetUser(int id)
        {
            //var user = await _context.Users.FindAsync(id);
            //if(user is null) 
            //{
            //    return NotFound("user not found");
            //}
            //return Ok(user);

            //User user = _userService.GetUser(id);
            // Use Task.Run to call GetUser asynchronously and await the result
            var createdUser = await Task.Run(() => _userService.GetUser(id));

            var response = new UserResponse(
                createdUser.Id,
                createdUser.userName,
                createdUser.userAccount,
                createdUser.avatarUrl,
                createdUser.gender,
                createdUser.phone,
                createdUser.email,
                createdUser.userStatus,
                createdUser.createTime,
                createdUser.updateTime,
                createdUser.isDelete,
                createdUser.userRole,
                createdUser.planetCode
                );
            return Ok( response );
        }
        //private async Task<List<User>> getAllUsers()
        //{
        //    return await _context.Users.Where(u => !u.isDelete).ToListAsync();
        //}

        [HttpGet]
        public async Task <ActionResult<List<User>>> GetAllUsers()
        {
            var users = await _userService.GetAllUsers();
            return Ok(users);
        }


        [HttpPut]
        public async Task<ActionResult<List<User>>> UpsertUser(UpsertUserRequest request)
        {
            var dbUser = await _context.Users.FindAsync(request.id);
            if (dbUser is null)
                return NotFound("User not found");

            //dbUser.userName = request.username;
            //dbUser.userAccount = request.userAccount;

            //await _context.SaveChangesAsync();
            var result = await _userService.UpsertUser(dbUser, request);
            //if(result == 1)

            return Ok(await _userService.GetAllUsers());
        }
        [HttpDelete]
        public async Task<ActionResult<List<User>>> DeleteUser(int id)
        {

            //var createdUser = await Task.Run(() => _userService.GetUser(user.Id));
            //var dbUser = await _userService.GetUser(id);
            //if (dbUser is null)
            //    return NotFound("User not found");

            //dbUser.isDelete = true;
            //await _context.SaveChangesAsync();

            var result = await _userService.DeleteUser(id);


            return Ok(await _userService.GetAllUsers());
        }
    }
}
