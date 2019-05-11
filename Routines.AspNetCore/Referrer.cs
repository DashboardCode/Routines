using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.WebUtilities;

namespace DashboardCode.Routines.AspNetCore
{
    public class Referrer
    {
        public string GoBack { get; private set; }
        readonly string requestPairName;

        HttpRequest httpRequest;
        readonly string defaultUrl;
        readonly bool useHeaderReferrer;
        public readonly Func<string> getId; // "{nameof(Group)}?id=" + getId
        public Referrer(string requestPairName, 
            Func<string> getId, 
            HttpRequest httpRequest, string defaultUrl,bool useHeaderReferrer)
        {
            this.getId = getId;
            this.requestPairName = requestPairName;

            this.httpRequest = httpRequest;
            this.defaultUrl = defaultUrl;
            this.useHeaderReferrer = useHeaderReferrer;

            GoBack = SetAndGetPageRoutineFeature();
        }

        public string Self
        {
            get {
                return $"{getId()}&{requestPairName}={GoBack}";
            }
        }

        public string Internal
        {
            get {
                var currentQuery = QueryHelpers.ParseQuery(GoBack);
                if (currentQuery.TryGetValue(requestPairName, out var internalStringValues))
                {
                    return internalStringValues.First();
                }
                return GoBack;
            }
        }

        private string SetAndGetPageRoutineFeature()
        {
            var url = default(string);
            if (requestPairName != default)
            {
                if (httpRequest.Method == "POST" && httpRequest.HasFormContentType && httpRequest.Form != null && httpRequest.Form.Count() > 0)
                    url = httpRequest.Form[requestPairName];
                else
                    url = httpRequest.Query?.GetQueryString(requestPairName);
            }
            if (url == default)
            {
                // referer is URI (by HTTP stadadrd URI:= scheme:[//authority]path[?query][#fragment] e.g http://localhost:7894/Path/To/Data?Filter=abc)
                if (httpRequest.Headers.TryGetValue("Referer", out var nameValuePairs)) // referer is misspeling in HTTP standard
                {
                    var referer = nameValuePairs.ToString();
                    if (string.IsNullOrEmpty(referer))
                    {
                        url = defaultUrl;
                    }
                    else if (useHeaderReferrer)
                    {
                        // transform absolute to relaive URL
                        var currentUrl = httpRequest.GetDisplayUrl();
                        if (!string.IsNullOrEmpty(currentUrl))
                        {
                            var refererUri = new Uri(referer);
                            var currentUri = new Uri(currentUrl);
                            url = currentUri.MakeRelativeUri(refererUri).ToString();
                        }
                    }
                }
            }
            if (string.IsNullOrEmpty(url))
                url = "/";
            return url;
        }
    }
}
