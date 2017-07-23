using System.Threading.Tasks;
using Microsoft.AspNetCore.Razor.TagHelpers;
using Microsoft.AspNetCore.Html;
using DashboardCode.AdminkaV1.Injected;

namespace DashboardCode.AdminkaV1.Web.MvcCoreApp
{
    /// <summary>
    /// Just a sample of TagHelper. It is not working for exception becaus it is not working for multiline content.
    /// Usage like <markdown>@DashboardCode.AdminkaV1.AdminkaManager.Markdown(Model)</markdown> doesn't work as you can expect. 
    /// Because of Razor @... operator changes the text therefore new lines become encoded through escape characters.
    /// </summary>
    [HtmlTargetElement("markdown")]
    [OutputElementHint("p")]
    public class MarkdownTagHelper : TagHelper
    {
        public override async Task ProcessAsync(TagHelperContext context, TagHelperOutput output)
        {
            var c = (await output.GetChildContentAsync()).GetContent();
            var html = InjectedManager.Markdown(c);
            var htmlString = new HtmlString(html);
            output.Content.SetHtmlContent(htmlString);
        }
    }
}
