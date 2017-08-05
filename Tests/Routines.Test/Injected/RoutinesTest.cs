using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Injected.Test
{
    [TestClass]
    public class RoutinesTest
    {
        [TestMethod]
        public void RoutinesInjectedHandle()
        {
            var routineGuid = new RoutineGuid(Guid.NewGuid(), new MemberTag(this));
            var log = new List<string>();
            var loggingTransients = new LoggingTransients(routineGuid, log);

            var routineContainer = new BasicRoutineTransients<StateService>(
                loggingTransients.BasicRoutineLoggingAdapter,
                loggingTransients.TransformException,
                (verbose) => new StateService(routineGuid.CorrelationToken, verbose)
                );

            string result = null;
            var routine = new RoutineHandler<StateService>(routineContainer, new { });
            routine.Handle((c) =>
                {
                    result = "success";
                }
            );
            if (result != "success")
                throw new ApplicationException("handle not working properly");
            if (log.Count != 4)
                throw new ApplicationException("handle logging not working properly");
        }
        public class TestRepositoryHandlerFactory : IRepositoryHandlerFactory<UserContext>{
            public IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(Routine<UserContext> state) where TEntity : class
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void RoutinesInjectedHandle2()
        {
            var correlationToken = Guid.NewGuid();
            var log = new List<string>();
            var routineGuid = new RoutineGuid(correlationToken, "RoutinesTest", "RoutinesInjectedHandle2");
            var loggingTransients = new LoggingTransients(routineGuid, log);

            var routineTransients = new BasicRoutineTransients<StateService>(
                loggingTransients.BasicRoutineLoggingAdapter,
                loggingTransients.TransformException,
                (verbose) => new StateService(correlationToken, verbose)
                );
            var testRepositoryHandlerFactory = new TestRepositoryHandlerFactory();
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };
            Func<Action<DateTime, string>, Routine<UserContext>> createRoutineState =
                (verbose)=>new Routine<UserContext>(userContext, routineGuid, verbose, null);
            string result = null;
            
            var routine = new UserRoutine<UserContext>(
                loggingTransients.BasicRoutineLoggingAdapter,
                loggingTransients.TransformException,
                createRoutineState, 
                testRepositoryHandlerFactory, 
                new { });
            routine.Handle((c) =>
            {
                if (c.UserContext.CultureInfo != CultureInfo.InvariantCulture)
                {
                    throw new ApplicationException("UserContext is not the passed");
                }
                result = "success";
            }
            );
            if (result != "success")
                throw new ApplicationException("handle not working properly");
            if (log.Count != 4)
                throw new ApplicationException("handle logging not working properly");
        }

        [TestMethod]
        public void RoutinesInjectedExceptionHandle()
        {
            var routineGuid = new RoutineGuid(Guid.NewGuid(), new MemberTag(this));
            var log = new List<string>();
            var loggingTransients = new LoggingTransients(routineGuid, log);
            var routineContainer = new BasicRoutineTransients<StateService>(
                loggingTransients.BasicRoutineLoggingAdapter,
                loggingTransients.TransformException,
                (verbose)=>new StateService(routineGuid.CorrelationToken, verbose)
                );
            try
            {
                var routine = new RoutineHandler<StateService>(routineContainer, new { });

                routine.Handle((c) =>
                {
                    c.Verbose("verbose over custom container");
                    throw new ApplicationException("failure");
                }
                );
            }
            catch (ApplicationException ex)
            {
                if (ex.Message!="failure")
                    throw new ApplicationException("handle logging not working properly");
                if (log.Count != 6)
                    throw new ApplicationException("handle logging not working properly");
            }
        }

        [TestMethod]
        public void RoutinesInjectedExceptionHandle2()
        {
            var tag = new RoutineGuid(Guid.NewGuid(), "RoutinesTest", "RoutinesInjectedExceptionHandle2");
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };

            var log = new List<string>();
            var loggingTransients = new LoggingTransients(tag, log);
            var testRepositoryHandlerFactory = new TestRepositoryHandlerFactory(); // stub
            Func<Action<DateTime, string>, Routine<UserContext>> createRoutineState =
                (verbose) => new Routine<UserContext>(userContext, tag, null/*no verbose logging for this routineGuid */, null); 
            try
            {
                var routine = new UserRoutine<UserContext>(
                    loggingTransients.BasicRoutineLoggingAdapter,
                    loggingTransients.TransformException,
                    createRoutineState,
                    testRepositoryHandlerFactory,
                    new { }
                );

                routine.Handle((state) =>
                {
                    if (state.Verbose!=null)
                        throw new InvalidOperationException("failure");
                    throw new ApplicationException("failure");
                }
                );
            }
            catch (ApplicationException ex)
            {
                if (ex.Message != "failure")
                    throw new ApplicationException("handle logging not working properly");
                if (log.Count != 5)
                    throw new ApplicationException("handle logging not working properly");
            }
        }
    }
}
