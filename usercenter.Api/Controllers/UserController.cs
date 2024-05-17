using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using System.Text.RegularExpressions;
using usercenter.Api.Services.Users;
using usercenter.Application.Data;
using usercenter.Domain.Entities;
using usercenter.Contracts.Common;
using usercenter.Contracts.user;
using usercenter.Domain.Exception;

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

        //[HttpPost]
        //public async Task<BaseResponse<int>> userRegister(UserRegisterRequest userRegisterRequest)
        //{
        //    // userAccount = UserName in DB
        //    if (userRegisterRequest == null)
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
        //    }
        //    string userAccount = userRegisterRequest.userAccount;
        //    string userPassword = userRegisterRequest.userPassword;
        //    string checkPassword = userRegisterRequest.checkPassword;
        //    string planetCode = userRegisterRequest.planetCode;

        //    // 1. Verify
        //    if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(checkPassword) || string.IsNullOrWhiteSpace(planetCode))
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
        //    }
        //    if (userAccount.Length < 4)
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户过短");
        //    }
        //    if (userPassword.Length < 8 || checkPassword.Length < 8)
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码过短");

        //    if (planetCode.Length > 5)
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "星球编号过长");

        //    // userAccount cant contain special character
        //    string pattern = @"[^a-zA-Z0-9\s]";
        //    if (Regex.IsMatch(userAccount, pattern))
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户有特殊字符");
        //    }
        //    // userPassword & checkPassword must same
        //    if (!userPassword.Equals(checkPassword))
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码与检查密码不对等");
        //    }

        //    // userAccount cant existed
        //    var user = await _userService.GetUserByUserAccount(userAccount);
        //    if (user != null)
        //    {
        //        if (user.isDelete == false)
        //            throw new BusinessException(ErrorCode.NULL_ERROR, "用户账户已有注册记录");
        //    }

        //    // planetCode cant existed
        //    var planetCodeExists = await _userService.CheckPlanetCodeIsExists(planetCode);
        //    if (planetCodeExists)
        //    {
        //        throw new BusinessException(ErrorCode.NULL_ERROR, "星球编号已有注册记录");
        //    }

        //    // 2. 加密 (.net core IdentityUser will encrypt themself
        //    string hashedPassword = await _userService.EncryptPassword(userPassword);

        //    // 3. Insert User to DB
        //    User newUser = new User(
        //        0,
        //        "",
        //        userAccount,
        //        "",
        //        1,
        //        hashedPassword,
        //        "",
        //        "",
        //        1,
        //        DateTime.Now,
        //        DateTime.Now,
        //        false,
        //        1,
        //        planetCode);

        //    int result = await _userService.CreateUser(newUser);

        //    if(result == 0)
        //        throw new BusinessException(ErrorCode.STSTEM_ERROR, "创建数据失败");

        //    return ResultUtils.success(newUser.Id);
        //}


        //[HttpPost]
        //public async Task<BaseResponse<User>?> userLogin(UserLoginRequest userLoginRequest)
        //{
        //    if (userLoginRequest == null)
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
        //    }
        //    string userAccount = userLoginRequest.userAccount;
        //    string userPassword = userLoginRequest.userPassword;

        //    //logger.LogWarning($"{userAccount} trying to userLogin with password: {userPassword}");

        //    // 1. Verify
        //    if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
        //    }
        //    if (userAccount.Length < 4)
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户过短");
        //    }
        //    if (userPassword.Length < 8)
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码过短");

        //    // userAccount cant contain special character
        //    string pattern = @"[^a-zA-Z0-9\s]";
        //    if (Regex.IsMatch(userAccount, pattern))
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户有特殊字符");
        //    }

        //    // 2. check user is exist
        //    var user = await _userService.GetUserByUserAccount(userAccount);
        //    if (user == null)
        //    {
        //        throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户");
        //    }
        //    if (user.isDelete == true)
        //    {
        //        throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户 用户已被删除");
        //    }
        //    if (user.userStatus != 1)
        //    {
        //        throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户 用户已被锁定");
        //    }
        //    if (!await _userService.CheckUserPassword(user, userPassword))
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码不对");
        //    }


        //    // 3. 用户脱敏 desensitization

        //    User safetyUser = await _userService.GetSafetyUser(user);

        //    // Convert user object to JSON string
        //    var serializedSafetyUser = JsonConvert.SerializeObject(user);

        //    // add user into session
        //    if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(USER_LOGIN_STATE)))
        //    {
        //        HttpContext.Session.SetString(USER_LOGIN_STATE, serializedSafetyUser);
        //    }

        //    //safetyUser.IsAdmin = await verifyIsAdminRoleAsync();
        //    //return safetyUser;
        //    return ResultUtils.success(safetyUser);
        //}

        //[HttpPost]
        //public async Task<BaseResponse<int>> userLogout()
        //{
        //    var userState = HttpContext.Session.GetString(USER_LOGIN_STATE);
        //    if (string.IsNullOrWhiteSpace(userState))
        //    {
        //        throw new BusinessException(ErrorCode.PARAMS_ERROR, "session里找不到用户状态");
        //    }
        //    HttpContext.Session.Remove(USER_LOGIN_STATE);
        //    //return 1;
        //    return ResultUtils.success(1);
        //}


        [HttpGet]
        public async Task<BaseResponse<User>?> getCurrentUser()
        {
            var userState = HttpContext.Session.GetString(USER_LOGIN_STATE);
            if (string.IsNullOrWhiteSpace(userState))
            {
                //return null;
                throw new BusinessException(ErrorCode.NOT_LOGIN);
            }

            var currentSafetyUser = await _userService.GetCurrentUser(userState);


            //// 1. get user by id
            //var loggedInUser = JsonConvert.DeserializeObject<User>(userState);
            //var user = await _userService.GetUser(loggedInUser.Id);
            //if (user == null || user.isDelete)
            //{
            //    //return null;
            //    throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户");
            //}
            //var safetyUser = await _userService.GetSafetyUser(user);
            ////safetyUser.IsAdmin = await verifyIsAdminRoleAsync();
            ////return safetyUser;
            return ResultUtils.success(currentSafetyUser);
        }

        [HttpGet]
        public async Task<BaseResponse<List<User>>?> searchUsers(string? username)
        {
            var safetyUsersList = await _userService.SearchUser(username);
            return ResultUtils.success(safetyUsersList);

            ////// 1. verify permission role
            ////if (!await verifyIsAdminRoleAsync())
            ////{
            ////    //return null;
            ////    throw new BusinessException(ErrorCode.NO_AUTH, "用户没权限");
            ////}

            //if (string.IsNullOrWhiteSpace(username))
            //{
            //    username = "";
            //}

            //var users = await _context.Users.Where(u => u.userName.Contains(username) && u.isDelete == false)
            //.ToListAsync();

            //// Create a list to store simplified user objects
            //List<User> safetyUsersList = new List<User>();

            //// Loop through each user and call getSafetyUser to get simplified user object
            //foreach (var user in users)
            //{
            //    var safetyUser = await _userService.GetSafetyUser(user);
            //    //var safetyUser = await _userService.GetSafetyUser(user);
            //    //safetyUser.IsAdmin = await getIsAdmin(user);
            //    safetyUsersList.Add(safetyUser);
            //}

            //// Return the list of simplified user objects
            ////return safetyUsersList;
            //return ResultUtils.success(safetyUsersList);
        }


        //[HttpPost]
        //public async Task <IActionResult> CreateUser(CreateUserRequest request)
        //{
        //    string hashedPassword = await _userService.EncryptPassword(request.userPassword);
        //    User user = new User(
        //        0,
        //        request.username,
        //        request.userAccount,
        //        request.avatarUrl,
        //        request.gender,
        //        hashedPassword,
        //        request.phone,
        //        request.email,
        //        1,
        //        DateTime.Now,
        //        DateTime.Now,
        //        false,
        //        request.userRole,
        //        request.planetCode);

        //    //_context.Users.Add(user);
        //    //var result = _context.SaveChanges();
        //    await _userService.CreateUser(user);

        //    // Use Task.Run to call GetUser asynchronously and await the result
        //    var createdUser = await Task.Run(() => _userService.GetUser(user.Id));

        //    var response = new UserResponse(
        //            createdUser.Id,
        //            createdUser.userName,
        //            createdUser.userAccount,
        //            createdUser.avatarUrl,
        //            createdUser.gender,
        //            createdUser.phone,
        //            createdUser.email,
        //            createdUser.userStatus,
        //            createdUser.createTime,
        //            createdUser.updateTime,
        //            createdUser.isDelete,
        //            createdUser.userRole,
        //            createdUser.planetCode
        //        );

        //    return CreatedAtAction(
        //            actionName : nameof(GetUser),
        //            routeValues: new {id = user.Id},
        //            value      : response
        //        );
        //}

        //[HttpGet]
        //public async Task <ActionResult<User>> GetUser(int id)
        //{
        //    //var user = await _context.Users.FindAsync(id);
        //    //if(user is null) 
        //    //{
        //    //    return NotFound("user not found");
        //    //}
        //    //return Ok(user);

        //    //User user = _userService.GetUser(id);
        //    // Use Task.Run to call GetUser asynchronously and await the result
        //    var createdUser = await Task.Run(() => _userService.GetUser(id));

        //    var response = new UserResponse(
        //        createdUser.Id,
        //        createdUser.userName,
        //        createdUser.userAccount,
        //        createdUser.avatarUrl,
        //        createdUser.gender,
        //        createdUser.phone,
        //        createdUser.email,
        //        createdUser.userStatus,
        //        createdUser.createTime,
        //        createdUser.updateTime,
        //        createdUser.isDelete,
        //        createdUser.userRole,
        //        createdUser.planetCode
        //        );
        //    return Ok( response );
        //}
        //private async Task<List<User>> getAllUsers()
        //{
        //    return await _context.Users.Where(u => !u.isDelete).ToListAsync();
        //}

        //[HttpGet]
        //public async Task <ActionResult<List<User>>> GetAllUsers()
        //{
        //    var users = await _userService.GetAllUsers();
        //    return Ok(users);
        //}


        //[HttpPut]
        //public async Task<ActionResult<List<User>>> UpsertUser(UpsertUserRequest request)
        //{
        //    var dbUser = await _context.Users.FindAsync(request.id);
        //    if (dbUser is null)
        //        return NotFound("User not found");

        //    //dbUser.userName = request.username;
        //    //dbUser.userAccount = request.userAccount;

        //    //await _context.SaveChangesAsync();
        //    var result = await _userService.UpsertUser(dbUser, request);
        //    //if(result == 1)

        //    return Ok(await _userService.GetAllUsers());
        //}
        [HttpPost]
        public async Task<BaseResponse<int>> deleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            return ResultUtils.success(result);
        }

        [HttpPost]
        public async Task<BaseResponse<int>> updateUser(UpsertUserRequest request)
        {
            //var result = await _userService.DeleteUser(id);
            var user = new User() { 
                Id = request.id,
            };

            return ResultUtils.success(1);
        }


    }
}
