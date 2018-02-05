using System;
using System.Collections.Generic;

using DashboardCode.Routines;
using DashboardCode.Routines.Configuration;
using DashboardCode.Routines.Configuration.NETFramework;

using DashboardCode.AdminkaV1.DataAccessEfCore;
using DashboardCode.AdminkaV1.Injected.NETFramework;

namespace DashboardCode.AdminkaV1.Injected.WcfApp
{
    public class WcfRoutine : AdminkaRoutineHandler
    {
        public WcfRoutine(MemberTag memberTag, string faultCodeNamespace, object input) 
            : this(new Routines.RoutineGuid(memberTag), GetUserContext(), faultCodeNamespace,
                  new ConfigurationManagerLoader(), input)
        {
        }

        protected WcfRoutine(Routines.RoutineGuid routineGuid, UserContext userContext, string faultCodeNamespace,
            ConfigurationManagerLoader configurationManagerLoader, object input)
            : this(routineGuid, GetUserContext(), faultCodeNamespace,
                  new SqlServerAdmikaConfigurationFacade(configurationManagerLoader).ResolveAdminkaStorageConfiguration(),
                  new ConfigurationFactory(configurationManagerLoader),  input)
        {
        }

        protected WcfRoutine(Routines.RoutineGuid routineGuid, UserContext userContext, string faultCodeNamespace,
            AdminkaStorageConfiguration adminkaStorageConfiguration,
            IConfigurationFactory configurationFactory, object input)
            : base(
                  adminkaStorageConfiguration,
                  configurationFactory,
                  (ex, rg, md) => TransformException(ex, rg, faultCodeNamespace, md),
                  routineGuid, userContext,
                  input)
        {
        }

        private static UserContext GetUserContext() =>
            new UserContext("Anonymous");

        public static Exception TransformException(Exception exception, Routines.RoutineGuid routineGuid, string faultCodeNamespace, Func<Exception, string> markdownException)
        {
            var message = default(string);
            var code = default(string);
            if (exception is AdminkaException)
            {
                message = exception.Message;
                code = ((AdminkaException)exception).Code;
            }
            else
            {
                message = "Remote server error: " + exception.Message + "(" + exception.GetType().FullName + ")";
                if (exception.Data.Contains("Code"))
                    code = exception.Data["Code"] as string;
            }

            var routineError = new RoutineError()
            {
                RoutineGuid = new RoutineGuid()
                {
                    CorrelationToken = routineGuid.CorrelationToken,
                    Namespace = routineGuid.MemberTag.Namespace,
                    Type = routineGuid.MemberTag.Type,
                    Member = routineGuid.MemberTag.Member
                },
                Message = message,
                AdminkaExceptionCode = code,
                Details = markdownException(exception)
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
}