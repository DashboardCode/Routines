using System;

namespace DashboardCode.Routines.Storage
{
    public interface IAuditVisitor
    {
        bool HasAuditProperties(object o);
        void SetAuditProperties(object o);
    }

    public class AuditVisitor<T> : IAuditVisitor
    {
        readonly Action<T> setAudited;
        

        public AuditVisitor(Action<T> setAudited)
        {
            this.setAudited = setAudited;
        }

        public bool HasAuditProperties(object o) =>
            o is T;

        public void SetAuditProperties(object o)
        {
            if (o is T auditableEntity)
            {
                setAudited(auditableEntity);
            }
        }
    }

    public class NoAuditVisitor : IAuditVisitor
    {
        public static NoAuditVisitor Singleton = new NoAuditVisitor();

        private NoAuditVisitor()
        {

        }
        public bool HasAuditProperties(object o)
        {
            return false;
        }

        public void SetAuditProperties(object o)
        {
        }
    }
}
