using System;
using System.Linq;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.WebUtilities;

namespace DashboardCode.Routines.AspNetCore
{
    /// <summary>
    /// e.g.
    /// ToReferrerHref      = "Groups", // TODO: analize query Referrer if no Groups
    /// CurrentWithReferrer = $"{nameof(Group)}?id={Entity.GroupId}&Referrer=Groups" 
    /// </summary>
    public class Referrer
    {
        Func<string> getCurrentWithReferrer;
        public Referrer(
            string toReferrerHref,
            Func<string> getId,
            string entityName, // $"{nameof(Group)}?id={Entity.GroupId}"
            string referrerRequestPairName = "Referrer"
            )
        {
            //this.getId = getId;
            //this.referrerRequestPairName = referrerRequestPairName;
            this.Href = toReferrerHref;
            this.getCurrentWithReferrer = () =>
            {
                if (getId == null)

                    throw new NotImplementedException("CurrentWithReferrer is not implemented");
                return $"{entityName}?id={getId()}&{referrerRequestPairName}={toReferrerHref}";
            };

        }

        //readonly string referrerRequestPairName;
        //public readonly Func<string> getId; // "{nameof(Group)}?id=" + getId
        public Referrer(
            //string referrerRequestPairName,
            //Func<string> getId,
            string toReferrerHref,
            string currentWithReferrer

            )
        {
            //this.getId = getId;
            //this.referrerRequestPairName = referrerRequestPairName;
            this.Href = toReferrerHref;
            this.getCurrentWithReferrer = () => currentWithReferrer;
            
        }

        //public Referrer(
        //    string toReferrerHref,
        //    string currentWithReferrer
        //    )
        //{
        //    //this.getId = getId;
        //    //this.referrerRequestPairName = referrerRequestPairName;
        //    this.ToReferrerHref = toReferrerHref;
        //    this.CurrentWithReferrer = currentWithReferrer;

        //}
        //public string Self
        //{
        //    get {
        //        return $"{getId()}&{referrerRequestPairName}={GoBackHref}";
        //    }
        //}

        public string Href { get; }
        public string CurrentWithReferrer { get { return this.getCurrentWithReferrer(); }   }
        //public string Internal
        //{
        //    get {
        //        var currentQueryDictionary = QueryHelpers.ParseQuery(GoBackHref);
        //        string value;
        //        if (currentQueryDictionary.TryGetValue(referrerRequestPairName, out var internalStringValues))
        //        {
        //            @value = internalStringValues.First();
        //        }
        //        else
        //        {
        //            @value = GoBack;
        //        }
        //        return @value;
        //    }
        //}
    }
}
