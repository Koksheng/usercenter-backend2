using System.Text.RegularExpressions;
using usercenter.Application.Common.Interfaces.Authentication;
using usercenter.Application.Common.Interfaces.Persistence;
using usercenter.Application.Data;
using usercenter.Contracts.Common;
using usercenter.Domain.Entities;
using usercenter.Domain.Exception;

namespace usercenter.Application.Services.Authentication
{
    public class AuthenticationService : IAuthenticationService
    {
        private readonly DataContext _context;
        private readonly IJwtTokenGenerator _jwtTokenGenerator;
        private readonly IUserRepository _userRepository;
        private const string USER_LOGIN_STATE = "userLoginState";

        public AuthenticationService(DataContext context, IJwtTokenGenerator jwtTokenGenerator, IUserRepository userRepository)
        {
            _context = context;
            _jwtTokenGenerator = jwtTokenGenerator;
            _userRepository = userRepository;
        }

        public async Task<User?> Login(string userAccount, string userPassword)
        {
            if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
            }
            if (userAccount.Length < 4)
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户过短");
            }
            if (userPassword.Length < 8)
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码过短");

            // userAccount cant contain special character
            string pattern = @"[^a-zA-Z0-9\s]";
            if (Regex.IsMatch(userAccount, pattern))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户有特殊字符");
            }

            // 2. check user is exist
            var user = await _userRepository.GetUserByUserAccount(userAccount);
            if (user == null)
            {
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户");
            }
            if (user.isDelete == true)
            {
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户 用户已被删除");
            }
            if (user.userStatus != 1)
            {
                throw new BusinessException(ErrorCode.NULL_ERROR, "找不到该用户 用户已被锁定");
            }
            if (!await _userRepository.CheckUserPassword(user, userPassword))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码不对");
            }

            // 3. 用户脱敏 desensitization
            User safetyUser = await _userRepository.GetSafetyUser(user);

            return safetyUser;
        }

        public async Task<BaseResponse<int>> Register(string userAccount, string userPassword, string checkPassword, string planetCode)
        {
            // 1. Verify
            if (string.IsNullOrWhiteSpace(userAccount) || string.IsNullOrWhiteSpace(userPassword) || string.IsNullOrWhiteSpace(checkPassword) || string.IsNullOrWhiteSpace(planetCode))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "请求参数为空");
            }
            if (userAccount.Length < 4)
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户过短");
            }
            if (userPassword.Length < 8 || checkPassword.Length < 8)
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码过短");

            if (planetCode.Length > 5)
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "星球编号过长");

            // userAccount cant contain special character
            string pattern = @"[^a-zA-Z0-9\s]";
            if (Regex.IsMatch(userAccount, pattern))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "用户账户有特殊字符");
            }
            // userPassword & checkPassword must same
            if (!userPassword.Equals(checkPassword))
            {
                throw new BusinessException(ErrorCode.PARAMS_ERROR, "账户密码与检查密码不对等");
            }

            // userAccount cant existed
            var user = await _userRepository.GetUserByUserAccount(userAccount);
            if (user != null)
            {
                if (user.isDelete == false)
                    throw new BusinessException(ErrorCode.NULL_ERROR, "用户账户已有注册记录");
            }

            // planetCode cant existed
            var planetCodeExists = await _userRepository.CheckPlanetCodeIsExists(planetCode);
            if (planetCodeExists)
            {
                throw new BusinessException(ErrorCode.NULL_ERROR, "星球编号已有注册记录");
            }

            // 2. 加密 (.net core IdentityUser will encrypt themself
            string hashedPassword = await _userRepository.EncryptPassword(userPassword);

            // 3. Insert User to DB
            User newUser = new User(
                0,
                "",
                userAccount,
                "",
                1,
                hashedPassword,
                "",
                "",
                1,
                DateTime.Now,
                DateTime.Now,
                false,
                1,
                planetCode);

            int result = await _userRepository.CreateUser(newUser);

            if(result == 0)
                throw new BusinessException(ErrorCode.STSTEM_ERROR, "创建数据失败");

            return ResultUtils.success(newUser.Id);
        }

        public User GetSafetyUser(User user)
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
