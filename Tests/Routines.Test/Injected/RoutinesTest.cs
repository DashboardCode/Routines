using System;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Globalization;
using Vse.Routines.Storage;

namespace Vse.Routines.Injected.Test
{
    [TestClass]
    public class RoutinesTest
    {
        [TestMethod]
        public void RoutinesInjectedHandle()
        {
            var routineTag = new RoutineTag(this);
            var log = new List<string>();
            var loggingTransients = new LoggingTransients(routineTag, log);

            var routineContainer = new BasicRoutineTransients<StateService>(
                loggingTransients.BasicRoutineLoggingAdapter,
                loggingTransients.TransformException,
                (verbose) => new StateService(routineTag.CorrelationToken, verbose)
                );

            string result = null;
            var routine = new Routine<StateService>(routineContainer, new { });
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
            public IRepositoryHandler<TEntity> CreateRepositoryHandler<TEntity>(RoutineState<UserContext> state) where TEntity : class
            {
                throw new NotImplementedException();
            }
        }

        [TestMethod]
        public void RoutinesInjectedHandle2()
        {
            var correlationToken = Guid.NewGuid();
            var log = new List<string>();
            var routineTag = new RoutineTag(correlationToken, "RoutinesTest", "RoutinesInjectedHandle2");
            var loggingTransients = new LoggingTransients(routineTag, log);

            var routineTransients = new BasicRoutineTransients<StateService>(
                loggingTransients.BasicRoutineLoggingAdapter,
                loggingTransients.TransformException,
                (verbose) => new StateService(correlationToken, verbose)
                );
            var testRepositoryHandlerFactory = new TestRepositoryHandlerFactory();
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };
            Func<Action<DateTime, string>, RoutineState<UserContext>> createRoutineState =
                (verbose)=>new RoutineState<UserContext>(userContext, routineTag, verbose, null);
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
            var routineTag = new RoutineTag(this);
            var log = new List<string>();
            var loggingTransients = new LoggingTransients(routineTag, log);
            var routineContainer = new BasicRoutineTransients<StateService>(
                loggingTransients.BasicRoutineLoggingAdapter,
                loggingTransients.TransformException,
                (verbose)=>new StateService(routineTag.CorrelationToken, verbose)
                );
            try
            {
                var routine = new Routine<StateService>(routineContainer, new { });

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
            var tag = new RoutineTag(Guid.NewGuid(), "RoutinesTest", "RoutinesInjectedExceptionHandle2");
            var userContext = new UserContext { CultureInfo = CultureInfo.InvariantCulture };

            var log = new List<string>();
            var loggingTransients = new LoggingTransients(tag, log);
            var testRepositoryHandlerFactory = new TestRepositoryHandlerFactory(); // stub
            Func<Action<DateTime, string>, RoutineState<UserContext>> createRoutineState =
                (verbose) => new RoutineState<UserContext>(userContext, tag, null/*no verbose logging for this routineTag */, null); 
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
