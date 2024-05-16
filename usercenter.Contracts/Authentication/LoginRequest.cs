using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace usercenter.Contracts.Authentication
{
    public record LoginRequest(string userAccount, string userPassword);
}
