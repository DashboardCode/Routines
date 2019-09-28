using System;
using Microsoft.EntityFrameworkCore;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class VerboseDbContext : DbContext
    {
        static VerboseDbContext()
        {
            // load EF DbContext dependent assemblies explicitly otherwise other tools can load "older" or "newer" versions and this can become a problem
            //var loadit = new[]
            //{
            //    //typeof(Remotion.Linq.DefaultQueryProvider),
            //    //typeof(System.Collections.Generic.),
            //};
        }

        readonly Action<DbContextOptionsBuilder> buildOptionsBuilder;
        readonly Action<string> verbose;

        /// <summary>
        /// It is not enough to use standard "DbContextOptions options" parameter to inject verbose logger, since we should release verbose logger on Dispose to avoid ef core memory leaks. Therefore parameters that we pass should include the method with which we manipulate DbContextOptionsBuilder in overrided OnConfiguring method . This is how EF Core team understand DI logging.
        /// </summary>
        /// <param name="buildOptionsBuilder"></param>
        /// <param name="verbose"></param>
        public VerboseDbContext(Action<DbContextOptionsBuilder> buildOptionsBuilder, Action<string> verbose = null)
            : base()
        {
            this.buildOptionsBuilder = buildOptionsBuilder;
            this.verbose = verbose;
        }

        private Action returnLoggerFactory;
        protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
        {
            if (verbose != null)
            {
                var loggerFactory = StatefullLoggerFactoryPool.Instance.Get(verbose, new LoggerProviderConfiguration { Enabled = true, CommandBuilderOnly = false });
                returnLoggerFactory = () => StatefullLoggerFactoryPool.Instance.Return(loggerFactory);
                optionsBuilder.UseLoggerFactory(loggerFactory);
            }
            buildOptionsBuilder(optionsBuilder);
        }

        // NOTE: not threadsafe way of disposing
        public override void Dispose()
        {
            returnLoggerFactory?.Invoke();
            returnLoggerFactory = null;
            base.Dispose();
        }
    }
}