// mockup JWT token for dev time, manage it through ENV_DEVDEBUG=DEVDEBUG env. variable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Identity.Client;
using System.Security.Claims;

namespace AdminkaV1.StorageDom.WebApi.Controllers;

[ApiController, Route("ui/authentication"), AllowAnonymous]

public class UiProdAuthenticationController(ILogger<UiConnectionsController> logger, IConfiguration configuration) : ControllerBase
{
    private readonly IConfiguration _configuration = configuration;
    private readonly ILogger<UiConnectionsController> _logger = logger;

    public record PasswordRequest(string Password);

    /*
     *  Mock a JWT Token Locally (for Testing Only!)
     *  Real Authority  (Auth0, etc) is disabled during builder.Services.AddAuthentication
     *  React app then gets JWT token from this method
     */
    [HttpPost(Name = "GetToken")]
    public async Task<IActionResult> GetTokenAsync(PasswordRequest passwordRequest)
    {
        if (passwordRequest.Password != "Welcome")
            return Unauthorized("Invalid password");
        //var config = StaticTools.GetConfiguration();

        var tenantId = _configuration["TenantId"];
        var clientId = "...";
        var clientSecret = _configuration["ClientSecret"];
        var scope = _configuration["Scope"];

        var app = ConfidentialClientApplicationBuilder.Create(clientId)
            .WithClientSecret(clientSecret)
            .WithAuthority($"https://login.microsoftonline.com/{tenantId}")
            .Build();

        //var tokenResult = await app.AcquireTokenForClient(new[] { scope }).ExecuteAsync();
        var tokenResult = await app.AcquireTokenForClient([scope]).ExecuteAsync();
        var token = tokenResult.AccessToken;

        return Ok(new { token });
    }

    [HttpGet("GetIdentityAndClaimsReport")]
    public IActionResult GetIdentityAndClaimsReport()
    {
        var result = "";

        result += string.Format("User.Identity?.Name : {0}\n", User.Identity?.Name);
        result += string.Format("User.IsInRole(\"Connections.Read\") : {0}\n", User.IsInRole("Connections.Read"));
        result += string.Format("User.Identity?.IsAuthenticated : {0}\n", User.Identity?.IsAuthenticated);
        result += string.Format("User.Claims.Where(c => c.Type == \"roles\") : {0}\n", User.Claims
                .Where(c => c.Type == "roles")
                .Select(c => c.Value));

        result += "User.Claims: \n";
        foreach (var claim in User.Claims)
        {
            result += string.Format("Type: {0}, Value: {1} \n", claim.Type, claim.Value);
        }

        result += " User.Identities: \n";
        foreach (var identity in User.Identities)
        {
            result += string.Format("AuthenticationType: {0}, IsAuthenticated: {1}, Roles: {2}",
                identity.AuthenticationType,
                identity.IsAuthenticated,
                string.Join(", ", identity.FindAll(ClaimTypes.Role).Select(c => c.Value)
            ));
        }
        return Ok(result);
    }
}