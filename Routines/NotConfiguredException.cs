using System;

namespace Vse.Routines
{
    public class NotConfiguredException : Exception
    {
        public NotConfiguredException(string message) : base(message)
        {

        }
    }
}
