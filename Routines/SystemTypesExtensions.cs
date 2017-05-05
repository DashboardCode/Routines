using System;
using System.Collections.Generic;
using System.Linq;

namespace Vse.Routines
{
    public static class SystemTypesExtensions
    {
        public static readonly IReadOnlyCollection<Type> DefaultSimpleTextTypes = new List<Type>
        {
             typeof(DateTime), typeof(Guid),  typeof(TimeSpan)
            , typeof(DateTime?), typeof(Guid?),  typeof(TimeSpan?), typeof(DateTimeOffset), typeof(DateTimeOffset?)
        };

        public static readonly IReadOnlyCollection<Type> DefaultSimpleNumberTypes = new List<Type>
        {
             typeof(Decimal), typeof(Decimal?)
        };

        public static readonly IReadOnlyCollection<Type> SystemTypes = new List<Type>
        {
                typeof(bool),
                typeof(bool?),
                typeof(byte),
                typeof(byte?),
                typeof(char),
                typeof(char?),
                typeof(decimal),
                typeof(decimal?),
                typeof(double),
                typeof(double?),
                typeof(float),
                typeof(float?),
                typeof(int),
                typeof(int?),
                typeof(long),
                typeof(long?),
                typeof(sbyte),
                typeof(sbyte?),
                typeof(short),
                typeof(short?),
                typeof(uint),
                typeof(uint?),
                typeof(ulong),
                typeof(ulong?),
                typeof(ushort),
                typeof(ushort?),
                typeof(string),
                typeof(DateTime),
                typeof(DateTime?),
                typeof(DateTimeOffset),
                typeof(DateTimeOffset?),
                typeof(Guid),
                typeof(Guid?),
                typeof(TimeSpan),
                typeof(TimeSpan?)
        };

        public static bool Contains(Type type)
        {
            return SystemTypes.Contains(type);
        }
    }
}
