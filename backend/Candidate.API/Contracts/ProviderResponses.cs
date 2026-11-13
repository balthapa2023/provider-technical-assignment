using Candidate.API.Models;

namespace Candidate.API.Contracts;

public sealed record ProviderListItemResponse(
    Guid Id,
    string Name,
    string County,
    string Status,
    DateTimeOffset CreatedAtUtc,
    int LicenseCount,
    int ActiveLicenseCount,
    int ExpiredLicenseCount);

public sealed record ProviderDetailsResponse(
    Guid Id,
    string Name,
    string County,
    string Status,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? DeletedAtUtc,
    IReadOnlyList<LicenseResponse> Licenses);

public sealed record LicenseResponse(
    Guid Id,
    string Number,
    string Status,
    string StoredStatus,
    DateOnly ExpirationDate,
    DateTimeOffset UpdatedAtUtc,
    bool IsDeleted,
    DateTimeOffset? DeletedAtUtc);

public sealed record DeletedLicenseResponse(
    Guid Id,
    Guid ProviderId,
    string Number,
    string StoredStatus,
    DateOnly ExpirationDate,
    DateTimeOffset CreatedAtUtc,
    DateTimeOffset UpdatedAtUtc,
    DateTimeOffset? DeletedAtUtc);

public static class ProviderResponseMapper
{
    public static ProviderListItemResponse ToListItem(Provider provider, DateOnly today)
    {
        var activeCount = provider.Licenses.Count(license =>
            GetEffectiveLicenseStatus(license, today) == LicenseStatuses.Active);
        var expiredCount = provider.Licenses.Count(license =>
            GetEffectiveLicenseStatus(license, today) == LicenseStatuses.Expired);

        return new ProviderListItemResponse(
            provider.Id,
            provider.Name,
            provider.County,
            provider.Status,
            provider.CreatedAtUtc,
            provider.Licenses.Count,
            activeCount,
            expiredCount);
    }

    public static ProviderDetailsResponse ToDetails(Provider provider, DateOnly today)
    {
        var licenses = provider.Licenses
            .OrderBy(license => license.ExpirationDate)
            .Select(license => new LicenseResponse(
                license.Id,
                license.Number,
                GetEffectiveLicenseStatus(license, today),
                license.Status,
                license.ExpirationDate,
                license.UpdatedAtUtc,
                license.IsDeleted,
                license.DeletedAtUtc))
            .ToList();

        return new ProviderDetailsResponse(
            provider.Id,
            provider.Name,
            provider.County,
            provider.Status,
            provider.CreatedAtUtc,
            provider.UpdatedAtUtc,
            provider.DeletedAtUtc,
            licenses);
    }

    public static string GetEffectiveLicenseStatus(License license, DateOnly today)
    {
        if (license.Status == LicenseStatuses.Suspended)
        {
            return LicenseStatuses.Suspended;
        }

        return license.ExpirationDate < today
            ? LicenseStatuses.Expired
            : LicenseStatuses.Active;
    }
}
