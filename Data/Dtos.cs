using System.ComponentModel.DataAnnotations;

namespace ProviderAssignmentStarter.Models;

public record ProviderUpsertDto(
    [Required, StringLength(200, MinimumLength = 2)] string ProviderName,
    [Required, StringLength(100)] string County,
    [Required, EnumDataType(typeof(ProviderStatus))] string Status);

public record ProviderDto(
    int ProviderId, string ProviderName, string County, string Status,
    DateTime CreatedDate, DateTime? ModifiedDate,
    int LicenseCount, int ActiveLicenseCount, int ExpiredLicenseCount);

public record ProviderDetailDto(
    int ProviderId, string ProviderName, string County, string Status,
    DateTime CreatedDate, DateTime? ModifiedDate,
    bool IsDeleted, DateTime? DeletedDate, string? DeletedBy,
    IEnumerable<LicenseDto> Licenses);

public record LicenseUpsertDto(
    [Required, StringLength(50)] string LicenseNumber,
    [Required, EnumDataType(typeof(LicenseStatus))] string LicenseStatus,
    [Required] DateOnly ExpirationDate);

public record LicenseDto(
    int LicenseId, int ProviderId, string LicenseNumber, string LicenseStatus,
    DateOnly ExpirationDate, bool IsExpired, int DaysUntilExpiration);

public record ProviderLicenseScenarioDto(
    int ProviderId, string ProviderName, string County, string ProviderStatus,
    int LicenseId, string LicenseNumber, string LicenseStatus, DateOnly ExpirationDate, bool IsExpired);

public record DashboardDto(
    int TotalProviders, int ActiveProviders, int DeletedProviders,
    int TotalLicenses, int ActiveLicenses, int ExpiredLicenses, int ExpiringIn30Days,
    IEnumerable<KeyValuePair<string, int>> ProvidersByStatus,
    IEnumerable<KeyValuePair<string, int>> LicensesPerProvider,
    IEnumerable<KeyValuePair<string, int>> ProvidersWithExpiredLicenses);
