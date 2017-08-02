using System;
using System.Runtime.CompilerServices;

namespace DashboardCode.Routines
{
    public class RoutineGuid
    {
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
        public RoutineGuid(Guid correlationToken, string @namespace, string type, string member)
        {
            CorrelationToken = correlationToken;
            Namespace = @namespace;
            Type = type;
            Member = member;
        }
        public RoutineGuid(Guid correlationToken, string type, string member)
        {
            CorrelationToken = correlationToken;
            Type = type;
            Member = member;
        }
        public Guid   CorrelationToken { get; private set; }
        public string Namespace        { get; private set; }
        public string Type             { get; private set; }
        public string Member           { get; private set; }

        public string GetCategory()
        {
            var value = (Namespace != null ? Namespace + "." : "") + Type + "." + Member;
            return value;
        }

        public override string ToString()
        {
            return CorrelationToken + " " + GetCategory();
        }
    }
}
