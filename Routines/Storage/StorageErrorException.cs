using System;

namespace Vse.Routines.Storage
{
    public class StorageErrorException : Exception
    {
        public StorageErrorException(string message, Exception exception):base(message, exception)
        {

        }
    }
}
