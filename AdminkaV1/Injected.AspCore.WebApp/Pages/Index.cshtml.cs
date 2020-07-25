using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
          
namespace DashboardCode.AdminkaV1.Injected.AspNetCore.WebApp.Pages
{
    public class IndexModel : PageModel
    {

        public IndexModel()
        {
        }

        public IActionResult OnGet()
        {
            return RedirectToPage("/Users", new { area = "Auth" });
        }
    }
}