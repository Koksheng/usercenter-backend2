
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using usercenter.Application.Services.Authentication;
using usercenter.Contracts.Authentication;
using usercenter.Contracts.Common;
using usercenter.Domain.Entities;
using usercenter.Domain.Exception;

namespace usercenter.Api.Controllers
{
    [Route("api/[controller]/[action]")]
    [ApiController]
    public class AuthenticationController : ControllerBase
    {
        private readonly IAuthenticationService _authenticationService;
        private const string USER_LOGIN_STATE = "userLoginState";

        public AuthenticationController(IAuthenticationService authenticationService)
        {
            _authenticationService = authenticationService;
        }

        [HttpPost]
        public async Task<BaseResponse<int>> Register(RegisterRequest request)
        {
            return await _authenticationService.Register(request.userAccount, request.userPassword, request.checkPassword, request.planetCode);
        }

        [HttpPost]
        public async Task<BaseResponse<User>?> Login(LoginRequest request)
        {
            var safetyUser = await _authenticationService.Login(request.userAccount, request.userPassword);
            
            // Convert user object to JSON string
            var serializedSafetyUser = JsonConvert.SerializeObject(safetyUser);

            // add user into session
            if (string.IsNullOrWhiteSpace(HttpContext.Session.GetString(USER_LOGIN_STATE)))
            {
                HttpContext.Session.SetString(USER_LOGIN_STATE, serializedSafetyUser);
            }
            return ResultUtils.success(safetyUser); ;
        }

        [HttpPost]
        public async Task<BaseResponse<int>> Logout()
        {
            var userState = HttpContext.Session.GetString(USER_LOGIN_STATE);
            if (string.IsNullOrWhiteSpace(userState))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "session里找不到用户状态");
            }
            HttpContext.Session.Remove(USER_LOGIN_STATE);
            //return 1;
            return ResultUtils.success(1);
        }
    }
}
