using System;
using System.Runtime.CompilerServices;

namespace Vse.Routines
{
    public class RoutineTag
    {
        public RoutineTag(Guid correlationToken, object o, [CallerMemberName] string member = null) :
           this(correlationToken, o.GetType(), member)
        {
        }
        public RoutineTag(object o, [CallerMemberName] string member = null) :
            this(Guid.NewGuid(), o.GetType(), member)
        {
        }

        public RoutineTag(Guid correlationToken, Type type, [CallerMemberName] string member = null) :
            this(correlationToken, type.Namespace, type.Name, member)
        {
        }
        public RoutineTag(Guid correlationToken, string @namespace, string @class, string member)
        {
            CorrelationToken = correlationToken;
            Namespace = @namespace;
            Class = @class;
            Member = member;
        }
        public RoutineTag(Guid correlationToken, string @class, string member)
        {
            CorrelationToken = correlationToken;
            Class = @class;
            Member = member;
        }
        public Guid CorrelationToken { get; private set; }
        public string Namespace { get; private set; }
        public string Class { get; private set; }
        public string Member { get; private set; }

        public string GetCategory()
        {
            var value = (Namespace != null ? Namespace + "." : "")
                + Class + "." + Member;
            return value;
        }

        public override string ToString()
        {
            return CorrelationToken+" "+GetCategory();
        }
    }
}
