using System.Globalization;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    public interface IGroupPartialModel
    {
        Group Entity { get; }
        AdminkaCrudRoutinePageConsumer<Group, int> Crud { get; }
    }

    public class GroupModel : PageModel, IGroupPartialModel
    {
        readonly static GroupMeta meta = Meta.GroupMeta;

        public Group Entity { get; private set; }

        public AdminkaCrudRoutinePageConsumer<Group, int> Crud { get; private set; }

        public Task<IActionResult> OnGetAsync()
        {
            var referrer = new AdminkaReferrer(this.HttpContext.Request, "Groups", () => Entity.GroupId.ToString(CultureInfo.InvariantCulture), "Group");
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(
                this,
                referrer
                );
            return Crud.HandleDetailsAsync(
                e => Entity = e,
                userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                meta.DetailsIncludes,
                meta.KeyConverter,
                meta.FindPredicate
            );
        }
    }
}