using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public class StorageError
    {
        public readonly List<FieldError> FieldErrors;
        public readonly Exception Exception;
        public StorageError(Exception exception, List<FieldError> fieldErrors)
        {
            Exception = exception;
            FieldErrors = fieldErrors;
        }
    }

    public class FieldError
    {
        public readonly string Field;
        public readonly string Message;

        public FieldError(string Field, string Message)
        {
            this.Field = Field;
            this.Message = Message;
        }
    }
}
