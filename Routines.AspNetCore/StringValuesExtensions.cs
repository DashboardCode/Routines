using System;
using Microsoft.Extensions.Primitives; // StringValues - type that is not included to standard framework

namespace DashboardCode.Routines.AspNetCore
{
    public static class StringValuesExtensions
    {
        public static ConvertResult<string> ConvertToString(this StringValues stringValues)
        {
            return new ConvertResult<string> { Value = stringValues.ToString() };
        }

        public static ConvertResult<int> ConvertToInt(this StringValues stringValues)
        {
            var str = stringValues.ToString();
            if (int.TryParse(str, out int number))
                return new ConvertResult<int> { Value = number };
            return new ConvertResult<int>( new[] { "Not number!" });
        }

        // used in samples
        public static BinderResult TryStringValidateLength(this StringValues stringValues, Action<string> setter, int length)
        {
            var v = stringValues.ToString();
            setter(v);
            return new BinderResult(v.Length > length ? new[] { "Too long!" } : null);
        }
    }
}
