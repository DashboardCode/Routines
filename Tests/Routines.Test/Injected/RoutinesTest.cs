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
            var memberTag = new MemberTag(this);
            var correlationToken = Guid.NewGuid();
            var log = new List<string>();
            var loggingTransients = new LoggingTransients(memberTag, log);

            
            Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
            var routineLogger = new RoutineLogger(correlationToken);
            var exceptionHandler = new ExceptionHandler(
                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                loggingTransients.TransformException);
            var input = new { };

            var (routineHandler, closure) = RoutineHandlerManager.CreateRoutineHandler(
                    exceptionHandler,
                    (verbose) => new CustomRoutineClosure(correlationToken, verbose),
                    loggingTransients.ShouldBufferVerbose, finishActivity: true,
                    testInputOutput, ticks => {; }, loggingTransients.BasicRoutineLoggingAdapter,
                    loggingTransients.BasicRoutineLoggingAdapter, input,
                    loggingTransients.BasicRoutineLoggingAdapter, routineLogger.buffer, shouldVerboseWithStackTrace:false,
                    loggingTransients.BasicRoutineLoggingAdapter
                    );

            string result = null;
            routineHandler.Handle(c =>
                {
                    result = "success";
                }
            );
            if (result != "success")
                throw new ApplicationException("handle not working properly");
            if (log.Count != 4)
                throw new ApplicationException("handle logging not working properly");
        }

        [TestMethod]
        public void RoutinesInjectedHandle2()
        {
            var memberTag = new MemberTag("RoutinesTest", "RoutinesInjectedHandle2");
            var correlationToken = Guid.NewGuid();
            var log = new List<string>();
            var loggingTransients = new LoggingTransients(memberTag, log);
            bool shouldBufferVerbose = loggingTransients.ShouldBufferVerbose;
            bool shouldVerboseWithStackTrace = loggingTransients.ShouldVerboseWithStackTrace;
            
            IOrmHandlerGFactory<UserContext> testOrmHandlerFactory = null;
            IRepositoryHandlerGFactory<UserContext> testRepositoryHandlerFactory = null; 
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };
            Func<Action<DateTime, string>, RoutineClosure<UserContext>> createRoutineState =
                (verbose)=>new RoutineClosure<UserContext>(userContext, verbose, null);

            var activityState = new ActivityState(loggingTransients.BasicRoutineLoggingAdapter.LogActivityStart, loggingTransients.BasicRoutineLoggingAdapter.LogActivityFinish, ticks => {; });
            Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
            var routineLogger = new RoutineLogger(correlationToken);
            var exceptionHandler = new ExceptionHandler(
                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                loggingTransients.TransformException);
            var input = new { };

            var (routineHandler, closure) = RoutineHandlerManager.CreateRoutineHandler(
                    exceptionHandler,
                    (verbose) => new RoutineClosure<UserContext>(userContext, verbose, null),
                    loggingTransients.ShouldBufferVerbose, finishActivity: true,
                    testInputOutput, ticks => {; }, loggingTransients.BasicRoutineLoggingAdapter,
                    loggingTransients.BasicRoutineLoggingAdapter, input,
                    loggingTransients.BasicRoutineLoggingAdapter, routineLogger.buffer, shouldVerboseWithStackTrace: false,
                    loggingTransients.BasicRoutineLoggingAdapter
                    );

            var userRoutineHandler = new UserRoutineHandler<UserContext>(
                testRepositoryHandlerFactory,
                testOrmHandlerFactory,
                routineHandler);
            string result = null;
            userRoutineHandler.Handle( c =>
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
            var correlationToken = Guid.NewGuid();
            var memberTag = new MemberTag(this);
            var log = new List<string>();
            var loggingTransients = new LoggingTransients(memberTag, log);

            var activityState = new ActivityState(loggingTransients.BasicRoutineLoggingAdapter.LogActivityStart, loggingTransients.BasicRoutineLoggingAdapter.LogActivityFinish, ticks => {; });
            Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
            var routineLogger = new RoutineLogger(correlationToken);
            var exceptionHandler = new ExceptionHandler(
                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                loggingTransients.TransformException
            );
            var input = new { };
            var (routineHandler, closure) = RoutineHandlerManager.CreateRoutineHandler(
                    exceptionHandler,
                    (verbose) => new CustomRoutineClosure(correlationToken, verbose),
                    loggingTransients.ShouldBufferVerbose, finishActivity: true,
                    testInputOutput, ticks => {; }, loggingTransients.BasicRoutineLoggingAdapter,
                    loggingTransients.BasicRoutineLoggingAdapter, input,
                    loggingTransients.BasicRoutineLoggingAdapter, routineLogger.buffer, shouldVerboseWithStackTrace: false,
                    loggingTransients.BasicRoutineLoggingAdapter
                    );

            try
            {
                
                routineHandler.Handle( c => {
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
            var memberTag = new MemberTag("RoutinesTest", "RoutinesInjectedExceptionHandle2");
            var correlationToken = Guid.NewGuid();
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };

            var log = new List<string>();
            var loggingTransients = new LoggingTransients(memberTag, log);
            IOrmHandlerGFactory<UserContext> testOrmHandlerFactory = null; // new TestOrmHandlerFactory();
            IRepositoryHandlerGFactory<UserContext> testRepositoryHandlerFactory = null; // new TestRepositoryHandlerFactory();
            //var testRepositoryHandlerFactory = new TestRepositoryHandlerFactory(); // stub
            //var testOrmHandlerFactory = new TestOrmHandlerFactory(); // stub
            Func<Action<DateTime, string>, RoutineClosure<UserContext>> createRoutineState =
                (verbose) => new RoutineClosure<UserContext>(userContext,  null/*no verbose logging for this memberTag */, null);
            var input = new { };
            try
            {
                var activityState = new ActivityState(
                    loggingTransients.BasicRoutineLoggingAdapter.LogActivityStart, loggingTransients.BasicRoutineLoggingAdapter.LogActivityFinish, ticks => {; });
                Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
                var routineLogger = new RoutineLogger(correlationToken);
                var exceptionHandler = new ExceptionHandler(
                    ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                    loggingTransients.TransformException
                );

                var (routineHandler, closure) = RoutineHandlerManager.CreateRoutineHandler(
                    exceptionHandler,
                    (verbose) => new RoutineClosure<UserContext>(userContext, null/*no verbose logging for this test */, null),
                    loggingTransients.ShouldBufferVerbose, finishActivity: true,
                    testInputOutput, ticks => {; }, loggingTransients.BasicRoutineLoggingAdapter,
                    loggingTransients.BasicRoutineLoggingAdapter, input,
                    loggingTransients.BasicRoutineLoggingAdapter, routineLogger.buffer, shouldVerboseWithStackTrace: false,
                    loggingTransients.BasicRoutineLoggingAdapter
                    );

                //IActivityGFactory activityGFactory;
                //if (!loggingTransients.ShouldBufferVerbose)
                //{
                //    activityGFactory = new RoutineLogging(activityState, loggingTransients.BasicRoutineLoggingAdapter);
                //}
                //else
                //{
                //    var buffer =
                //        new BufferedVerboseLogging(
                //            routineLogger.buffer,
                //            loggingTransients.BasicRoutineLoggingAdapter,
                //            loggingTransients.BasicRoutineLoggingAdapter.LogBufferedVerbose,
                //            loggingTransients.ShouldVerboseWithStackTrace
                //        );
                //    activityGFactory =
                //     new BufferedRoutineLogging(
                //        activityState,
                //        buffer,
                //        buffer.LogVerbose,
                //        buffer.Flash,
                //        testInputOutput);
                //}


                //var closure = new RoutineClosure<UserContext>(userContext, null/*no verbose logging for this memberTag */, null);

                //var logOnStart = activityGFactory.Compose(input);

                //var routineHandler = new RoutineHandler<RoutineClosure<UserContext>>(closure, exceptionHandler, logOnStart);

                var userRoutineHandler = new UserRoutineHandler<UserContext>(
                    testRepositoryHandlerFactory,
                    testOrmHandlerFactory,
                    routineHandler
                );

                routineHandler.Handle((state) =>
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
