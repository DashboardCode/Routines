using System;
using System.Collections.Concurrent;

namespace DashboardCode.Routines.Storage.EfCore
{
    public class StatefullLoggerFactoryPool
    {
        public static readonly StatefullLoggerFactoryPool Instance = new StatefullLoggerFactoryPool(()=> new StatefullLoggerFactory());
        private readonly Func<StatefullLoggerFactory> construct;
        private readonly ConcurrentBag<StatefullLoggerFactory> bag = new ConcurrentBag<StatefullLoggerFactory>();

        private StatefullLoggerFactoryPool(Func<StatefullLoggerFactory> construct) =>
            this.construct = construct;

        public StatefullLoggerFactory Get(Action<string> verbose, LoggerProviderConfiguration loggerProviderConfiguration)
        {
            if (!bag.TryTake(out StatefullLoggerFactory statefullLoggerFactory))
                statefullLoggerFactory = construct();
            statefullLoggerFactory.LoggerProvider.Set(verbose, loggerProviderConfiguration);
            return statefullLoggerFactory;
        }

        public void Return(StatefullLoggerFactory statefullLoggerFactory)
        {
            statefullLoggerFactory.LoggerProvider.Set(null, null);
            bag.Add(statefullLoggerFactory);
        }
    }
}