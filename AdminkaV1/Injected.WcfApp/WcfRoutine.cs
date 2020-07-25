using System;
using System.Collections.Generic;

using DashboardCode.Routines.Configuration;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    public class WcfRoutine : AdminkaAnonymousRoutineHandler
    {
        public static readonly ApplicationSettings ApplicationSettings = InjectedManager.CreateApplicationSettingsClassic();

        public WcfRoutine(Routines.MemberTag memberTag, string faultCodeNamespace, object input) 
            : this(Guid.NewGuid(), memberTag,  faultCodeNamespace,
                    input)
        {
        }

        protected WcfRoutine(Guid correlationToken, Routines.MemberTag memberTag, string faultCodeNamespace,
             object input)
            : this(
                  correlationToken, memberTag, faultCodeNamespace,
                  ApplicationSettings,
                  InjectedManager.ResetConfigurationContainerFactoryClassic(),
                  input)
        {
        }

        protected WcfRoutine(
            Guid correlationToken,
            Routines.MemberTag memberTag,
            string faultCodeNamespace,
            ApplicationSettings applicationSettings,
            IConfigurationContainerFactory configurationContainerFactory, 
            object input)
            : base(
                  applicationSettings: applicationSettings,
                  performanceCounters: ApplicationSettings.PerformanceCounters,
                  configurationContainerFactory: configurationContainerFactory,
                  transformException: (ex, g, mt/*, md*/) => TransformException(ex, g, mt, faultCodeNamespace/*, md*/),
                  correlationToken: correlationToken,
                  documentBuilder: null,
                  memberTag: memberTag,
                  anonymousUserContext: new AnonymousUserContext(),
                  input: input)
        {
        }

        
        public static Exception TransformException(
            Exception exception, 
            Guid correlationToken, 
            Routines.MemberTag memberTag, 
            string faultCodeNamespace/*, Func<Exception, string> markdownException*/)
        {
            var code = default(string);
            string message;
            if (exception is AdminkaException adminkaException)
            {
                message = adminkaException.Message;
                code = adminkaException.Code;
            }
            else
            {
                message = "Remote server error: " + exception.Message + "(" + exception.GetType().FullName + ")";
                if (exception.Data.Contains("Code"))
                    code = exception.Data["Code"] as string;
            }

            var routineError = new RoutineError()
            {
                CorrelationToken = correlationToken,
                MemberTag = new MemberTag()
                {
                    Namespace = memberTag.Namespace,
                    Type = memberTag.Type,
                    Member = memberTag.Member
                },
                Message = message,
                AdminkaExceptionCode = code,
                
                Details = InjectedManager.Markdown(exception)
            };

            if (exception.Data.Count>0)
            {
                var data = new Dictionary<string, string>();
                foreach(var k in exception.Data.Keys)
                {
                    if (k is string kStr && exception.Data[k] is string vStr)
                       data[kStr] = vStr;
                }
                if (data.Count > 0)
                {
                    routineError.Data = data;
                }
            }
            return new WcfException(routineError, message, "UNSPECIFIED", faultCodeNamespace);
        }
    }

    public class WcfRoutineAsync : AdminkaAnonymousRoutineHandlerAsync
    {
        public static readonly ApplicationSettings ApplicationSettings = InjectedManager.CreateApplicationSettingsClassic();

        public WcfRoutineAsync(Routines.MemberTag memberTag, string faultCodeNamespace, object input)
            : this(Guid.NewGuid(), memberTag, faultCodeNamespace,
                    input)
        {
        }

        protected WcfRoutineAsync(Guid correlationToken, Routines.MemberTag memberTag, string faultCodeNamespace,
             object input)
            : this(
                  correlationToken, memberTag, faultCodeNamespace,
                  ApplicationSettings,
                  InjectedManager.ResetConfigurationContainerFactoryClassic(),
                  input)
        {
        }

        protected WcfRoutineAsync(
            Guid correlationToken,
            Routines.MemberTag memberTag,
            string faultCodeNamespace,
            ApplicationSettings applicationSettings,
            IConfigurationContainerFactory configurationContainerFactory,
            object input)
            : base(
                  applicationSettings: applicationSettings,
                  performanceCounters: ApplicationSettings.PerformanceCounters,
                  configurationContainerFactory: configurationContainerFactory,
                  transformException: (ex, g, mt/*, md*/) => TransformException(ex, g, mt, faultCodeNamespace/*, md*/),
                  correlationToken: correlationToken,
                  documentBuilder: null,
                  memberTag: memberTag,
                  anonymousUserContext: new AnonymousUserContext(),
                  input: input)
        {
        }


        public static Exception TransformException(
            Exception exception,
            Guid correlationToken,
            Routines.MemberTag memberTag,
            string faultCodeNamespace/*, Func<Exception, string> markdownException*/)
        {
            var code = default(string);
            string message;
            if (exception is AdminkaException adminkaException)
            {
                message = adminkaException.Message;
                code = adminkaException.Code;
            }
            else
            {
                message = "Remote server error: " + exception.Message + "(" + exception.GetType().FullName + ")";
                if (exception.Data.Contains("Code"))
                    code = exception.Data["Code"] as string;
            }

            var routineError = new RoutineError()
            {
                CorrelationToken = correlationToken,
                MemberTag = new MemberTag()
                {
                    Namespace = memberTag.Namespace,
                    Type = memberTag.Type,
                    Member = memberTag.Member
                },
                Message = message,
                AdminkaExceptionCode = code,

                Details = InjectedManager.Markdown(exception)
            };

            if (exception.Data.Count > 0)
            {
                var data = new Dictionary<string, string>();
                foreach (var k in exception.Data.Keys)
                {
                    if (k is string kStr && exception.Data[k] is string vStr)
                        data[kStr] = vStr;
                }
                if (data.Count > 0)
                {
                    routineError.Data = data;
                }
            }
            return new WcfException(routineError, message, "UNSPECIFIED", faultCodeNamespace);
        }
    }
}