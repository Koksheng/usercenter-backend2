using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usercenter.Contracts.user
{
    public record SearchUserRequest(
        string? username,
        string? userAccount,
        string? avatarUrl,
        int gender,
        string? phone,
        string? email,
        int userStatus,
        bool? isDelete,
        int userRole,
        string? planetCode,
        DateTime createTime
        );
}
