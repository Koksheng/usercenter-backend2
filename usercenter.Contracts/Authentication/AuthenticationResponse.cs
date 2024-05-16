using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usercenter.Contracts.Authentication
{
    public record AuthenticationResponse(
        int id,
        string username,
        string userAccount,
        string avatarUrl,
        int gender,
        //string userPassword,
        string phone,
        string email,
        int userStatus,
        DateTime createTime,
        DateTime updateTime,
        bool isDelete,
        int userRole,
        string planetCode,
        string token
        );
}
