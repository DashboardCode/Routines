using System;
using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using DashboardCode.AdminkaV1.Injected;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString MarkdownException(this IHtmlHelper helper, Exception exception)
        {
            var html = InjectedManager.ToHtml(exception);
            return new HtmlString(html);
        }
    }
}
