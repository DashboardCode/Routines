using System;
using OdataToEntity.EfCore;
using DashboardCode.Routines.Storage.EfCore;

namespace DashboardCode.AdminkaV1.LoggingDom.DataAccessEfCore.OdataToEntity
{
    public class AdminkaOeEfCoreDataAdapter : OeEfCoreDataAdapter<LoggingDomDbContext>
    {
        readonly IDbContextOptionsFactory optionsFactory;
        readonly Action<string> verbose;
        public AdminkaOeEfCoreDataAdapter(IDbContextOptionsFactory optionsFactory, Action<string> verbose)
        {
            this.optionsFactory = optionsFactory;
            this.verbose = verbose;
        }

        public override object CreateDataContext()
        {
            return new LoggingDomDbContext((b) => optionsFactory.Create(b), verbose);
        }
    }
}
