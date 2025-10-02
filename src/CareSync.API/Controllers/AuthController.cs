using System.ComponentModel.DataAnnotations;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using CareSync.API.Attributes;
using CareSync.API.Identity;
using CareSync.API.Services;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.IdentityModel.Protocols;
using Microsoft.IdentityModel.Protocols.OpenIdConnect;
using Microsoft.IdentityModel.Tokens;

namespace CareSync.API.Controllers;

/// <summary>
///     Authentication controller for user registration and login
/// </summary>
[ApiController]
[Route("api/[controller]")]
public class AuthController : ControllerBase
{
    private readonly ILogger<AuthController> _logger;
    private readonly SignInManager<ApplicationUser> _signInManager;
    private readonly TokenService _tokenService;
    private readonly UserManager<ApplicationUser> _userManager;

    public AuthController(
        UserManager<ApplicationUser> userManager,
        SignInManager<ApplicationUser> signInManager,
        TokenService tokenService,
        ILogger<AuthController> logger)
    {
        _userManager = userManager ?? throw new ArgumentNullException(nameof(userManager));
        _signInManager = signInManager ?? throw new ArgumentNullException(nameof(signInManager));
        _tokenService = tokenService ?? throw new ArgumentNullException(nameof(tokenService));
        _logger = logger ?? throw new ArgumentNullException(nameof(logger));
    }

    /// <summary>
    ///     Register a new user
    /// </summary>
    [HttpPost("register")]
    public async Task<IActionResult> Register([FromBody] RegisterRequest request)
    {
        var user = new ApplicationUser
        {
            UserName = request.Email,
            Email = request.Email,
            FirstName = request.FirstName,
            LastName = request.LastName
        };

        var result = await _userManager.CreateAsync(user, request.Password);

        if (result.Succeeded)
        {
            _logger.LogInformation("User {Email} registered successfully", request.Email);
            return Ok(new { message = "User registered successfully" });
        }

        foreach (var error in result.Errors) ModelState.AddModelError("", error.Description);

        return BadRequest(ModelState);
    }

