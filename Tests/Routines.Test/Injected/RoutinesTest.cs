using System;
using System.Collections.Generic;
using System.Globalization;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DashboardCode.Routines.Logging;
using DashboardCode.Routines.Storage;

namespace DashboardCode.Routines.Logging.Test
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

            bool testInputOutput(object input2, object output, TimeSpan duration) => true;
            var routineLogger = new RoutineLogger(correlationToken);
            var exceptionHandler = new ExceptionHandler(
                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                loggingTransients.TransformException);
            var input = new { };

            var (routineHandler, closure) = routineLogger.CreateRoutineHandler(
                    loggingTransients.ShouldBufferVerbose,
                    (verbose) => new CustomRoutineClosure(correlationToken, verbose),
                    exceptionHandler,
                    finishActivity: true,
                    input,
                    loggingTransients.BasicRoutineLoggingAdapter,
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d, o),
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
                    memberTag,
                    loggingTransients.BasicRoutineLoggingAdapter.LogVerbose,
                    shouldVerboseWithStackTrace: false,
                    testInputOutput,
                    (ticks) => {}
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

            bool testInputOutput(object input2, object output, TimeSpan duration) => true;
            var routineLogger = new RoutineLogger(correlationToken);
            var exceptionHandler = new ExceptionHandler(
                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                loggingTransients.TransformException);
            var input = new { };
            

            var (routineHandler, closure) = routineLogger.CreateRoutineHandler(
                    loggingTransients.ShouldBufferVerbose,
                    (verbose) => new RoutineClosure<UserContext>(userContext, verbose, null),
                    exceptionHandler,
                    finishActivity: true,
                    input,
                    loggingTransients.BasicRoutineLoggingAdapter,
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d, o),
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
                    memberTag,
                    loggingTransients.BasicRoutineLoggingAdapter.LogVerbose,
                    shouldVerboseWithStackTrace: false,
                    testInputOutput,
                    (ticks) => { }
            );

            var userRoutineHandler = new UserRoutineHandler<UserContext>(
                testRepositoryHandlerFactory,
                testOrmHandlerFactory,
                routineHandler);
            string result = null;
            userRoutineHandler.Handle(c =>
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

            bool testInputOutput(object input2, object output, TimeSpan duration) => true;
            var routineLogger = new RoutineLogger(correlationToken);
            var exceptionHandler = new ExceptionHandler(
                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                loggingTransients.TransformException
            );
            var input = new { };
            var (routineHandler, closure) = routineLogger.CreateRoutineHandler(
                    loggingTransients.ShouldBufferVerbose,
                    (verbose) => new CustomRoutineClosure(correlationToken, verbose),
                    exceptionHandler,
                    finishActivity: true,
                    input,
                    loggingTransients.BasicRoutineLoggingAdapter,
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d, o),
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
                    memberTag,
                    loggingTransients.BasicRoutineLoggingAdapter.LogVerbose,
                    shouldVerboseWithStackTrace: false,
                    testInputOutput,
                    (ticks) => { }
            );

            try
            {
                routineHandler.Handle(c =>
                {
                    c.Verbose("- test verbose");
                    throw new ApplicationException("failure");
                }
                );
            }
            catch (ApplicationException ex)
            {
                if (ex.Message != "failure")
                    throw new ApplicationException("handle logging not working properly");
                if (log.Count != 5) /* start + exception + finish + input + test verbose */
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
            var input = new { };
            try
            {
                bool testInputOutput(object input2, object output, TimeSpan duration) => true;
                var routineLogger = new RoutineLogger(correlationToken);
                var exceptionHandler = new ExceptionHandler(
                    ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
                    loggingTransients.TransformException
                );

                var (routineHandler, closure) = routineLogger.CreateRoutineHandler(
                    loggingTransients.ShouldBufferVerbose,
                    (verbose) => new RoutineClosure<UserContext>(userContext, null /*no verbose logger for this test*/, null),
                    exceptionHandler,
                    finishActivity: true,
                    input,
                    loggingTransients.BasicRoutineLoggingAdapter,
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d, o),
                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
                    memberTag,
                    loggingTransients.BasicRoutineLoggingAdapter.LogVerbose,
                    shouldVerboseWithStackTrace: false,
                    testInputOutput,
                    (ticks) => { }
            );

                var userRoutineHandler = new UserRoutineHandler<UserContext>(
                    testRepositoryHandlerFactory,
                    testOrmHandlerFactory,
                    routineHandler
                );

                routineHandler.Handle((state) =>
                {
                    if (state.Verbose != null)
                        throw new InvalidOperationException("failure");
                    throw new ApplicationException("failure");
                }
                );
            }
            catch (ApplicationException ex)
            {
                if (ex.Message != "failure")
                    throw new ApplicationException("handle logging not working properly");
                if (log.Count != 4) /* start + exception + finish + input  */
                    throw new ApplicationException("handle logging not working properly");
            }
        }
    }
}
