using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using AngleSharp.Html.Dom;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.IntegrationTest
{
    public static class HttpClientExtensions
    {
        public static void TransferAntiforgeryCookie(this HttpClient httpClient, HttpResponseMessage httpResponseMessage)
        {
            var cookieList = httpResponseMessage.Headers.GetValues("Set-Cookie");
            var first = cookieList.Single();
            var separatorIndex = first.IndexOf(';');
            var cookie = first.Substring(0, separatorIndex);
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            //httpClient.DefaultRequestHeaders.Add("Cookie", cookieList);
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IHtmlElement submitButton)
        {
            return client.SendAsync(form, submitButton, new Dictionary<string, string>());
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IEnumerable<KeyValuePair<string, string>> formValues)
        {
            var submitElement = form.QuerySelectorAll("[type=submit]");
            var submitButton = (IHtmlElement)submitElement;
            return client.SendAsync(form, submitButton, formValues);
        }

        public static Task<HttpResponseMessage> SendAsync(
            this HttpClient client,
            IHtmlFormElement form,
            IHtmlElement submitButton,
            IEnumerable<KeyValuePair<string, string>> formValues)
        {
            foreach (var kvp in formValues)
            {
                var element = (IHtmlInputElement)(form[kvp.Key]);
                element.Value = kvp.Value;
            }

            var submit = form.GetSubmission(submitButton);
            var target = (Uri)submit.Target;
            if (submitButton.HasAttribute("formaction"))
            {
                var formaction = submitButton.GetAttribute("formaction");
                target = new Uri(formaction, UriKind.Relative);
            }
            var submission = new HttpRequestMessage(new HttpMethod(submit.Method.ToString()), target)
            {
                Content = new StreamContent(submit.Body)
            };

            foreach (var header in submit.Headers)
            {
                submission.Headers.TryAddWithoutValidation(header.Key, header.Value);
                submission.Content.Headers.TryAddWithoutValidation(header.Key, header.Value);
            }

            return client.SendAsync(submission);
        }
    }

    public static class HtmlDocumentExtensions
    {
        public static (string, string) GetRequestVerificationToken(this HtmlDocument htmlDocument)
        {
            var token = htmlDocument.DocumentNode.Descendants("input")
                .Where(x => x.Attributes["name"]?.Value == "__RequestVerificationToken")?
                .First()?.Attributes["value"]?.Value;
            return ("__RequestVerificationToken", token);
        }

        public static (string, string) GetField(this HtmlDocument htmlDocument, string fieldName)
        {
            var token = htmlDocument.DocumentNode.Descendants("input")
                .Where(x => x.Attributes["name"]?.Value == fieldName)?
                .First()?.Attributes["value"]?.Value;
            return (fieldName, token);
        }

        public async static Task<HtmlDocument> GetDocument(this HttpResponseMessage httpResponseMessage)
        {
            var textContent = await httpResponseMessage.Content.ReadAsStringAsync();
            var htmlDocument = new HtmlDocument();
            htmlDocument.LoadHtml(textContent);
            return htmlDocument;
        }

        public static string GetTableCell(this HtmlDocument htmlDocument, string tableName, int conditionColumnNumber, Func<string, bool> condition, int resultColumnNumber)
        {
            var table = htmlDocument.DocumentNode.Descendants("table")
                .Where(x => x.Attributes["id"]?.Value == tableName)?.First();
            var rows = table.Descendants("tr").ToList();
            foreach (var r in rows)
            {
                var cells = r.Descendants("td").ToList();
                if (cells.Count > 0)
                {
                    var cell = cells[conditionColumnNumber].InnerText.Trim();
                    if (condition(cell))
                        return cells[resultColumnNumber].InnerText.Trim();
                }
            }
            throw new Exception("Cell not found");
        }
    }
}