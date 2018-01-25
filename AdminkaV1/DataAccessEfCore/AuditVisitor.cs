using System;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.DataAccessEfCore
{
    public class AuditVisitor : IAuditVisitor
    {
        UserContext userContext;
        public AuditVisitor(UserContext userContext)
        {
            this.userContext = userContext;
        }
        
        public bool HasAuditProperties(object o)
        {
            return o is IVersioned;
        }

        public void SetAuditProperties(object o)
        {
            if (o is IVersioned versionedEntity)
            {
                versionedEntity.RowVersionBy = userContext.AuditStamp;
                versionedEntity.RowVersionAt = DateTime.Now;
            }
        }
    }
}