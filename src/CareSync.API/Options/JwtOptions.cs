namespace CareSync.API.Options;

public sealed class JwtOptions
{
    public const string SectionName = "JWT";
    public string? Secret { get; set; }
    public string? ValidIssuer { get; set; }
    public string? ValidAudience { get; set; }
}
