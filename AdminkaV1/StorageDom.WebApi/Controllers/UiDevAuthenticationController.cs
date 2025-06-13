#if DEVDEBUG // mockup JWT token for dev time, manage it through ENV_DEVDEBUG=DEVDEBUG env. variable
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Tokens;
using System.Data;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace AdminkaV1.StorageDom.WebApi.Controllers;

[ApiController, Route("ui/authenticationdev"), AllowAnonymous]
public class UiDevAuthenticationController(ILogger<UiConnectionsController> logger) : ControllerBase
{
    private readonly ILogger<UiConnectionsController> _logger = logger;

    public record PasswordRequest(string Password);
    
    /*
     *  Mock a JWT Token Locally (for Testing Only!)
     *  Real Authority  (Auth0, etc) is disabled during builder.Services.AddAuthentication
     *  React app then gets JWT token from this method
     */
    [HttpPost(Name = "GetTokenDev")]
    public IActionResult GetToken([FromBody] PasswordRequest passwordRequest)
    {
        if (passwordRequest.Password != "Welcome")
            return Unauthorized("Invalid password");
        var claims = new[]
        {
            new Claim(JwtRegisteredClaimNames.Sub, "user-id-123"),
            new Claim(JwtRegisteredClaimNames.Jti, Guid.NewGuid().ToString()),
            new Claim(ClaimTypes.Name, "testuser")
        };

        var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(StaticTools.DevTimeJwtSecurityKey));
        var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

        var jwtSettings = StaticTools.GetConfiguration().GetSection("Jwt");

        var jwtSecurityToken = new JwtSecurityToken(
            issuer: StaticTools.DevTimeJwtSecurityTokenIssuer,
            audience: jwtSettings["Audience"],
            claims: claims,
            expires: DateTime.UtcNow.AddHours(1),
            signingCredentials: creds
        );
        var token = new JwtSecurityTokenHandler().WriteToken(jwtSecurityToken);
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
#endif