    /// <summary>
    ///     Login user
    /// </summary>
    [HttpPost("login")]
    public async Task<IActionResult> Login([FromBody] LoginRequest request)
    {
        var user = await _userManager.FindByEmailAsync(request.Email);
        if (user == null) return Unauthorized("Invalid email or password");

        var result = await _signInManager.CheckPasswordSignInAsync(user, request.Password, false);
        if (!result.Succeeded) return Unauthorized("Invalid email or password");

        var roles = await _userManager.GetRolesAsync(user);
        var accessToken = _tokenService.GenerateAccessToken(user, roles);
        var refreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = refreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        _logger.LogInformation("User {Email} logged in successfully", request.Email);

        return Ok(new LoginResponse
        {
            AccessToken = accessToken, RefreshToken = refreshToken, ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    ///     Refresh token
    /// </summary>
    [HttpPost("refresh-token")]
    public async Task<IActionResult> RefreshToken([FromBody] RefreshTokenRequest request)
    {
        var principal = _tokenService.GetPrincipalFromExpiredToken(request.AccessToken);
        if (principal == null) return BadRequest("Invalid access token");

        var username = principal.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username ?? "");

        if (user == null || user.RefreshToken != request.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
            return BadRequest("Invalid refresh token");

        var roles = await _userManager.GetRolesAsync(user);
        var newAccessToken = _tokenService.GenerateAccessToken(user, roles);
        var newRefreshToken = _tokenService.GenerateRefreshToken();

        user.RefreshToken = newRefreshToken;
        user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
        await _userManager.UpdateAsync(user);

        return Ok(new LoginResponse
        {
            AccessToken = newAccessToken, RefreshToken = newRefreshToken, ExpiresAt = DateTime.UtcNow.AddMinutes(60)
        });
    }

    /// <summary>
    ///     Logout user (revoke refresh token)
    /// </summary>
    [HttpPost("logout")]
    [CustomAuthorize]
    public async Task<IActionResult> Logout()
    {
        var username = User.Identity?.Name;
        var user = await _userManager.FindByNameAsync(username ?? "");

        if (user != null)
        {
            user.RefreshToken = null;
            user.RefreshTokenExpiryTime = DateTime.UtcNow;
            await _userManager.UpdateAsync(user);
        }

        return Ok(new { message = "Logged out successfully" });
    }

    /// <summary>
    ///     Exchange Entra ID token for local JWT token
    /// </summary>
    [HttpPost("login-with-entra")]
    public async Task<IActionResult> LoginWithEntra([FromBody] EntraTokenRequest request)
    {
        try
        {
            // Validate the Entra ID token
            var principal = await ValidateEntraIdToken(request.AccessToken);
            if (principal == null) return Unauthorized("Invalid Entra ID token");

            // Extract user information from the token
            var email = principal.FindFirst("preferred_username")?.Value ??
                        principal.FindFirst(ClaimTypes.Email)?.Value;
            var name = principal.FindFirst("name")?.Value ?? "";
            var firstName = principal.FindFirst("given_name")?.Value ?? "";
            var lastName = principal.FindFirst("family_name")?.Value ?? "";
            var entraId = principal.FindFirst("oid")?.Value ?? principal.FindFirst("sub")?.Value;

            if (string.IsNullOrEmpty(email)) return BadRequest("Unable to extract email from Entra ID token");

            // Find or create user
            var user = await _userManager.FindByEmailAsync(email);
            if (user == null)
            {
                // Create new user from Entra ID information
                user = new ApplicationUser
                {
                    UserName = email,
                    Email = email,
                    FirstName = firstName,
                    LastName = lastName,
                    EntraId = entraId
                };

                var createResult = await _userManager.CreateAsync(user);
                if (!createResult.Succeeded)
                {
                    _logger.LogError("Failed to create user from Entra ID token for {Email}: {Errors}",
                        email, string.Join(", ", createResult.Errors.Select(e => e.Description)));
                    return BadRequest("Failed to create user account");
                }

                // Assign default role if needed
                await _userManager.AddToRoleAsync(user, "User");
            }
            else
            {
                // Update user information if needed
                if (string.IsNullOrEmpty(user.EntraId))
                {
                    user.EntraId = entraId;
                    await _userManager.UpdateAsync(user);
                }
            }

            // Generate local JWT token
            var roles = await _userManager.GetRolesAsync(user);
            var accessToken = _tokenService.GenerateAccessToken(user, roles);
            var refreshToken = _tokenService.GenerateRefreshToken();

            user.RefreshToken = refreshToken;
            user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
            await _userManager.UpdateAsync(user);

            _logger.LogInformation("User {Email} logged in via Entra ID successfully", email);

            return Ok(new LoginResponse
            {
                AccessToken = accessToken, RefreshToken = refreshToken, ExpiresAt = DateTime.UtcNow.AddMinutes(60)
            });
        }
        catch (Exception ex)
        {
            _logger.LogError(ex, "Error during Entra ID login");
            return BadRequest("Invalid token or authentication failed");
        }
    }

    private async Task<ClaimsPrincipal?> ValidateEntraIdToken(string token)
    {
        var configuration = new ConfigurationBuilder()
            .SetBasePath(Directory.GetCurrentDirectory())
            .AddJsonFile("appsettings.json")
            .Build();

        var azureAd = configuration.GetSection("AzureAd");
        var tenantId = azureAd["TenantId"];
        var audience = azureAd["Audience"] ?? azureAd["ClientId"];

        var tokenHandler = new JwtSecurityTokenHandler();

        // Get the OpenID Connect configuration
        var configurationManager = new ConfigurationManager<OpenIdConnectConfiguration>(
            $"https://login.microsoftonline.com/{tenantId}/v2.0/.well-known/openid_configuration",
            new OpenIdConnectConfigurationRetriever());

        var openIdConfig = await configurationManager.GetConfigurationAsync();

        var validationParameters = new TokenValidationParameters
        {
            ValidateIssuer = true,
            ValidateAudience = true,
            ValidateLifetime = true,
            ValidateIssuerSigningKey = true,
            ValidIssuer = $"https://login.microsoftonline.com/{tenantId}/v2.0",
            ValidAudiences = new[] { audience, $"api://{audience}" },
            IssuerSigningKeys = openIdConfig.SigningKeys
        };

        try
        {
            var principal = tokenHandler.ValidateToken(token, validationParameters, out _);
            return principal;
        }
        catch
        {
            return null;
        }
    }
}

public record RegisterRequest
{
    [Required] [EmailAddress] public string Email { get; init; } = string.Empty;

    [Required]
    [StringLength(100, MinimumLength = 6)]
    public string Password { get; init; } = string.Empty;

    [Required] public string FirstName { get; init; } = string.Empty;

    [Required] public string LastName { get; init; } = string.Empty;
}

public record LoginRequest
{
    [Required] [EmailAddress] public string Email { get; init; } = string.Empty;

    [Required] public string Password { get; init; } = string.Empty;
}

public record RefreshTokenRequest
{
    [Required] public string AccessToken { get; init; } = string.Empty;

    [Required] public string RefreshToken { get; init; } = string.Empty;
}

public record LoginResponse
{
    public string AccessToken { get; init; } = string.Empty;
    public string RefreshToken { get; init; } = string.Empty;
    public DateTime ExpiresAt { get; init; }
}

public record EntraTokenRequest
{
    [Required] public string AccessToken { get; init; } = string.Empty;
}
