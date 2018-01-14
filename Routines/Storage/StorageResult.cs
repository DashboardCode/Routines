using System;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public struct StorageResult : IVerboseResult<List<FieldMessage>>
    {
        public List<FieldMessage> Message { get; set; }
        public readonly Exception Exception;
        public StorageResult(Exception exception, List<FieldMessage> fieldErrors)
        {
            Exception = exception;
            if (fieldErrors != null && fieldErrors.Count > 0)
                Message = fieldErrors;
            else
                Message = null;
        }

        public bool IsOk()
        {
            return Message == null;
        }
    }

    public struct FieldMessage
    {
        public readonly string Field;
        public readonly string Message;

        public FieldMessage(string field, string message)
        {
            Field = field;
            Message = message;
        }
    }
}
