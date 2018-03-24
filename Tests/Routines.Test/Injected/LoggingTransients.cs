//using System;
//using System.Collections.Generic;

//namespace DashboardCode.Routines.Injected.Test
//{
//    public class LoggingTransients //: IBasicLoggingTransients
//    {
//        public LoggingAdapter BasicRoutineLoggingAdapter { get; private set; }
//        public Func<Exception, Exception> TransformException { get; private set; }

//        public bool ShouldBufferVerbose
//        {
//            get
//            {
//                return true;
//            }
//        }
//        public bool ShouldVerboseWithStackTrace
//        {
//            get
//            {
//                return false;
//            }
//        }

//        public LoggingTransients(MemberTag memberTag, List<string> log)
//        {
//            var loggingConfiguration = new LoggingConfiguration();
//            BasicRoutineLoggingAdapter = new LoggingAdapter(memberTag, log);
//            TransformException = (ex) => ex;
//        }
//    }
//}
