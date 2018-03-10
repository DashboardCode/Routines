using System;

namespace DashboardCode.Routines
{
    public static class MemberTagExtensions
    {
        public static string ToText(this MemberTag memberTag, Guid correlationToken) =>
            correlationToken + " " + memberTag.GetCategory();

        public static string GetCategory(this MemberTag memberTag) =>
            (memberTag.Namespace != null ? memberTag.Namespace + "." : "") 
                + memberTag.Type + "." + memberTag.Member;
    }
}