using Microsoft.AspNetCore.Identity;

namespace CareSync.API.Identity;

/// <summary>
///     Application user for authentication
/// </summary>
public class ApplicationUser : IdentityUser
{
    public string? FirstName { get; set; }
    public string? LastName { get; set; }
    public string? RefreshToken { get; set; }
    public DateTime RefreshTokenExpiryTime { get; set; }
    public string? EntraId { get; set; }
}
