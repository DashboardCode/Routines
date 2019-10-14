using System;
using System.Linq;
using System.Collections.Generic;

namespace DashboardCode.Routines.Storage
{
    public struct StorageResult : IVerboseResult<FormMessages>
    {
        public FormMessages Message { get; set; }
        public readonly Exception Exception;
        public StorageResult(Exception exception, FormMessages formMessages)
        {
            Exception = exception;
            if (formMessages != null && (formMessages.DirectMessages.Count > 0 || formMessages.EntityValidationMessages.Count>0))
                Message = formMessages;
            else
                Message = null;
        }

        public bool IsOk()
        {
            return Message == null;
        }

        public int GetCount()
        {
            return Message == null ? 0 : Message.DirectMessages.Count() 
                + Message.EntityValidationMessages.Values.SelectMany(e => e.FieldMessages).Count()
                + Message.EntityValidationMessages.Values.SelectMany(e => e.Messages).Count(); 
        }

        public IEnumerable<string> GetAllMessages()
        {
            if (Message == null)
                return new List<string>();
            else
            {
                return Message.DirectMessages
                    .Union(Message.EntityValidationMessages.Values.SelectMany(e => e.Messages)
                    .Union(Message.EntityValidationMessages.Values.SelectMany(e => e.FieldMessages).Select(e=>e.Message)));
            }
        }

        public IEnumerable<FieldValidationMessage> GetAllFieldMessages()
        {
            if (Message == null)
                return new List<FieldValidationMessage>();
            else
            {
                return Message.EntityValidationMessages.Values.SelectMany(e => e.FieldMessages);
            }
        }
    }

    public class FormMessages
    {
        public readonly List<string> DirectMessages = new List<string>();
        public readonly Dictionary<string, EntityValidationMessages> EntityValidationMessages = new Dictionary<string, EntityValidationMessages>();

        public void Add(string entityName, string fieldName, string message)
        {
            if (entityName == null)
                DirectMessages.Add(message);
            else
            {
                EntityValidationMessages entityValidationMessages;
                if (!EntityValidationMessages.TryGetValue(entityName, out entityValidationMessages))
                {
                    entityValidationMessages = new EntityValidationMessages();
                    EntityValidationMessages[entityName] = entityValidationMessages;
                }
                if (fieldName==null)
                {
                    entityValidationMessages.Messages.Add(message);
                }
                else
                {
                    entityValidationMessages.FieldMessages.Add(fieldName, message);
                }

            }

        }
    }

    public class EntityValidationMessages
    {
        public readonly List<string> Messages = new List<string>();
        public readonly List<FieldValidationMessage> FieldMessages = new List<FieldValidationMessage>();
    }

    public struct FieldValidationMessage
    {
        public readonly string Field;
        public readonly string Message;

        public FieldValidationMessage(string field, string message)
        {
            Field = field;
            Message = message;
        }
    }
}