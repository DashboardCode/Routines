using System.Globalization;

namespace DashboardCode.AdminkaV1
{
    public class AnonymousUserContext
    {
        public AnonymousUserContext(string auditStamp = "Anonymous")
        {
            AuditStamp = auditStamp;
            CultureInfo = CultureInfo.CurrentCulture;
        }

        public string AuditStamp { get; private set; }

        public CultureInfo CultureInfo { get; private set; }
    }
}
