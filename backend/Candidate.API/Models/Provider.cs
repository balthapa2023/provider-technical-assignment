namespace Candidate.API.Models;

public sealed class Provider
{
    public Guid Id { get; set; }

    public string Name { get; set; } = string.Empty;

    public string County { get; set; } = string.Empty;

    public string Status { get; set; } = ProviderStatuses.Pending;

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAtUtc { get; set; }

    public List<License> Licenses { get; set; } = [];
}
