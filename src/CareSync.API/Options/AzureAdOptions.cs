namespace CareSync.API.Options;

public sealed class AzureAdOptions
{
    public const string SectionName = "AzureAd";
    public string? TenantId { get; set; }
    public string? Audience { get; set; }
    public string? ClientId { get; set; }
}
