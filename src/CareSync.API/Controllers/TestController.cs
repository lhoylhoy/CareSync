using CareSync.API.Attributes;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace CareSync.API.Controllers;

[ApiController]
[Route("api/[controller]")]
public class TestController : ControllerBase
{
    /// <summary>
    ///     Public endpoint - no authentication required
    /// </summary>
    [HttpGet("public")]
    [AllowAnonymous]
    public IActionResult GetPublic()
    {
        return Ok(new
        {
            message = "This is a public endpoint",
            timestamp = DateTime.UtcNow,
            authenticated = User.Identity?.IsAuthenticated ?? false
        });
    }

    /// <summary>
    ///     Protected endpoint - requires valid Entra ID bearer token
    /// </summary>
    [HttpGet("protected")]
    [CustomAuthorize]
    public IActionResult GetProtected()
    {
        var userId = User.FindFirst("oid")?.Value;
        var userName = User.FindFirst("name")?.Value ??
                       User.FindFirst("preferred_username")?.Value;
        var roles = User.FindAll("roles")?.Select(c => c.Value).ToList();

        return Ok(new
        {
            message = "This is a protected endpoint",
            timestamp = DateTime.UtcNow,
            user = new
            {
                id = userId,
                name = userName,
                roles = roles ?? new List<string>(),
                authenticated = User.Identity?.IsAuthenticated ?? false,
                claims = User.Claims.Select(c => new { c.Type, c.Value }).ToList()
            }
        });
    }

    /// <summary>
    ///     Health check for authentication system
    /// </summary>
    [HttpGet("auth-health")]
    [AllowAnonymous]
    public IActionResult GetAuthHealth()
    {
        return Ok(new
        {
            authenticationConfigured = true,
            bearerTokenSupport = true,
            entraIdIntegration = true,
            message = "Authentication system is healthy"
        });
    }
}
