namespace ProviderAssignmentStarter.Models;

public enum LicenseStatus { Active, Expired, Suspended }

public class License
{
    public int LicenseId { get; set; }
    public int ProviderId { get; set; }
    public string LicenseNumber { get; set; } = string.Empty;
    public LicenseStatus LicenseStatus { get; set; } = LicenseStatus.Active;
    public DateOnly ExpirationDate { get; set; }
    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;

    public Provider Provider { get; set; } = null!;

    /// <summary>Validity is driven by ExpirationDate only — never by provider status.</summary>
    public bool IsExpired(DateOnly today) => ExpirationDate < today;
}
