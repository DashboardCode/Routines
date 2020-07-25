using System;
using System.Linq;
using System.Diagnostics;

using Microsoft.AspNetCore.Diagnostics;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.Injected;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp.Pages
{
    public class AccessDeniedModel : PageModel
    {
        public string ReturnUrl { get; set; } = "/";
        public string ReturnUrlTitle { get; set; } = "Admin";

        public string Title { get; set; } = "Error";
        public string Message { get; set; } = "There was been a problem with the website. We are working on resolving it.";
        public string ExceptionHtml { get; set; } = "";
        public string RequestId { get; set; }
        public string CorrelationToken { get; set; }
        public bool ShowAdvancedInformation { get; set; } = false;
        public bool ShowRequestId => !string.IsNullOrEmpty(RequestId);

        readonly ApplicationSettings applicationSettings;
        public AccessDeniedModel(
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


        // TODO setup ReturnUrl
        // For this every link to potentially forbidden should contain returnUrl parameter 
        // one option to get it is HttpContext.Request.GetDisplayUrl();
        void Prepare()
        {
            var routineFeature = this.HttpContext.Features.Get<AspRoutineFeature>();
            var exceptionHandler = this.HttpContext.Features.Get<IExceptionHandlerFeature>();
            var exception = exceptionHandler?.Error;
            if (exception != null && routineFeature==null)
            {
                var markdown = InjectedManager.Markdown(exception);
                var correlationTokenRequest = this.HttpContext.Request.Headers["X-CorrelationToken"].FirstOrDefault();
                Guid.TryParse(correlationTokenRequest, out var correlationToken);
                
                //TODO add internal authorization log ?
                //applicationSettings.UnhandledExceptionLogger.TraceError(correlationToken, markdown);
            }

            RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier;
            
            Message = "Access denied. Ask network administrator to promote your user account.";
            Title = "Security";

            var environment = Environment.GetEnvironmentVariable("ASPNETCORE_ENVIRONMENT");
            if (environment == "Development")
            {
                ShowAdvancedInformation = true;
            }
            else
            {
                var isAdminPrivilege = false; // TODO: add this privilege through config file to the users with specific names
                if (isAdminPrivilege)
                {
                    ShowAdvancedInformation = true;
                }
            }
            if (ShowAdvancedInformation)
            {
                if (routineFeature!=null && routineFeature.TraceDocument != null)
                {
                    var text = routineFeature.TraceDocument.Build();
                    ExceptionHtml = InjectedManager.ToHtml(text);
                }
                else if (exception != null)
                {
                    ExceptionHtml = InjectedManager.ToHtml(exception);
                }
                if (routineFeature!=null)
                    CorrelationToken = routineFeature.CorrelationToken.ToString();
            }
        }
    }
}