using usercenter.Contracts.Common;
using usercenter.Domain.Entities;

namespace usercenter.Application.Services.Authentication
{
    public interface IAuthenticationService
    {
        Task<User?> Login(string userAccount, string userPassword);
        Task<BaseResponse<int>> Register(string userAccount, string userPassword, string checkPassword, string planetCode);

    }
}
