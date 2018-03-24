//using System;
//using System.Collections.Generic;
//using System.Globalization;
//using Microsoft.VisualStudio.TestTools.UnitTesting;

//using DashboardCode.Routines.Storage;

//namespace DashboardCode.Routines.Injected.Test
//{
//    [TestClass]
//    public class RoutinesTest
//    {
//        [TestMethod]
//        public void RoutinesInjectedHandle()
//        {
//            var memberTag = new MemberTag(this);
//            var correlationToken = Guid.NewGuid();
//            var log = new List<string>();
//            var loggingTransients = new LoggingTransients(memberTag, log);

            
//            Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
//            var routineLogger = new RoutineLogger(correlationToken);
//            var exceptionHandler = new ExceptionHandler(
//                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
//                loggingTransients.TransformException);
//            var input = new { };

//            var routineHandlerFactory = new RoutineHandlerFactory<CustomRoutineClosure>( ticks => {; });
//            var (routineHandler, closure) = routineHandlerFactory.CreateRoutineHandler(
//                    loggingTransients.ShouldBufferVerbose,
//                    (verbose) => new CustomRoutineClosure(correlationToken, verbose),
//                    exceptionHandler,
//                    finishActivity: true,
//                    input,
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d, o),
//                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    memberTag,
//                    routineLogger.buffer, 
//                    shouldVerboseWithStackTrace: false,
//                    testInputOutput
//                    );

//            string result = null;
//            routineHandler.Handle(c =>
//                {
//                    result = "success";
//                }
//            );
//            if (result != "success")
//                throw new ApplicationException("handle not working properly");
//            if (log.Count != 4)
//                throw new ApplicationException("handle logging not working properly");
//        }

//        [TestMethod]
//        public void RoutinesInjectedHandle2()
//        {
//            var memberTag = new MemberTag("RoutinesTest", "RoutinesInjectedHandle2");
//            var correlationToken = Guid.NewGuid();
//            var log = new List<string>();
//            var loggingTransients = new LoggingTransients(memberTag, log);
//            bool shouldBufferVerbose = loggingTransients.ShouldBufferVerbose;
//            bool shouldVerboseWithStackTrace = loggingTransients.ShouldVerboseWithStackTrace;
            
//            IOrmHandlerGFactory<UserContext> testOrmHandlerFactory = null;
//            IRepositoryHandlerGFactory<UserContext> testRepositoryHandlerFactory = null; 
//            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };
//            Func<Action<DateTime, string>, RoutineClosure<UserContext>> createRoutineState =
//                (verbose)=>new RoutineClosure<UserContext>(userContext, verbose, null);

//            Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
//            var routineLogger = new RoutineLogger(correlationToken);
//            var exceptionHandler = new ExceptionHandler(
//                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
//                loggingTransients.TransformException);
//            var input = new { };

//            var routineHandlerFactory = new RoutineHandlerFactory<RoutineClosure<UserContext>>(ticks => {; });
//            var (routineHandler, closure) = routineHandlerFactory.CreateRoutineHandler(
//                    loggingTransients.ShouldBufferVerbose,
//                    (verbose) => new RoutineClosure<UserContext>(userContext, verbose, null),
//                    exceptionHandler,
//                    finishActivity: true,
//                    input,
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    (d,o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d,o),
//                    (d,o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    memberTag,
//                    routineLogger.buffer, 
//                    shouldVerboseWithStackTrace: false,
//                    testInputOutput
//                    );

//            var userRoutineHandler = new UserRoutineHandler<UserContext>(
//                testRepositoryHandlerFactory,
//                testOrmHandlerFactory,
//                routineHandler);
//            string result = null;
//            userRoutineHandler.Handle( c =>
//                {
//                    if (c.UserContext.CultureInfo != CultureInfo.InvariantCulture)
//                    {
//                        throw new ApplicationException("UserContext is not the passed");
//                    }
//                    result = "success";
//                }
//            );
//            if (result != "success")
//                throw new ApplicationException("handle not working properly");
//            if (log.Count != 4)
//                throw new ApplicationException("handle logging not working properly");
//        }

//        [TestMethod]
//        public void RoutinesInjectedExceptionHandle()
//        {
//            var correlationToken = Guid.NewGuid();
//            var memberTag = new MemberTag(this);
//            var log = new List<string>();
//            var loggingTransients = new LoggingTransients(memberTag, log);

