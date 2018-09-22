using System;
using System.Data.SqlClient;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage.SqlServer
{
    public class SqlConnectionHandler<TIService, TUserContext,  TService> : IDisposable, 
        IRoutineHandler<(TIService,Action<Action>), TUserContext> 
        where TService : TIService
    {
        readonly Func<SqlConnection, SqlTransaction, TService> construct;
        readonly Func<SqlConnection> createConnection;
        readonly RoutineClosure<TUserContext> closure;

        public SqlConnectionHandler(
            Func<SqlConnection, SqlTransaction, TService> construct,
            Func<SqlConnection> createConnection,
            RoutineClosure<TUserContext> closure)
        {
            this.construct = construct;
            this.createConnection = createConnection;
            this.closure = closure;
        }

        public void Dispose()
        {
            //nothing to dispose; every system resource controlled with using block
        }

        private static Action<Action> ComposeTransact(SqlConnection  connection)
        {
            return a =>
            {
                var tran = connection.BeginTransaction();
                a();
                tran.Commit();
            };
        }

        public void Handle(Action<(TIService, Action<Action>)> action)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                action((service, transact));
            }
        }

        public TOutput Handle<TOutput>(Func<(TIService, Action<Action>), TOutput> func)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                return func((service, transact));
            }
        }

        public void Handle(Action<(TIService, Action<Action>), RoutineClosure<TUserContext>> action)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                action((service, transact), closure);
            }
        }

        public TOutput Handle<TOutput>(Func<(TIService, Action<Action>), RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                return func((service, transact), closure);
            }
        }

        // -------------------- -------------------- -------------------- -------------------- -------------------- --------------------
        public async Task<TOutput> HandleAsync<TOutput>(Func<(TIService, Action<Action>), Task<TOutput>> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                var output = await func((service, transact));
                return output;
            }
        }

        public async Task HandleAsync(Func<(TIService, Action<Action>), Task> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                await func((service, transact));
            }
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<(TIService, Action<Action>), RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                var output = await func((service, transact), closure);
                return output;
            }
        }

        public async Task HandleAsync(Func<(TIService, Action<Action>), RoutineClosure<TUserContext>, Task> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var service = construct(connection, null);
                var transact = ComposeTransact(connection);
                await func((service, transact), closure);
            }
        }
    }
}
