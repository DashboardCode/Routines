using System;
using System.Threading.Tasks;
using System.Collections.Generic;

using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

using DashboardCode.Routines.AspNetCore;
using DashboardCode.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp.Areas.Logs.Pages
{
    public class AllModel : PageModel
    {
        public List<string> Records { get; set; }

        public string RangeValue { get; set; } = "";
        public string Filter { get; set; } = "";
        public string SinceDate { get; set; } = "";
        public string TillDate { get; set; } = "";

        public IActionResult OnGet()
        {
            Referrer referrer = null;
            var pageRoutineFeature = AspNetCoreManager.SetAndGetPageRoutineFeature(this, referrer);
            var routine = new PageContainerRoutineHandler(this, pageRoutineFeature);
            var page = routine.Handle((container, closure) =>
                {
                    var sinceDate = HttpContext.Request.Query.GetNDate("Since", "MM.dd.yyyy");
                    var tillDate = HttpContext.Request.Query.GetNDate("Till", "MM.dd.yyyy");
                    var filter = HttpContext.Request.Query.GetString("Filter");

                    if (sinceDate != null && tillDate != null)
                    {
                        SinceDate = sinceDate.Value.ToString("MM/dd/yyyy");
                        TillDate = tillDate.Value.ToString("MM/dd/yyyy");
                        RangeValue = SinceDate + " - " + TillDate;
                    }
                    Filter = filter; 
                    return Page();
                }
            );
            return page;
        }
    }
}