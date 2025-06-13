using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.OData.Deltas;
using Microsoft.AspNetCore.OData.Query;
using Microsoft.AspNetCore.OData.Results;
using Microsoft.AspNetCore.OData.Routing.Controllers;

namespace AdminkaV1.StorageDom.WebApi.Controllers;

/*
 * OData was used. 
 * OData is good for automatically filtering, sorting, paging, and querying data.
 * CRUD operations are not OData, so OData it is explicit here. But we are testing how it works with react
 */

[ApiController]
[Route("ui/exctables")]
[Authorize] // requires an authenticated user, means react should send a  authentication token (JWT)
public class UiTablesController(ILogger<UiTablesController> logger) : ODataController
{
    private readonly ILogger<UiTablesController> _logger = logger;

    //[HttpGet(Name = "GetConnections")]
    //public async Task<IEnumerable<ExcTable>> GetConnectionsAsync()
    //{
    //    var container = StaticTools.GetContainer();
    //    var excConnectionsService = container.GetExcConnectionsService();
    //    var excConnections = await excConnectionsService.GetAllAsync();
    //    return excConnections;
    //}


    [EnableQuery] // enables OData query options like $filter, $orderby, $top, $skip etc. GET /ui/connections/ExcConnections?$filter=Price gt 100&$orderby=Name
    [HttpGet]
    public ActionResult<IQueryable<ExcTable>> Get()
    {
        // returns IQueryable! context is not disposed, connection is not closed, so it can be used for OData queries
        return Ok(StaticTools.CreateExcDbContext().ExcTables);
    }

    [EnableQuery]
    [HttpGet("{key}")]
    public SingleResult<ExcTable> Get(string key)
        => SingleResult.Create(StaticTools.CreateExcDbContext().ExcTables.Where(p => p.ExcConnectionId == key));


    // OData is not intensively used for POST, PATCH, DELETE, so we use standard ASP.NET Core actions
    // Still there are  Created, Updated  replies - they are part of OData specification
    // Created and Updated handles $expand by expecting query optios, but this is not used here
    [HttpPost]
    public IActionResult Post([FromBody] ExcTable excTable)
    {
        using var dbContext = StaticTools.CreateExcDbContext();
        dbContext.ExcTables.Add(excTable);
        dbContext.SaveChanges();
        return Created(excTable);
    }

    // ODATA delta can have complex context (several related entities). There it is just a plain entity.
    [HttpPatch("{key}")]
    //[Route("/odata/connections")] // replacement of class controller route (starts as absolute path)
    public IActionResult Patch(string key, [FromBody] Delta<ExcTable> delta)
    {
        //OData delta is not used here
        // use next 3 lines for test
        //var unchanged = delta.GetUnchangedPropertyNames()?.ToList();
        //var changed = delta.GetChangedPropertyNames()?.ToList();
        //bool s = delta.TryGetPropertyValue("Name", out var newValue);
        using var dbContext = StaticTools.CreateExcDbContext();
        var entity = dbContext.ExcTables.Find(key);
        if (entity == null) return NotFound();

        var updatedEntity = delta.Patch(entity);
        dbContext.SaveChanges();
        return Updated(updatedEntity);
    }

    // No OData processing Delete - regular ASP
    [HttpDelete("{key}")]
    public IActionResult Delete(string key)
    {
        using var dbContext = StaticTools.CreateExcDbContext();
        var entity = dbContext.ExcTables.Find(key);
        if (entity == null) return NotFound();

        dbContext.ExcTables.Remove(entity);
        dbContext.SaveChanges();
        return NoContent();
    }

}
