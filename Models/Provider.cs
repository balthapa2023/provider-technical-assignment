using System.ComponentModel;

namespace ProviderAssignmentStarter.Models;

public enum ProviderStatus { Pending, Active, Inactive }

public class Provider : ISoftDeletable
{
    public int ProviderId { get; set; }
    public string ProviderName { get; set; } = string.Empty;
    public string County { get; set; } = string.Empty;
    public ProviderStatus Status { get; set; } = ProviderStatus.Pending;

    public DateTime CreatedDate { get; set; } = DateTime.UtcNow;
    public DateTime? ModifiedDate { get; set; }

    public bool IsDeleted { get; set; }
    public DateTime? DeletedDate { get; set; }
    public string? DeletedBy { get; set; }

    public ICollection<License> Licenses { get; set; } = new List<License>();
}
