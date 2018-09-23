using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class DbConnectionHandler<TIService, TUserContext,  TService> : 
        IRoutineHandler<(TIService service, Action<Action> tran), TUserContext> 
        where TService : TIService
    {
        readonly Func<DbConnection, Action<DbCommand>, TService> construct;
        readonly Func<DbConnection> createConnection;
        readonly RoutineClosure<TUserContext> closure;

        public DbConnectionHandler(
            Func<DbConnection, Action<DbCommand>, TService> construct,
            Func<DbConnection> createConnection,
            RoutineClosure<TUserContext> closure)
        {
            this.construct = construct;
            this.createConnection = createConnection;
            this.closure = closure;
        }

        private static (Action<DbCommand>, Action<Action>) ComposeTransact(DbConnection  connection)
        {
            DbTransaction transaction = null;
            Action<Action> tran = a =>
            {
                transaction = connection.BeginTransaction();
                a();
                transaction.Commit();
                transaction = null;
            };
            Action<DbCommand> set = (c) =>
            {
                if (transaction != null)
                    c.Transaction = transaction;
            };
            return (set, tran);

        }

        public void Handle(Action<(TIService service, Action<Action> tran)> action)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (get,transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                action((service, transact));
            }
        }

        public TOutput Handle<TOutput>(Func<(TIService service, Action<Action> tran), TOutput> func)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (get, transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                return func((service, transact));
            }
        }

        public void Handle(Action<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>> action)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (get, transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                action((service, transact), closure);
            }
        }

        public TOutput Handle<TOutput>(Func<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (get, transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                return func((service, transact), closure);
            }
        }

        // -------------------- -------------------- -------------------- -------------------- -------------------- --------------------
        public async Task<TOutput> HandleAsync<TOutput>(Func<(TIService service, Action<Action> tran), Task<TOutput>> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (get, transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                var output = await func((service, transact));
                return output;
            }
        }

        public async Task HandleAsync(Func<(TIService service, Action<Action> tran), Task> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (get, transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                await func((service, transact));
            }
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (get, transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                
                var output = await func((service, transact), closure);
                return output;
            }
        }

        public async Task HandleAsync(Func<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>, Task> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (get, transact) = ComposeTransact(connection);
                var service = construct(connection, get);
                await func((service, transact), closure);
            }
        }
    }
}
