using System;
using System.Linq;
using System.Diagnostics;
using System.Collections.Generic;

using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Http.Extensions;

namespace DashboardCode.Routines.AspNetCore
{
    public static class AspNetCoreManager
    {
        public static string GetQueryString(this IQueryCollection queryCollection, string pairName)
        {
            string @value = default;
            if (queryCollection.TryGetValue(pairName, out var stringValues))
            {
                if (stringValues.Count > 0)
                    @value = stringValues.ToString();
                else
                    @value = null;
            }
            return @value;
        }
                                        
        public static AspRoutineFeature GetAspRoutineFeature(PageModel pageModel)
        {
            Guid correlationToken = default;
            var correlationTokenString = pageModel.HttpContext.Request.Headers["X-CorrelationToken"].FirstOrDefault();
            if (correlationTokenString == null)
            {
                correlationToken = Guid.NewGuid();
                pageModel.HttpContext.Request.Headers.Add("X-CorrelationToken", correlationToken.ToString());
            }
            else
            {
                correlationToken = Guid.Parse(correlationTokenString);
            }
            pageModel.HttpContext.Response.Headers["X-CorrelationToken"] = correlationToken.ToString();
            var routineFeature = new AspRoutineFeature() { CorrelationToken = correlationToken, AspRequestId = Activity.Current?.Id ?? pageModel.HttpContext.TraceIdentifier, TraceDocument = new TraceDocument()};
            pageModel.HttpContext.Features.Set(routineFeature);
            return routineFeature;
        }

        public static AspRoutineFeature GetAspRoutineFeature(ControllerBase controllerBase)
        {
            Guid correlationToken = default;
            var correlationTokenString = controllerBase.HttpContext.Request.Headers["X-CorrelationToken"].FirstOrDefault();
            if (correlationTokenString == null)
            {
                correlationToken = Guid.NewGuid();
                controllerBase.HttpContext.Request.Headers.Add("X-CorrelationToken", correlationToken.ToString());
            }
            else
            {
                correlationToken = Guid.Parse(correlationTokenString);
            }
            controllerBase.HttpContext.Response.Headers["X-CorrelationToken"] = correlationToken.ToString();
            var routineFeature = new AspRoutineFeature() { CorrelationToken = correlationToken, AspRequestId = Activity.Current?.Id ?? controllerBase.HttpContext.TraceIdentifier, TraceDocument = new TraceDocument() };
            controllerBase.HttpContext.Features.Set(routineFeature);
            return routineFeature;
        }

        public static object GetRequest(HttpRequest httpRequest)
        {
            var rawQueryString = httpRequest.QueryString.ToString();
            var traceList = new List<string>{ "QueryString: " + rawQueryString};
            if (httpRequest.Method == "POST" && httpRequest.HasFormContentType && httpRequest.Form != null && httpRequest.Form.Count() > 0)
            {
                var pairs = httpRequest.Form;
                foreach (var nameValuePair in pairs)
                    traceList.Add(nameValuePair.ToString());
            }
            return traceList;
        }

       
    }
}
