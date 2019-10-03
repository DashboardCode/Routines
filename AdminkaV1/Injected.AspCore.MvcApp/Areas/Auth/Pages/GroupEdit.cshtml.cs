using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using DashboardCode.AdminkaV1.AuthenticationDom;
using DashboardCode.Routines.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Auth.Pages
{
    public interface IGroupEditPartialModel
    {
        Group Entity { get; }
    }

    [ValidateAntiForgeryToken]
    public class GroupEditModel : PageModel, IGroupEditPartialModel
    {
        readonly static GroupMeta meta = Meta.GroupMeta;

        public Group Entity { get; private set; }
        
        public AdminkaCrudRoutinePageConsumer<Group, int> Crud;

        public Task<IActionResult> OnGetAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultReferrer: "Groups");
            return Crud.HandleEditAsync(
                e => Entity = e,
                authorize: null,
                meta.EditIncludes, meta.KeyConverter, meta.FindPredicate, 
                meta.ReferencesCollection.PrepareOptions
            );
        }

        public Task<IActionResult> OnPostAsync()
        {
            Crud = new AdminkaCrudRoutinePageConsumer<Group, int>(this, defaultReferrer: "Groups");
            return Crud.HandleEditConfirmedAsync(
                e => Entity = e,
                authorize: userContext => userContext.HasPrivilege(Privilege.ConfigureSystem),
                nameof(Entity), meta.Constructor, meta.FormFields, meta.HiddenFormFields, meta.DisabledFormFields, 
                meta.ReferencesCollection.ParseRelatedOnUpdate
            );
        }
    }
}