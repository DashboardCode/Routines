using System;
using System.Collections.Generic;

namespace DashboardCode.Routines
{
    public struct ConvertFuncResult<T>
    {
        public T Value;
        private bool success;
        public ConvertFuncResult(T value, bool success)
        {
            Value = value;
            this.success = success;
        }
        public bool IsSuccess() => success;
    }

    public struct ConvertVerboseResult<T>
    {
        public T Value;
        public ConvertVerboseResult(string message)
        {
            Value = default(T);
            BinderResult = new VerboseResult(message);
        }
        public VerboseResult BinderResult;
        public bool IsSuccess() => BinderResult.IsSuccess();
    }

    public struct VerboseResult
    {
        public VerboseResult(List<string> errorMessages)
        {
            if (errorMessages != null && errorMessages.Count > 0)
            {
                ErrorMessages = errorMessages;
            }
            else
            {
                ErrorMessages = null;
            }
        }

        public VerboseResult(string message = null)
        {
            if (message != null)
            {
                ErrorMessages = new List<string>() { message };
            }
            else
            {
                ErrorMessages = null;
            }
        }
        public List<string> ErrorMessages;
        public bool IsSuccess() { return ErrorMessages == null; }
    }

    public static class Converters
    {
        public static Func<string, T> GetParser<T>()
        {
            if (typeof(T) == typeof(int))
            {
                Func<string, int> d = (s) => int.Parse(s);
                return (Func<string, T>)(Delegate)d;
            }
            else if (typeof(T) == typeof(string))
            {
                Func<string, string> d = (s) => s;
                return (Func<string, T>)(Delegate)d;
            }
            else if (typeof(T) == typeof(Guid))
            {
                Func<string, Guid> d = (s) => Guid.Parse(s);
                return (Func<string, T>)(Delegate)d;
            }
            else if (typeof(T) == typeof(long))
            {
                Func<string, long> d = (s) => long.Parse(s);
                return (Func<string, T>)(Delegate)d;
            }
            else if (typeof(T) == typeof(byte))
            {
                Func<string, byte> d = (s) => byte.Parse(s);
                return (Func<string, T>)(Delegate)d;
            }
            else if (typeof(T) == typeof(short))
            {
                Func<string, short> d = (s) => short.Parse(s);
                return (Func<string, T>)(Delegate)d;
            }
            throw new NotImplementedException($"Type '{typeof(T).FullName}' is not supported by '{nameof(Converters)}'.'{nameof(GetParser)}' method");
        }


        public static Func<string, ConvertFuncResult<T>> GerConverter<T>()
        {
            if (typeof(T) == typeof(int))
            {
                Func<string, ConvertFuncResult<int>> d = (s) => TryParseInt(s);
                return (Func<string, ConvertFuncResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(string))
            {
                Func<string, ConvertFuncResult<string>> d = (s) => TryParseString(s);
                return (Func<string, ConvertFuncResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(Guid))
            {
                Func<string, ConvertFuncResult<Guid>> d = (s) => TryParseGuid(s);
                return (Func<string, ConvertFuncResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(long))
            {
                Func<string, ConvertFuncResult<long>> d = (s) => TryParseLong(s);
                return (Func<string, ConvertFuncResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(byte))
            {
                Func<string, ConvertFuncResult<byte>> d = (s) => TryParseByte(s);
                return (Func<string, ConvertFuncResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(short))
            {
                Func<string, ConvertFuncResult<short>> d = (s) => TryParseShort(s);
                return (Func<string, ConvertFuncResult<T>>)(Delegate)d;
            }

            throw new NotImplementedException($"Type '{typeof(T).FullName}' is not supported by '{nameof(Converters)}'.'{nameof(GerConverter)}' method");
        }

        public static ConvertFuncResult<int> TryParseInt(string s)
        {
            var b = int.TryParse(s, out int id);
            return new ConvertFuncResult<int>(id, b);
        }
        public static ConvertFuncResult<string> TryParseString(string s)
        {
            return new ConvertFuncResult<string>(s, true);
        }
        public static ConvertFuncResult<long> TryParseLong(string s)
        {
            var b = long.TryParse(s, out long id);
            return new ConvertFuncResult<long>(id, b);
        }
        public static ConvertFuncResult<byte> TryParseByte(string s)
        {
            var b = byte.TryParse(s, out byte id);
            return new ConvertFuncResult<byte>(id, b);
        }
        public static ConvertFuncResult<short> TryParseShort(string s)
        {
            var b = short.TryParse(s, out short id);
            return new ConvertFuncResult<short>(id, b);
        }
        public static ConvertFuncResult<Guid> TryParseGuid(string s)
        {
            var b = Guid.TryParse(s, out Guid id);
            return new ConvertFuncResult<Guid>(id, b);
        }
    }
}