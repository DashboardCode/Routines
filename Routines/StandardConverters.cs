using System;

namespace DashboardCode.Routines
{
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

        public static Func<string, ValuableResult<T>> GerConverter<T>()
        {
            if (typeof(T) == typeof(int))
            {
                Func<string, ValuableResult<int>> d = (s) => TryParseInt(s);
                return (Func<string, ValuableResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(string))
            {
                Func<string, ValuableResult<string>> d = (s) => TryParseString(s);
                return (Func<string, ValuableResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(Guid))
            {
                Func<string, ValuableResult<Guid>> d = (s) => TryParseGuid(s);
                return (Func<string, ValuableResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(long))
            {
                Func<string, ValuableResult<long>> d = (s) => TryParseLong(s);
                return (Func<string, ValuableResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(byte))
            {
                Func<string, ValuableResult<byte>> d = (s) => TryParseByte(s);
                return (Func<string, ValuableResult<T>>)(Delegate)d;
            }
            else if (typeof(T) == typeof(short))
            {
                Func<string, ValuableResult<short>> d = (s) => TryParseShort(s);
                return (Func<string, ValuableResult<T>>)(Delegate)d;
            }

            throw new NotImplementedException($"Type '{typeof(T).FullName}' is not supported by '{nameof(Converters)}'.'{nameof(GerConverter)}' method");
        }

        public static ValuableResult<int> TryParseInt(string s)
        {
            var b = int.TryParse(s, out int id);
            return new ValuableResult<int>(id, b);
        }
        public static ValuableResult<string> TryParseString(string s)
        {
            return new ValuableResult<string>(s, true);
        }
        public static ValuableResult<long> TryParseLong(string s)
        {
            var b = long.TryParse(s, out long id);
            return new ValuableResult<long>(id, b);
        }
        public static ValuableResult<byte> TryParseByte(string s)
        {
            var b = byte.TryParse(s, out byte id);
            return new ValuableResult<byte>(id, b);
        }
        public static ValuableResult<short> TryParseShort(string s)
        {
            var b = short.TryParse(s, out short id);
            return new ValuableResult<short>(id, b);
        }
        public static ValuableResult<Guid> TryParseGuid(string s)
        {
            var b = Guid.TryParse(s, out Guid id);
            return new ValuableResult<Guid>(id, b);
        }
    }
}
