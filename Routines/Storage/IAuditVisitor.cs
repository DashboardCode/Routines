namespace DashboardCode.Routines.Storage
{
    public interface IAuditVisitor
    {
        bool HasAuditProperties(object o);
        void SetAuditProperties(object o);
    }
}
