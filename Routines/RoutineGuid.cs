using System;
using System.Diagnostics;
using System.Runtime.CompilerServices;

namespace DashboardCode.Routines
{
    [DebuggerDisplay("{MemberTag.Namespace} {MemberTag.Type} {MemberTag.Member} {CorrelationToken}")]
    public struct RoutineGuid
    {
        public readonly Guid CorrelationToken;
        public readonly MemberTag MemberTag;

        public RoutineGuid(Guid correlationToken, object o, [CallerMemberName] string member = null) :
           this(correlationToken, o.GetType(), member)
        {
        }
        public RoutineGuid(object o, [CallerMemberName] string member = null) :
            this(Guid.NewGuid(), o.GetType(), member)
        {
        }
        public RoutineGuid(Guid correlationToken, Type type, [CallerMemberName] string member = null) :
            this(correlationToken, type.Namespace, type.Name, member)
        {
        }
        public RoutineGuid(Guid correlationToken, string type, string member) :
            this(correlationToken, null, type, member)
        {
        }
        public RoutineGuid(Guid correlationToken, string @namespace, string type, string member):
            this(correlationToken, new MemberTag(@namespace, type, member))
        {
        }
        public RoutineGuid(MemberTag memberTag): this(Guid.NewGuid(), memberTag)
        {
        }
        public RoutineGuid(Guid correlationToken, MemberTag memberTag)
        {
            CorrelationToken = correlationToken;
            MemberTag = memberTag;
        }
    }

    public static class RoutineGuidExtensions
    {
        public static string ToText(this RoutineGuid routineGuid) =>
            routineGuid.CorrelationToken + " " + routineGuid.GetCategory();

        public static string GetCategory(this RoutineGuid routineGuid) =>
            (routineGuid.MemberTag.Namespace != null ? routineGuid.MemberTag.Namespace + "." : "") 
                + routineGuid.MemberTag.Type + "." + routineGuid.MemberTag.Member;
    }
}