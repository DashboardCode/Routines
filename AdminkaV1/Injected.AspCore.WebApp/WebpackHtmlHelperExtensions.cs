using Microsoft.AspNetCore.Html;
using Microsoft.AspNetCore.Mvc.Rendering;
using Newtonsoft.Json;
using System.Collections.Generic;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp
{
    public static class WebpackHtmlHelperExtensions
    {
        public static IHtmlContent MainScript(this IHtmlHelper helper, string fileName)
        {
            return new HtmlFormattableString(
                $"<script type='application/json' src='/js/{fileName}' />");
        }

        private static string GetWebpackAssetsJson(string applicationBasePath)
        {
            string json = @"{""main.js"":""main.90966be3ccbf7991d500.js""}";
            var values = JsonConvert.DeserializeObject<Dictionary<string, string>>(json);
            return values["main.js"];

            //JObject webpackAssetsJson = null;
            //string packageJsonFilePath = $"{applicationBasePath}\\js\\{"manifest.json"}";

            //using (StreamReader packageJsonFile = File.OpenText(packageJsonFilePath))
            //{
            //    using (JsonTextReader packageJsonReader = new JsonTextReader(packageJsonFile))
            //    {
            //        JObject packageJson = (JObject)JToken.ReadFrom(packageJsonReader);
            //        JObject webpackConfigJson = (JObject)packageJson["customConfig"]["webpackConfig"];
            //        string webpackAssetsFileName = webpackConfigJson["assetsFileName"].Value<string>();
            //        string webpackBuildDirectory = webpackConfigJson["buildDirectory"].Value<string>();
            //        string webpackAssetsFilePath = $"{applicationBasePath}\\{webpackBuildDirectory}\\{webpackAssetsFileName}";

            //        using (StreamReader webpackAssetsFile = File.OpenText(webpackAssetsFilePath))
            //        {
            //            using (JsonTextReader webpackAssetsReader = new JsonTextReader(webpackAssetsFile))
            //            {
            //                webpackAssetsJson = (JObject)JToken.ReadFrom(webpackAssetsReader);
            //            }
            //        }
            //    }
            //}

            //return webpackAssetsJson;
        }
    }
}
