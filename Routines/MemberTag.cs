using System;
using System.Runtime.CompilerServices;

namespace DashboardCode.Routines
{
    public struct MemberTag
    {
        public readonly string Namespace;
        public readonly string Type;
        public readonly string Member;

        public MemberTag(object o, [CallerMemberName] string member = null) : this(o.GetType(), member)
        {
        }
        public MemberTag(Type type, [CallerMemberName] string member = null):this(type.Namespace, type.Name, member)
        {
        }
        public MemberTag(string Type, string Member): this(null, Type, Member)
        {
        }
        public MemberTag(string Namespace, string Type, string Member)
        {
            this.Namespace = Namespace;
            this.Type = Type;
            this.Member = Member;
        }
    }
}