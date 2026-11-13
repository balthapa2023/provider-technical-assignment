namespace Candidate.API.Contracts;

/// <summary>Read-only registry metrics evaluated against a single UTC date.</summary>
public sealed record DashboardResponse(
    DateOnly AsOfDate,
    int ExpiringWithinDays,
    int ProviderCount,
    int LicenseCount,
    int ExpiredLicenseCount,
    int ProvidersWithExpiringLicensesCount,
    IReadOnlyList<StatusCountResponse> ProvidersByStatus,
    IReadOnlyList<StatusCountResponse> LicensesByStatus,
    IReadOnlyList<ProviderLicenseCountResponse> LicensesPerProvider,
    IReadOnlyList<ExpiringProviderResponse> ProvidersWithExpiringLicenses);

/// <summary>A count of records sharing a status.</summary>
public sealed record StatusCountResponse(string Status, int Count);

/// <summary>The number of non-deleted licenses belonging to a provider.</summary>
public sealed record ProviderLicenseCountResponse(Guid ProviderId, string ProviderName, int Count);

/// <summary>A provider with currently active licenses approaching expiration.</summary>
public sealed record ExpiringProviderResponse(
    Guid ProviderId,
    string ProviderName,
    string County,
    int ExpiringLicenseCount,
    DateOnly NextExpirationDate);
