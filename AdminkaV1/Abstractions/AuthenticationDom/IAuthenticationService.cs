using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.AuthenticationDom
{
    public interface IAuthenticationService
    {
        User GetUser(string loginName, string firstName, string secondName, IEnumerable<string> groups);
    }
}