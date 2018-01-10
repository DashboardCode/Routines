using System.Collections.Generic;

namespace DashboardCode.Routines
{
    public struct ConvertResult<T> : IValuableVerboseResult<T, List<string>> 
    {
        public T Value { get; set; }

        public ConvertResult(string[] messages)
        {
            Value = default(T);
            if (messages != null)
                Message = new List<string>(messages);
            else
                Message = null;
        }
        public List<string> Message { get; set; }

        public bool IsOk() => Message==null;

        public IVerboseResult<List<string>> ToVerboseResult() =>
            new VerboseResult<List<string>>(Message);
    }

    
    public struct BinderResult : IVerboseResult<List<string>>
    {
        public BinderResult(string errorMessage)
        {
            if (errorMessage != null)
                Message = new List<string>() { errorMessage };
            else
                Message = null;
        }

        public BinderResult(List<string> errorMessages)
        {
            if (errorMessages != null && errorMessages.Count > 0)
                Message = errorMessages;
            else
                Message = null;
        }

        public BinderResult(string[] errorMessages)
        {
            if (errorMessages != null && errorMessages.Length > 0)
                Message = new List<string>(errorMessages);
            else
                Message = null;
        }

        public List<string> Message { get; set; }
        public bool IsOk() { return Message == null; }
    }

    public interface IResult
    {
        bool IsOk();
    }

    public interface IVerboseResult<TMessage> : IResult
    {
        TMessage Message { get; set; }
    }

    public interface IValuable<TValue>
    {
        TValue Value { get; set; }
    }

    public interface IValuableVerboseResult<TValue,TMessage>: IValuable<TValue>, IVerboseResult<TMessage>
    {
        IVerboseResult<TMessage> ToVerboseResult();
    }

    public struct ValuableResult<TValue> : IResult, IValuable<TValue>
    {
        public TValue Value { get; set; }
        private bool success;
        public ValuableResult(TValue value, bool success)
        {
            Value = value;
            this.success = success;
        }
        public bool IsOk() => success;
    }

    public struct VerboseResult<TMessage> : IVerboseResult<TMessage>
    {
        public VerboseResult(TMessage message)=>
            Message = message;

        public bool IsOk() => EqualityComparer<TMessage>.Default.Equals(Message, default(TMessage));
        public TMessage Message { get; set; }
    }

    public struct ValuableVerboseResult<TValue,TMessage> : IValuableVerboseResult<TValue, TMessage>
    {
        public ValuableVerboseResult(TValue value, TMessage message)
        {
            Message = message;
            Value = value;
        }

        public bool IsOk() => EqualityComparer<TMessage>.Default.Equals(Message, default(TMessage));


        public IVerboseResult<TMessage> ToVerboseResult()
        {
            return new VerboseResult<TMessage>(Message);
        }

        public TMessage Message { get; set; }

        public TValue Value { get ; set; }
    }

    public interface IComplexBinderResult<TValue>: IValuableVerboseResult<TValue, List<(string, List<string>)>>
    {

    }

    public struct ComplexBinderResult<TValue> : IComplexBinderResult<TValue>
    {
        public ComplexBinderResult(TValue value) : this (value, null)
        {
        }

        public ComplexBinderResult(TValue value, List<(string, List<string>)> message)
        {
            if (message != null && message.Count > 0)
                Message = message;
            else
                Message = null;
            Value = value;
        }

        
        public bool IsOk() => Message == null;


        public IVerboseResult<List<(string, List<string>)>> ToVerboseResult() =>
            new VerboseResult<List<(string, List<string>)>>(Message);

        public List<(string, List<string>)> Message { get; set; }

        public TValue Value { get; set; }
    }
}