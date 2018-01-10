using Microsoft.AspNetCore.Http;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp
{
    public class SessionState
    {
        ISession session;
        UserContext userContext;
        public SessionState(ISession session, UserContext userContext)
        {
            this.session = session;
            this.userContext = userContext;
        }
        public string UserContextKey
        {
            get
            {
                return session.GetString("UserContextKey");
            }
            set
            {
                session.SetString("UserContextKey", value);
            }
        }
    }
}
