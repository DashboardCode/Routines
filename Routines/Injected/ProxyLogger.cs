//using System;
//using System.Collections.Generic;
//using System.Text;

//namespace DashboardCode.Routines.Injected
//{
//    public class ProxyLogger : IActivityGFactory, IVerboseLogger
//    {
//        public ProxyLogger()
//        {
//            IActivityGFactory activityGFactory = null;
//            Action<DateTime, string> logVerbose = null;
//            if (!loggingVerboseConfiguration.ShouldBufferVerbose)
//            {
//                activityGFactory = new RoutineLogging(activityState, rawRoutineLogger);
//                logVerbose = rawRoutineLogger.LogVerbose;
//            }
//            else
//            {
//                var buffered = new BufferedVerboseLogging(
//                        routineLogger,
//                        rawRoutineLogger,
//                        rawRoutineLogger.LogBufferedVerbose,
//                        loggingVerboseConfiguration.ShouldVerboseWithStackTrace
//                    );
//                activityGFactory = new BufferedRoutineLogging(
//                    activityState,
//                    buffered,
//                    testInput,
//                    testOutput);
//                logVerbose = buffered.LogVerbose;
//            };
//        }

//        public Func<(Action<object>, Action)> Compose(object input)
//        {
//            throw new NotImplementedException();
//        }

//        public void LogVerbose(DateTime dateTime, string message)
//        {
//            throw new NotImplementedException();
//        }
//    }
//}
