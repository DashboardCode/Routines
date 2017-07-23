using System;

namespace DashboardCode.Routines.Injected.Test
{
    public class StateService
    {
        private Guid correlationToken;
        Action<DateTime, string> verbose;
        public StateService(Guid correlationToken, Action<DateTime, string> verbose)
        {
            this.verbose = verbose;
            this.correlationToken = correlationToken;
        }

        public void Verbose(string message)
        {
            verbose(DateTime.Now, message);
        }
    }
}
