using System;
using System.Linq;
using System.Globalization;
using System.Threading.Tasks;

using Microsoft.Extensions.Caching.Memory;
using Microsoft.EntityFrameworkCore;
using Microsoft.AspNetCore.Mvc;

using DashboardCode.Routines.Json;
using DashboardCode.AspNetCore;

namespace DashboardCode.AdminkaV1.Injected.AspCore.WebApp.Areas.Logs
{

    // API Controller could be not in the Pages folder since it is not rotable by folder structure
    // API Controller on NET5 is routable only by attribute routing. Alternatively by HttpGet|HttpPost("[area]/api/[controller]/[action]") route pattern
    // API Controller still requires  endpoints.MapControllers() in startup
    // Those two methods works to get URI:
    // var uri1 = Url.Action(new UrlActionContext() { Controller = "Values", Action = "GetValue", Values = new { area = "myarea" } });
    // var uri2 = Url.Action(controller: "Values", action: "GetValue", values: new { area = "myarea" }); // shorter form  from extension. requires using Microsoft.AspNetCore.Mvc;
    // Important information about routing can be obtained using
    //var UrlList = this.actionDescriptorCollectionProvider.ActionDescriptors.Items
    //            .Select(descriptor => '/' + string.Join('/', descriptor.RouteValues.Values.Where(v => v != null).Select(c => c).Reverse())).Distinct().ToList();
    // alternative populare pattern is [Route("[area]/api/[controller]/[action]")]
    // note: leading '/' will be added it is absent in the pattern.
    [Area(nameof(Logs))]
    [Route("[area]/[controller]/[action]")]
    [ApiController]
    public class LogsApiController : ControllerBase
    {
        private readonly IMemoryCache memoryCache;
        private readonly ApplicationSettings applicationSettings;

        public LogsApiController(ApplicationSettings applicationSettings, IMemoryCache memoryCache)
        {
            this.memoryCache = memoryCache;
            this.applicationSettings = applicationSettings;
        }

        readonly static CachedFormatter getRecordsCachedFormatter = new();
        [HttpPost]
        public async Task<IActionResult> GetRecords()
        {
            var router = new ApiRoutineHandlerAsync(this, applicationSettings, memoryCache);
            return await router.HandleAsync(async (PerCallContainer<UserContext> container, Routines.RoutineClosure<UserContext> closure) => {
                // TODO: priveleges
                var routine = container.ResolveLoggingDomDbContextHandlerAsync();
                return await routine.HandleDbContextAsync(
                   async db =>
                   {
                       var formCollection = this.HttpContext.Request.Form;
                       var enUS = new CultureInfo("en-US");
                       DateTime? since = formCollection.GetNDate("since", "MM/dd/yyyy");
                       DateTime? till = formCollection.GetNDate("till", "MM/dd/yyyy");

                       var cachedList = await memoryCache.GetOrCreateAsync(
                           "GetRecords", async c =>
                           {
                               c.SetAbsoluteExpiration(TimeSpan.FromSeconds(20))
                                .SetSlidingExpiration(TimeSpan.FromSeconds(5));
                               return await db.ActivityRecords.AsNoTracking().Select(
                               e => new
                               {
                                   e.ActivityRecordLoggedAt, e.ActivityRecordId, e.FullActionName 
                               }
                           ).ToListAsync();
                           }
                       );

                       var recordsTotal = cachedList.Count;

                       var (startPageAtIndex, pageLength, searchValue, columnsOrders, columnsSearches) = AspNetCoreManager.GetJQueryDataTableRequest(this);

                       var queryable = cachedList.Where(e => (string.IsNullOrEmpty(searchValue)
                                    || e.FullActionName.ToString() == searchValue
                                    || "ID" + e.ActivityRecordId.ToString() == searchValue
                                )
                                && (since == null || e.ActivityRecordLoggedAt >= since.Value)
                                && (till == null || e.ActivityRecordLoggedAt <= till.Value)
                       );

                       foreach (var c in columnsOrders)
                       {
                           switch (c.Item1)
                           {
                               case 1:
                                   if (c.Item2)
                                       queryable = queryable.OrderByDescending(e => e.ActivityRecordId);
                                   else
                                       queryable = queryable.OrderBy(e => e.ActivityRecordId);
                                   break;
                               case 2:
                                   if (c.Item2)
                                       queryable = queryable.OrderByDescending(e => e.ActivityRecordLoggedAt);
                                   else
                                       queryable = queryable.OrderBy(e => e.ActivityRecordLoggedAt);
                                   break;
                               case 3:
                                   if (c.Item2)
                                       queryable = queryable.OrderByDescending(e => e.FullActionName);
                                   else
                                       queryable = queryable.OrderBy(e => e.FullActionName);
                                   break;
                               default:
                                   queryable = queryable.OrderByDescending(e => e.ActivityRecordLoggedAt);
                                   break;
                           }
                       }

                       var list = queryable.ToList();
                       var recordsFiltered = list.Count;

                       var json = list
                                .Skip(startPageAtIndex > recordsFiltered ? (recordsFiltered - pageLength < 0 ? 0 : recordsFiltered - pageLength) : startPageAtIndex)
                                .Take(pageLength)
                                .ToJsonAll(new { recordsTotal, recordsFiltered }, getRecordsCachedFormatter,
                                    include: chain => chain
                                        .Include(e => e.ActivityRecordId) // 1
                                        .Include(e => e.ActivityRecordLoggedAt) // 2
                                        .Include(e => e.FullActionName) // 3
                                        ,
                                    objectAsArray: true, rootAsProperty: "data",
                                    rootPropertyAppender: (j, c) => j.AddNumberProperty("recordsTotal", c.recordsTotal)
                                                                    .AddNumberProperty("recordsFiltered", c.recordsFiltered)
                                 );

                       return new ContentResult { Content = json, ContentType = "application/json" };
                   });
                   
            });
        }
    }
}