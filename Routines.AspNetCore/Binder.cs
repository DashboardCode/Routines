using System;
using Microsoft.Extensions.Primitives;

namespace DashboardCode.Routines.AspNetCore
{
    public static class Binder
    {
        public static Func<StringValues, ConvertResult<string>> ConvertToString
        {
            get
            {
                return (stringValues) =>
                {
                    return new ConvertResult<string> { Value = stringValues.ToString() };
                };
            }
        }
        
        public static Func<StringValues, ConvertResult<int>> ConvertToInt
        {
            get
            {
                return (stringValues) =>
                {
                    var str = stringValues.ToString();
                    if (int.TryParse(str, out int number))
                        return new ConvertResult<int> { Value = number };
                    return new ConvertResult<int>( new[] { "Not number!" });
                };
            }
        }

        // used in samples
        public static BinderResult TryStringValidateLength(StringValues stringValues, Action<string> setter, int length)
        {
            var v = stringValues.ToString();
            setter(v);
            return new BinderResult(v.Length > length ? new[] { "Too long!" } : null);
        }
    }
}
