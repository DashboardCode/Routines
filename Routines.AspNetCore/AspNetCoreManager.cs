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

        public static PageRoutineFeature GetPageRoutineFeature(this HttpRequest httpRequest, (string requestPairName, string defaultUrl, bool useReferer) backward)
        {
            var backwardUrl = default(string);
            if (backward.requestPairName != default)
            {
                if (httpRequest.Method == "POST" && httpRequest.HasFormContentType && httpRequest.Form != null && httpRequest.Form.Count() > 0)
                    backwardUrl = httpRequest.Form[backward.requestPairName];
                else
                    backwardUrl = httpRequest.Query?.GetQueryString(backward.requestPairName);
            }
            if (backwardUrl == default)
            {
                // referer is URI (by HTTP stadadrd URI:= scheme:[//authority]path[?query][#fragment] e.g http://localhost:7894/Path/To/Data?Filter=abc)
                if (httpRequest.Headers.TryGetValue("Referer", out var nameValuePairs))
                {
                    var referer = nameValuePairs.ToString();
                    if (string.IsNullOrEmpty(referer))
                    {
                        backwardUrl = backward.defaultUrl;
                    }
                    else if (backward.useReferer)
                    {
                        // transform absolute to relaive URL
                        var currentUrl = httpRequest.GetDisplayUrl();
                        if (!string.IsNullOrEmpty(currentUrl))
                        {
                            var refererUri = new Uri(referer);
                            var currentUri = new Uri(currentUrl);
                            backwardUrl = currentUri.MakeRelativeUri(refererUri).ToString();
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(backwardUrl))
                backwardUrl = "/";
            return new PageRoutineFeature() { BackwardUrl = backwardUrl };
        }
    }
}
