using System;
using System.Linq;
using System.Diagnostics;
using System.Data.SqlClient;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.MvcApp.Pages
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

        public static bool FindSqlException(AggregateException aggregateException, out SqlException sqlException)
        {
            sqlException = null;
            foreach (var ex in aggregateException.InnerExceptions)
            {
                if (ex is SqlException)
                {
                    sqlException = (SqlException)ex;
                    return true;
                }
            }
            return false;
        }

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
            bool isDown = false;
            bool isOverloaded = false;
            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;

            SqlException sqlException = null;
            if (unhandledException is SqlException)
            {
                sqlException = (SqlException)unhandledException;
            }
            else if (unhandledException is AggregateException aggregateException)
            {
                FindSqlException((AggregateException)unhandledException, out sqlException);
            }
            if (sqlException!=null)
            {
                // sql server tests
                // SELECT * FROM SYS.MESSAGES where language_id = 1033 order by message_id
                switch (sqlException.Number)
                {
                    case 17:    // SQL Server does not exist or access denied.
                    case 40:    // Could not open a connection to SQL Server
                    case 4060:  // Invalid Database (checked by SYS.MESSAGES)
                    case 18456: // Login Failed (checked by SYS.MESSAGES)
                    case 9002:  // Full transaction log (checked by SYS.MESSAGES) - means under maitinance job
                        isDown = true;
                        break;
                    case 1205:  // DeadLock Victim (checked by SYS.MESSAGES)
                    case -2:    // Client level timeout - Execution Timeout Expired.  The timeout period elapsed prior to completion of the operation or the server is not responding.
                        isOverloaded = true;
                        break;
                }

                if (isDown)
                {
                    Message = "Downloads are currently down for maintenance. Back soon.";
                    Title = "Maintenance";
                }
                else if (isOverloaded)
                {
                    Message = "Downloads are a bit overloaded right now... We are sorry asking you try again later";
                    Title = "Maintenance";
                }
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