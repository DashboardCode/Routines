using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using HtmlAgilityPack;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;


namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.NETCore.Test
{
    public static class HttpClientExtensions
    {
        //public static async Task<FormUrlEncodedContent> GetRequestContentAsync(
        //    this HttpClient httpClient, string path, IDictionary<string, string> data)
        //{
        //    var httpResponseMessage = await httpClient.GetAsync(path);
        //    //  antiforgery cookie
        //    var cookie = httpResponseMessage.Headers.GetValues("Set-Cookie");
        //    httpClient.DefaultRequestHeaders.Add("Cookie", cookie);

        //    // verification token
        //    var content = await httpResponseMessage.Content.ReadAsStringAsync();
        //    var html = new HtmlDocument();
        //    html.LoadHtml(content);
        //    var token= html.DocumentNode.Descendants("input")
        //        //.Select(y => y.Descendants()
        //        .Where(x => x.Attributes["name"].Value == "__RequestVerificationToken")
        //        .First().Attributes["value"].Value;

        //    // Add the token to the form data for the request.
        //    data.Add("__RequestVerificationToken", token);

        //    return new FormUrlEncodedContent(data);
        //}

        public static void TransferAntiforgeryCookie(this HttpClient httpClient, HttpResponseMessage httpResponseMessage)
        {
            var cookieList = httpResponseMessage.Headers.GetValues("Set-Cookie");
            var first = cookieList.Single();
            var separatorIndex = first.IndexOf(';');
            var cookie = first.Substring(0, separatorIndex);
            httpClient.DefaultRequestHeaders.Add("Cookie", cookie);
            //httpClient.DefaultRequestHeaders.Add("Cookie", cookieList);
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