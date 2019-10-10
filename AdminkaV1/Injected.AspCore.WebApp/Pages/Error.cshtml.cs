using System;
using System.Linq;
using System.Diagnostics;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;
using DashboardCode.Routines.Storage.SqlServer;
using DashboardCode.Routines.Storage;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Pages
{
    public class ErrorModel : PageModel
    {
        public readonly ApplicationSettings applicationSettings;
        public string ReturnUrl { get; set; } = "/";
        public string ReturnUrlTitle { get; set; } = "Admin";

        public string Title { get; set; } = "Error";
        public string Message { get; set; } = "There was been a problem with the website. We are working on resolving it.";
        public string ExceptionHtml { get; set; } = "";
        public string RequestId { get; set; }
        public string CorrelationToken { get; set; }
        public bool ShowAdvancedInformation { get; set; } = false;
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        public ErrorModel(
            ApplicationSettings applicationSettings
            )
        {
            this.applicationSettings = applicationSettings;
        }

        public void OnGet()
        {
            Prepare();
        }

        public void OnPost()
        {
            Prepare();
        }

        //public static bool FindSqlException(AggregateException aggregateException, out SqlException sqlException)
        //{
        //    sqlException = null;
        //    foreach (var ex in aggregateException.InnerExceptions)
        //    {
        //        if (ex is SqlException)
        //        {
        //            sqlException = (SqlException)ex;
        //            return true;
        //        }
        //    }
        //    return false;
        //}

        void Prepare()
        {
            var exceptionHandler = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            var aspRoutineFeature = this.HttpContext.Features.Get<AspRoutineFeature>();
            var pageRoutineFeature = this.HttpContext.Features.Get<PageRoutineFeature>();

            // TODO: create url tree (where to go on error) and url to title (button name) map
            if (pageRoutineFeature != null)
                ReturnUrl = pageRoutineFeature.Referrer;

            var unhandledException = exceptionHandler?.Error;
            string detailsMarkdown = default;
            var isHandledByDocument = aspRoutineFeature != null && aspRoutineFeature.TraceDocument.IsExceptionHandled;
            if (unhandledException != null && !isHandledByDocument)
            {
                detailsMarkdown = InjectedManager.Markdown(unhandledException);
                var correlationTokenRequest = this.HttpContext.Request.Headers["X-CorrelationToken"].FirstOrDefault();
                Guid.TryParse(correlationTokenRequest, out var correlationToken);
                applicationSettings.UnhandledExceptionLogger.TraceError(correlationToken, detailsMarkdown);
            }
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            //SqlException sqlException = null;
            var remoteServerErrorType = SqlServerManager.QuickAnalyze(unhandledException);

            if (remoteServerErrorType == RemoteServerErrorType.DOWN)
            {
                Message = "Adminka is currently down for maintenance. Back soon.";
                Title = "Maintenance";
            }
            else if (remoteServerErrorType == RemoteServerErrorType.OVERLOADED)
            {
                Message = "Adminka is a bit overloaded right now... We are sorry asking you try again later";
                Title = "Maintenance";
            }
            

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development" || applicationSettings.ForceDetailsOnCustomErrorPage)
            {
                ShowAdvancedInformation = true;
            }
            else
            {
                var isAdminPrivilege = User.Claims.Any(c => c.Type == "PRIVILEGE" && c.Value == "ADMIN");
                if (isAdminPrivilege)
                {
                    ShowAdvancedInformation = true;
                }
            }
            if (ShowAdvancedInformation)
            {
                if (isHandledByDocument)
                {
                    var text = aspRoutineFeature.TraceDocument.Build();
                    ExceptionHtml = InjectedManager.ToHtml(text);
                }
                else if (unhandledException != null && detailsMarkdown != null)
                {
                    ExceptionHtml = InjectedManager.ToHtmlException(detailsMarkdown);
                }

                if (aspRoutineFeature != null)
                    CorrelationToken = aspRoutineFeature.CorrelationToken.ToString();
            }
        }
    }
}