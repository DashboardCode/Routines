using System;
using Microsoft.Extensions.Primitives;

namespace DashboardCode.Routines.AspNetCore
{
    public static class Binder
    {
        public static Func<StringValues, ConvertVerboseResult<string>> ConvertToString
        {
            get
            {
                return (stringValues) =>
                {
                    return new ConvertVerboseResult<string> { Value = stringValues.ToString() };
                };
            }
        }
        
        public static Func<StringValues, ConvertVerboseResult<int>> ConvertToInt
        {
            get
            {
                return (stringValues) =>
                {
                    var str = stringValues.ToString();
                    if (int.TryParse(str, out int number))
                        return new ConvertVerboseResult<int> { Value = number };
                    return new ConvertVerboseResult<int>("Not number!");
                };
            }
        }

        // used in samples
        public static VerboseResult TryStringValidateLength(StringValues stringValues, Action<string> setter, int length)
        {
            var v = stringValues.ToString();
            setter(v);
            return new VerboseResult(v.Length > length ? "Too long!" : null);
        }
    }
}
