using System.Globalization;
using System.Threading;
using Vse.AdminkaV1.DomAuthentication;

namespace Vse.AdminkaV1
{
    public class UserContext 
    {
        public UserContext(User user, CultureInfo cultureInfo = null)
        {
            User = user;
            AuditStamp = user.LoginName;
            CultureInfo = cultureInfo ?? Thread.CurrentThread.CurrentCulture;
        }

        public UserContext(string auditStamp, CultureInfo cultureInfo = null)
        {
            AuditStamp = auditStamp;

            CultureInfo = cultureInfo ?? Thread.CurrentThread.CurrentCulture;
        }
        public string AuditStamp { get; private set; }

        public CultureInfo CultureInfo { get; private set; }

        public User User { get; private set; }

        public bool HasPrivilege(string privilegeId)
        {
            var @value = false;
            if (User!=null)
            {
                @value = User.HasPrivilege(privilegeId);
            }
            return @value;
        }
    }
}
