
using usercenter.Domain.Entities;

namespace usercenter.Api.Services.Users
{
    public interface IUserService
    {
        Task<User> GetCurrentUser(string userState);
        Task<List<User>> SearchUser(string username);
        Task<List<User>> SearchUserList(User user);
        Task<int> DeleteUser(int id);
        Task<int> UpdateUser(User updateUser);

    }

}
