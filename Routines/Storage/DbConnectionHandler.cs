using System;
using System.Data.Common;
using System.Threading.Tasks;

namespace DashboardCode.Routines.Storage
{
    public class DbConnectionHandler<TIService, TUserContext> : 
        IRoutineHandler<(TIService service, Action<Action> tran), TUserContext> 
    {
        readonly Func<DbConnection, Action<DbCommand>, TIService> construct;
        readonly Func<DbConnection> createConnection;
        readonly RoutineClosure<TUserContext> closure;

        public DbConnectionHandler(
            Func<DbConnection, Action<DbCommand>, TIService> construct,
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
            void tran(Action a)
            {
                transaction = connection.BeginTransaction();
                a();
                transaction.Commit();
                transaction = null;
            }
            void set(DbCommand c)
            {
                if (transaction != null)
                    c.Transaction = transaction;
            }
            return (set, tran);

        }

        // -------------------- -------------------- -------------------- -------------------- -------------------- --------------------
        public void Handle(Action<(TIService service, Action<Action> tran)> action)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (set,transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                action((service, transact));
            }
        }

        public TOutput Handle<TOutput>(Func<(TIService service, Action<Action> tran), TOutput> func)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (set, transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                return func((service, transact));
            }
        }

        public void Handle(Action<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>> action)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (set, transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                action((service, transact), closure);
            }
        }

        public TOutput Handle<TOutput>(Func<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>, TOutput> func)
        {
            using (var connection = createConnection())
            {
                connection.Open();
                var (set, transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                return func((service, transact), closure);
            }
        }

        // -------------------- -------------------- -------------------- -------------------- -------------------- --------------------
        public async Task<TOutput> HandleAsync<TOutput>(Func<(TIService service, Action<Action> tran), Task<TOutput>> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (set, transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                var output = await func((service, transact));
                return output;
            }
        }

        public async Task HandleAsync(Func<(TIService service, Action<Action> tran), Task> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (set, transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                await func((service, transact));
            }
        }

        public async Task<TOutput> HandleAsync<TOutput>(Func<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>, Task<TOutput>> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (set, transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                var output = await func((service, transact), closure);
                return output;
            }
        }

        public async Task HandleAsync(Func<(TIService service, Action<Action> tran), RoutineClosure<TUserContext>, Task> func)
        {
            using (var connection = createConnection())
            {
                await connection.OpenAsync();
                var (set, transact) = ComposeTransact(connection);
                var service = construct(connection, set);
                await func((service, transact), closure);
            }
        }
    }
}
