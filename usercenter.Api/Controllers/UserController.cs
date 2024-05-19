using Microsoft.AspNetCore.Mvc;
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

            return ResultUtils.success(currentSafetyUser);
        }

        //[HttpGet]
        //public async Task<BaseResponse<List<User>>?> searchUsers(string? username)
        //{
        //    var safetyUsersList = await _userService.SearchUser(username);
        //    return ResultUtils.success(safetyUsersList);
        //}

        [HttpGet]
        public async Task<BaseResponse<List<User>>?> searchUserList([FromQuery] SearchUserRequest request)
        {
            var userRquest = new User()
            {
                userName = request.username == null ? "" : request.username,
                userAccount = request.userAccount == null ? "" : request.userAccount,
                avatarUrl = request.avatarUrl == null ? "" : request.avatarUrl,
                gender = request.gender,
                phone = request.phone == null ? "" : request.phone,
                email = request.email == null ? "" : request.email,
                userStatus = request.userStatus,
                planetCode = request.planetCode == null ? "" : request.planetCode,
                userRole = request.userRole,
                createTime = request.createTime,
            };
            var safetyUsersList = await _userService.SearchUserList(userRquest);
            return ResultUtils.success(safetyUsersList);
        }

        [HttpPost]
        public async Task<BaseResponse<int>> deleteUser(int id)
        {
            var result = await _userService.DeleteUser(id);
            return ResultUtils.success(result);
        }

        [HttpPost]
        public async Task<BaseResponse<int>> updateUser(CommonUserRequest request)
        {
            var updateUser = new User()
            {
                Id = request.id,
                userName = request.username,
                userAccount = request.userAccount,
                avatarUrl = request.avatarUrl,
                gender = request.gender,
                phone = request.phone,
                email = request.email,
                userStatus = request.userStatus,
                planetCode = request.planetCode,
                userRole = request.userRole,
            };

            var result = await _userService.UpdateUser(updateUser);

            return ResultUtils.success(result);
        }
    }
}
