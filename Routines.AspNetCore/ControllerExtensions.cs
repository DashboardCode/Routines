using System.Text;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;

using DashboardCode.Routines.Json;

namespace DashboardCode.Routines.AspNetCore
{
    public static class ControllerExtensions
    {
        public static string ToLog(this HttpRequest request)
        {
            var sb = new StringBuilder();
            if (!request.HasFormContentType)
            {
                sb.Append(request.GetDisplayUrl());
            }
            else
            {
                var formCollection = request.Form;

                if (formCollection.Keys.Count > 0)
                {
                    sb.Append("{");
                    foreach (var key in formCollection.Keys)
                    {
                        var value = formCollection[key];
                        sb.Append(key).Append(":");
                        if (value.Count == 1)
                            sb.Append("\"").AppendJsonEscaped(value).Append("\"").Append(",");
                        else if (value.Count > 1)
                        {
                            sb.Append("[");
                            foreach (var v in value)
                                sb.Append("\"").AppendJsonEscaped(value).Append("\"").Append(",");
                            sb.Length--;
                            sb.Append("]");
                        }
                    }
                    sb.Length--;
                    sb.Append("}");
                }
            }
            var text = sb.ToString();
            return text;
        }

    }
}