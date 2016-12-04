using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using System;

namespace Vse.AdminkaV1.Web
{
    public static class HtmlHelperExtensions
    {
        public static HtmlString MarkdownException(this IHtmlHelper helper, Exception message)
        {
            var text = Injected.IoCManager.Markdown(message);
            var markdown = new MarkdownSharp.Markdown();
            var html = markdown.Transform(text);
            return new HtmlString(html);
        }
    }
}
