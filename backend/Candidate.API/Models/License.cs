namespace Candidate.API.Models;

public sealed class License
{
    public Guid Id { get; set; }

    public Guid ProviderId { get; set; }

    public Provider Provider { get; set; } = null!;

    public string Number { get; set; } = string.Empty;

    public string Status { get; set; } = LicenseStatuses.Active;

    public DateOnly ExpirationDate { get; set; }

    public DateTimeOffset CreatedAtUtc { get; set; }

    public DateTimeOffset UpdatedAtUtc { get; set; }

    public bool IsDeleted { get; set; }

    public DateTimeOffset? DeletedAtUtc { get; set; }
}
