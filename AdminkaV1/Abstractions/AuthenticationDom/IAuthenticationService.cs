using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public interface IAuthenticationService
    {
        Task<User> GetUserAsync(string loginName, string firstName, string secondName, IEnumerable<string> groups);
        Task<User> GetUserAsync(string loginName, string firstName, string secondName, Func<string, bool> isInRole);
    }
}