using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usercenter.Contracts.user
{
    public record UserRegisterRequest(string userAccount, string userPassword, string checkPassword, string planetCode);
}
