using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usercenter.Application.Services.Authentication
{
    public record AuthenticationResult(
        int Id,
        string userName,
        string userAccount,
        string Token);
}