//            Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
//            var routineLogger = new RoutineLogger(correlationToken);
//            var exceptionHandler = new ExceptionHandler(
//                ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
//                loggingTransients.TransformException
//            );
//            var input = new { };
//            var routineHandlerFactory = new RoutineHandlerFactory<CustomRoutineClosure>(ticks => {; });
//            var (routineHandler, closure) = routineHandlerFactory.CreateRoutineHandler(
//                    loggingTransients.ShouldBufferVerbose,
//                    (verbose) => new CustomRoutineClosure(correlationToken, verbose),
//                    exceptionHandler,
//                    finishActivity: true,
//                    input,
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d, o),
//                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    memberTag,
//                    routineLogger.buffer, 
//                    shouldVerboseWithStackTrace: false,
//                    testInputOutput
//                    );

//            try
//            {
//                routineHandler.Handle( c => {
//                        c.Verbose("- test verbose");
//                        throw new ApplicationException("failure");
//                    }
//                );
//            }
//            catch (ApplicationException ex)
//            {
//                if (ex.Message!="failure")
//                    throw new ApplicationException("handle logging not working properly");
//                if (log.Count != 5) /* start + exception + finish + input + test verbose */
//                    throw new ApplicationException("handle logging not working properly");
//            }
//        }

//        [TestMethod]
//        public void RoutinesInjectedExceptionHandle2()
//        {
//            var memberTag = new MemberTag("RoutinesTest", "RoutinesInjectedExceptionHandle2");
//            var correlationToken = Guid.NewGuid();
//            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };

//            var log = new List<string>();
//            var loggingTransients = new LoggingTransients(memberTag, log);
//            IOrmHandlerGFactory<UserContext> testOrmHandlerFactory = null; // new TestOrmHandlerFactory();
//            IRepositoryHandlerGFactory<UserContext> testRepositoryHandlerFactory = null; // new TestRepositoryHandlerFactory();
//            //var testRepositoryHandlerFactory = new TestRepositoryHandlerFactory(); // stub
//            //var testOrmHandlerFactory = new TestOrmHandlerFactory(); // stub
//            Func<Action<DateTime, string>, RoutineClosure<UserContext>> createRoutineState =
//                (verbose) => new RoutineClosure<UserContext>(userContext,  null/*no verbose logging for this memberTag */, null);
//            var input = new { };
//            try
//            {
//                Func<object, object, TimeSpan, bool> testInputOutput = (input2, output, duration) => true;
//                var routineLogger = new RoutineLogger(correlationToken);
//                var exceptionHandler = new ExceptionHandler(
//                    ex => loggingTransients.BasicRoutineLoggingAdapter.LogException(DateTime.Now, ex),
//                    loggingTransients.TransformException
//                );

//                var routineHandlerFactory = new RoutineHandlerFactory<RoutineClosure<UserContext>>(ticks => {; });

//                var (routineHandler, closure) = routineHandlerFactory.CreateRoutineHandler(
//                    loggingTransients.ShouldBufferVerbose,
//                    (verbose) => new RoutineClosure<UserContext>(userContext, null/*no verbose logging for this test */, null),
//                    exceptionHandler,
//                    finishActivity: true,
//                    input,
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Input(d, o),
//                    (d, o) => loggingTransients.BasicRoutineLoggingAdapter.Output(d, o),
//                    loggingTransients.BasicRoutineLoggingAdapter,
//                    memberTag,
//                    routineLogger.buffer, 
//                    shouldVerboseWithStackTrace: false,
//                    testInputOutput
//                    );

//                var userRoutineHandler = new UserRoutineHandler<UserContext>(
//                    testRepositoryHandlerFactory,
//                    testOrmHandlerFactory,
//                    routineHandler
//                );

//                routineHandler.Handle((state) =>
//                {
//                    if (state.Verbose!=null)
//                        throw new InvalidOperationException("failure");
//                    throw new ApplicationException("failure");
//                }
//                );
//            }
//            catch (ApplicationException ex)
//            {
//                if (ex.Message != "failure")
//                    throw new ApplicationException("handle logging not working properly");
//                if (log.Count != 4) /* start + exception + finish + input  */
//                    throw new ApplicationException("handle logging not working properly");
//            }
//        }
//    }
//}
