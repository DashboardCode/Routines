using System;

namespace DashboardCode.Routines.Storage
{
    public class StorageResultException : Exception
    {
        public readonly StorageResult storageResult;
        public StorageResultException(string message, StorageResult storageResult):base(message, storageResult.Exception) =>
            this.storageResult = storageResult;
    }
}