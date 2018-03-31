using System;

namespace DashboardCode.Routines.Injected.Test
{
    public class CustomRoutineClosure
    {
        private Guid correlationToken;
        Action<DateTime, string> verbose;
        public CustomRoutineClosure(Guid correlationToken, Action<DateTime, string> verbose)
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