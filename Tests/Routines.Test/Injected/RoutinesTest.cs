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

            var factory = new BasicRoutineTransientsFactory<StateService>(
                loggingTransients.ShouldBufferVerbose,
                loggingTransients.ShouldVerboseWithStackTrace,
                new RoutineLogger(routineGuid),
                loggingTransients.BasicRoutineLoggingAdapter
            );

            var routineLogging = factory.Create();
            var exceptionHandler = new ExceptionHandler(new ExceptionAdapter(
                loggingTransients.BasicRoutineLoggingAdapter.LogException,
                loggingTransients.TransformException
            ), null/*monitorRoutineDurationTicks*/);

            var closure = new StateService(routineGuid.CorrelationToken, loggingTransients.BasicRoutineLoggingAdapter.LogVerbose); 
            string result = null;


            var routine = new RoutineHandler<StateService>(exceptionHandler, routineLogging,  closure, new { });
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

        //public class TestOrmHandlerFactory //: IOrmHandlerFactory<UserContext>{
        //    public IOrmHandler<TEntity> CreateOrmHandler<TEntity>(RoutineClosure<UserContext> state) where TEntity : class
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        //public class TestRepositoryHandlerFactory //: IRepositoryHandlerFactory<UserContext>
        //{
        //    public IRepositoryHandler<TEntity> CreateRespositoryHandler<TEntity>(RoutineClosure<UserContext> state) where TEntity : class
        //    {
        //        throw new NotImplementedException();
        //    }
        //}

        [TestMethod]
        public void RoutinesInjectedHandle2()
        {
            var correlationToken = Guid.NewGuid();
            var log = new List<string>();
            var routineGuid = new RoutineGuid(correlationToken, "RoutinesTest", "RoutinesInjectedHandle2");
            var loggingTransients = new LoggingTransients(routineGuid, log);
            bool shouldBufferVerbose = loggingTransients.ShouldBufferVerbose;
            bool shouldVerboseWithStackTrace = loggingTransients.ShouldVerboseWithStackTrace;

            
            IOrmHandlerGFactory<UserContext> testOrmHandlerFactory = null; // new TestOrmHandlerFactory();
            IRepositoryHandlerGFactory<UserContext> testRepositoryHandlerFactory = null; // new TestRepositoryHandlerFactory();
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };
            Func<Action<DateTime, string>, RoutineClosure<UserContext>> createRoutineState =
                (verbose)=>new RoutineClosure<UserContext>(userContext, routineGuid, verbose, null);
            string result = null;
            var routineLogger = new RoutineLogger(routineGuid);
            var factory = new BasicRoutineTransientsFactory<RoutineClosure<UserContext>>(
               loggingTransients.ShouldBufferVerbose,
               loggingTransients.ShouldVerboseWithStackTrace,
               routineLogger,
               loggingTransients.BasicRoutineLoggingAdapter
            );
            var routineLogging = factory.Create();
            var exceptionHandler = new ExceptionHandler(new ExceptionAdapter(
                loggingTransients.BasicRoutineLoggingAdapter.LogException,
                loggingTransients.TransformException
            ), null/*monitorRoutineDurationTicks*/);
            var closure = new RoutineClosure<UserContext>(userContext, routineGuid, loggingTransients.BasicRoutineLoggingAdapter.LogVerbose, null);

            var routine = new UserRoutineHandler<UserContext>(
                exceptionHandler, routineLogging, closure,
                testRepositoryHandlerFactory,
                testOrmHandlerFactory,
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
            var routineLogger = new RoutineLogger(routineGuid);
            var factory = new BasicRoutineTransientsFactory<StateService>(
                loggingTransients.ShouldBufferVerbose,
                loggingTransients.ShouldVerboseWithStackTrace,
                routineLogger,
                loggingTransients.BasicRoutineLoggingAdapter
            );
            var routineLogging = factory.Create();
            var exceptionHandler = new ExceptionHandler(new ExceptionAdapter(
                loggingTransients.BasicRoutineLoggingAdapter.LogException,
                loggingTransients.TransformException
            ), null/*monitorRoutineDurationTicks*/);
            var closure = new StateService(routineGuid.CorrelationToken, loggingTransients.BasicRoutineLoggingAdapter.LogVerbose);
            try
            {
                var routine = new RoutineHandler<StateService>(exceptionHandler, routineLogging, closure, new { });

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
            var routineGuid = new RoutineGuid(Guid.NewGuid(), "RoutinesTest", "RoutinesInjectedExceptionHandle2");
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };

            var log = new List<string>();
            var loggingTransients = new LoggingTransients(routineGuid, log);
            IOrmHandlerGFactory<UserContext> testOrmHandlerFactory = null; // new TestOrmHandlerFactory();
            IRepositoryHandlerGFactory<UserContext> testRepositoryHandlerFactory = null; // new TestRepositoryHandlerFactory();
            //var testRepositoryHandlerFactory = new TestRepositoryHandlerFactory(); // stub
            //var testOrmHandlerFactory = new TestOrmHandlerFactory(); // stub
            Func<Action<DateTime, string>, RoutineClosure<UserContext>> createRoutineState =
                (verbose) => new RoutineClosure<UserContext>(userContext, routineGuid, null/*no verbose logging for this routineGuid */, null); 
            try
            {
                var routineLogger = new RoutineLogger(routineGuid);
                var factory = new BasicRoutineTransientsFactory<RoutineClosure<UserContext>>(
                    loggingTransients.ShouldBufferVerbose,
                    loggingTransients.ShouldVerboseWithStackTrace,
                    routineLogger,
                    loggingTransients.BasicRoutineLoggingAdapter
                );
                var routineLogging = factory.Create();

                var exceptionHandler = new ExceptionHandler(new ExceptionAdapter(
                    loggingTransients.BasicRoutineLoggingAdapter.LogException,
                    loggingTransients.TransformException
                ), null/*monitorRoutineDurationTicks*/);


                var closure = new RoutineClosure<UserContext>(userContext, routineGuid, null/*no verbose logging for this routineGuid */, null);
                var routine = new UserRoutineHandler<UserContext>(
                    exceptionHandler, routineLogging, closure,
                    testRepositoryHandlerFactory,
                    testOrmHandlerFactory,
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
