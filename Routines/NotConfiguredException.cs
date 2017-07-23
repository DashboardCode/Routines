using System;

namespace DashboardCode.Routines
{
    public class NotConfiguredException : Exception
    {
        public NotConfiguredException(string message) : base(message)
        {

        }
    }
}
