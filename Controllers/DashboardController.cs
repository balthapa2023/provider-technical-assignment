using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Middleware;

namespace ProviderAssignmentStarter.Controllers
{
    /// <summary>
    /// API Controller for dashboard metrics and statistics.
    /// </summary>
    [ApiController]
    [Route("api/[controller]")]
    public class DashboardController : ControllerBase
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DashboardController> _logger;

        public DashboardController(AppDbContext context, ILogger<DashboardController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get provider counts grouped by status (Active, Inactive, Pending) - includes soft-deleted with black color for deleted
        /// </summary>
        [HttpGet("providers-by-status")]
        public async Task<ActionResult<object>> GetProvidersByStatus()
        {
            try
            {
                var data = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted records
                    .GroupBy(p => new { p.Status, p.IsDeleted })
                    .Select(g => new
                    {
                        status = g.Key.Status,
                        isDeleted = g.Key.IsDeleted,
                        count = g.Count()
                    })
                    .OrderBy(x => x.status)
                    .ThenBy(x => x.isDeleted)
                    .ToListAsync();

                // Define colors: Active (green), Inactive (grey), Pending (orange), Deleted variants (black)
                var statusColorMap = new Dictionary<string, Dictionary<bool, (string bg, string border)>>
                {
                    { "Active", new Dictionary<bool, (string, string)> { { false, ("#28a745", "#1e7e34") }, { true, ("#000000", "#1a1a1a") } } },
                    { "Inactive", new Dictionary<bool, (string, string)> { { false, ("#6c757d", "#5a6268") }, { true, ("#000000", "#1a1a1a") } } },
                    { "Pending", new Dictionary<bool, (string, string)> { { false, ("#ffc107", "#e0a800") }, { true, ("#000000", "#1a1a1a") } } }
                };

                var labels = data.Select(x => $"{x.status}{(x.isDeleted ? " (Deleted)" : "")}").ToList();
                var backgroundColors = data.Select(x => statusColorMap[x.status][x.isDeleted].bg).ToList();
                var borderColors = data.Select(x => statusColorMap[x.status][x.isDeleted].border).ToList();

                return Ok(new
                {
                    labels = labels,
                    datasets = new[]
                    {
                        new
                        {
                            label = "Providers by Status",
                            data = data.Select(x => x.count).ToList(),
                            backgroundColor = backgroundColors.Cast<object>().ToArray(),
                            borderColor = borderColors.Cast<object>().ToArray(),
                            borderWidth = 1
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving providers by status");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occurred while retrieving provider data."
                });
            }
        }

        /// <summary>
        /// Get number of licenses per provider (top 10 providers) - includes soft-deleted with black color for deleted
        /// </summary>
        [HttpGet("licenses-per-provider")]
        public async Task<ActionResult<object>> GetLicensesPerProvider()
        {
            try
            {
                var data = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted providers
                    .Select(p => new
                    {
                        providerName = p.ProviderName,
                        isDeleted = p.IsDeleted,
                        licenseCount = p.Licenses.Count()
                    })
                    .OrderByDescending(x => x.licenseCount)
                    .Take(10)
                    .ToListAsync();

                var labels = data.Select(x => $"{x.providerName}{(x.isDeleted ? " (Deleted)" : "")}").ToList();
                var backgroundColors = data.Select(x => x.isDeleted ? "#000000" : "#007bff").Cast<object>().ToArray();
                var borderColors = data.Select(x => x.isDeleted ? "#1a1a1a" : "#0056b3").Cast<object>().ToArray();

                return Ok(new
                {
                    labels = labels,
                    datasets = new[]
                    {
                        new
                        {
                            label = "Licenses per Provider",
                            data = data.Select(x => x.licenseCount).ToList(),
                            backgroundColor = backgroundColors,
                            borderColor = borderColors,
                            borderWidth = 1
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving licenses per provider");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occurred while retrieving license data."
                });
            }
        }

        /// <summary>
        /// Get count of expired vs active licenses - excludes soft-deleted
        /// </summary>
        [HttpGet("license-status")]
        public async Task<ActionResult<object>> GetLicenseStatus()
        {
            try
            {
                var today = DateTime.UtcNow.Date;

                var activeLicenses = await _context.Licenses
                    .Where(l => l.ExpirationDate >= today)
                    .CountAsync();

                var expiredLicenses = await _context.Licenses
                    .Where(l => l.ExpirationDate < today)
                    .CountAsync();

                return Ok(new
                {
                    labels = new[] { "Active", "Expired" },
                    datasets = new[]
                    {
                        new
                        {
                            label = "License Status",
                            data = new[] { activeLicenses, expiredLicenses },
                            backgroundColor = new[] { "#28a745", "#dc3545" },
                            borderColor = new[] { "#1e7e34", "#bd2130" },
                            borderWidth = 1
                        }
                    }
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving license status");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occurred while retrieving license status."
                });
            }
        }

        /// <summary>
        /// Get summary metrics - includes soft-deleted records
        /// </summary>
        [HttpGet("summary")]
        public async Task<ActionResult<object>> GetSummary()
        {
            try
            {
                var totalProviders = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted
                    .CountAsync();

                var totalLicenses = await _context.Licenses
                    .IgnoreQueryFilters() // Include soft-deleted
                    .CountAsync();

                var today = DateTime.UtcNow.Date;
                var expiringIn30Days = await _context.Licenses
                    .IgnoreQueryFilters() // Include soft-deleted
                    .Where(l => l.ExpirationDate >= today && l.ExpirationDate <= today.AddDays(30))
                    .CountAsync();

                var expiredLicenses = await _context.Licenses
                    .IgnoreQueryFilters() // Include soft-deleted
                    .Where(l => l.ExpirationDate < today)
                    .CountAsync();

                var deletedProviders = await _context.Providers
                    .IgnoreQueryFilters()
                    .Where(p => p.IsDeleted)
                    .CountAsync();

                var deletedLicenses = await _context.Licenses
                    .IgnoreQueryFilters()
                    .Where(l => l.IsDeleted)
                    .CountAsync();

                return Ok(new
                {
                    totalProviders,
                    totalLicenses,
                    expiringIn30Days,
                    expiredLicenses,
                    deletedProviders,
                    deletedLicenses
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving summary metrics");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occurred while retrieving summary data."
                });
            }
        }

        /// <summary>
        /// Get providers with licenses expiring within 30 days - includes soft-deleted
        /// </summary>
        [HttpGet("providers-expiring-soon")]
        public async Task<ActionResult<object>> GetProvidersExpiringSoon()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var thirtyDaysFromNow = today.AddDays(30);

                var data = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted
                    .Select(p => new
                    {
                        providerName = p.ProviderName,
                        county = p.County,
                        status = p.Status,
                        isDeleted = p.IsDeleted,
                        expiringLicenseCount = p.Licenses
                            .Where(l => l.ExpirationDate >= today && l.ExpirationDate <= thirtyDaysFromNow)
                            .Count()
                    })
                    .Where(x => x.expiringLicenseCount > 0)
                    .OrderByDescending(x => x.expiringLicenseCount)
                    .ToListAsync();

                return Ok(data);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving providers with expiring licenses");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occurred while retrieving providers with expiring licenses."
                });
            }
        }

        /// <summary>
        /// Get all dashboard data in a single call - includes soft-deleted records with black color for deleted
        /// </summary>
        [HttpGet("all")]
        public async Task<ActionResult<object>> GetAllDashboardData()
        {
            try
            {
                var today = DateTime.UtcNow.Date;
                var thirtyDaysFromNow = today.AddDays(30);

                // Providers by status - ALL providers including deleted with color coding
                var providersByStatus = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted
                    .GroupBy(p => new { p.Status, p.IsDeleted })
                    .Select(g => new
                    {
                        status = g.Key.Status,
                        isDeleted = g.Key.IsDeleted,
                        count = g.Count()
                    })
                    .OrderBy(x => x.status)
                    .ThenBy(x => x.isDeleted)
                    .ToListAsync();

                // Licenses per provider - ALL licenses including deleted with color coding
                var licensesPerProvider = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted
                    .Select(p => new
                    {
                        providerName = p.ProviderName,
                        isDeleted = p.IsDeleted,
                        licenseCount = p.Licenses.Count()
                    })
                    .OrderByDescending(x => x.licenseCount)
                    .Take(10)
                    .ToListAsync();

                // License status - only active (non-deleted) licenses
                var activeLicenses = await _context.Licenses
                    .Where(l => l.ExpirationDate >= today)
                    .CountAsync();

                var expiredLicenses = await _context.Licenses
                    .Where(l => l.ExpirationDate < today)
                    .CountAsync();

                // Summary metrics - ALL records including deleted
                var totalProviders = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted
                    .CountAsync();

                var totalLicenses = await _context.Licenses
                    .IgnoreQueryFilters() // Include soft-deleted
                    .CountAsync();

                var expiringIn30Days = await _context.Licenses
                    .IgnoreQueryFilters() // Include soft-deleted
                    .Where(l => l.ExpirationDate >= today && l.ExpirationDate <= thirtyDaysFromNow)
                    .CountAsync();

                var deletedProviders = await _context.Providers
                    .IgnoreQueryFilters()
                    .Where(p => p.IsDeleted)
                    .CountAsync();

                var deletedLicenses = await _context.Licenses
                    .IgnoreQueryFilters()
                    .Where(l => l.IsDeleted)
                    .CountAsync();

                // Providers expiring soon - ALL providers including deleted
                var providersExpiringSoon = await _context.Providers
                    .IgnoreQueryFilters() // Include soft-deleted
                    .Select(p => new
                    {
                        providerName = p.ProviderName,
                        county = p.County,
                        status = p.Status,
                        isDeleted = p.IsDeleted,
                        expiringLicenseCount = p.Licenses
                            .Where(l => l.ExpirationDate >= today && l.ExpirationDate <= thirtyDaysFromNow)
                            .Count()
                    })
                    .Where(x => x.expiringLicenseCount > 0)
                    .OrderByDescending(x => x.expiringLicenseCount)
                    .ToListAsync();

                // Define colors for providers by status
                var statusColorMap = new Dictionary<string, Dictionary<bool, (string bg, string border)>>
                {
                    { "Active", new Dictionary<bool, (string, string)> { { false, ("#28a745", "#1e7e34") }, { true, ("#000000", "#1a1a1a") } } },
                    { "Inactive", new Dictionary<bool, (string, string)> { { false, ("#6c757d", "#5a6268") }, { true, ("#000000", "#1a1a1a") } } },
                    { "Pending", new Dictionary<bool, (string, string)> { { false, ("#ffc107", "#e0a800") }, { true, ("#000000", "#1a1a1a") } } }
                };

                var providerStatusLabels = providersByStatus.Select(x => $"{x.status}{(x.isDeleted ? " (Deleted)" : "")}").ToList();
                var providerStatusBackgroundColors = providersByStatus.Select(x => statusColorMap[x.status][x.isDeleted].bg).ToList();
                var providerStatusBorderColors = providersByStatus.Select(x => statusColorMap[x.status][x.isDeleted].border).ToList();

                var licenseLabels = licensesPerProvider.Select(x => $"{x.providerName}{(x.isDeleted ? " (Deleted)" : "")}").ToList();
                var licenseBackgroundColors = licensesPerProvider.Select(x => x.isDeleted ? "#000000" : "#007bff").ToList();
                var licenseBorderColors = licensesPerProvider.Select(x => x.isDeleted ? "#1a1a1a" : "#0056b3").ToList();

                return Ok(new
                {
                    providersByStatus = new
                    {
                        labels = providerStatusLabels,
                        datasets = new[]
                        {
                            new
                            {
                                label = "Providers by Status",
                                data = providersByStatus.Select(x => x.count).ToList(),
                                backgroundColor = providerStatusBackgroundColors.Cast<object>().ToArray(),
                                borderColor = providerStatusBorderColors.Cast<object>().ToArray(),
                                borderWidth = 1
                            }
                        }
                    },
                    licensesPerProvider = new
                    {
                        labels = licenseLabels,
                        datasets = new[]
                        {
                            new
                            {
                                label = "Licenses per Provider",
                                data = licensesPerProvider.Select(x => x.licenseCount).ToList(),
                                backgroundColor = licenseBackgroundColors.Cast<object>().ToArray(),
                                borderColor = licenseBorderColors.Cast<object>().ToArray(),
                                borderWidth = 1
                            }
                        }
                    },
                    licenseStatus = new
                    {
                        labels = new[] { "Active", "Expired" },
                        datasets = new[]
                        {
                            new
                            {
                                label = "License Status",
                                data = new[] { activeLicenses, expiredLicenses },
                                backgroundColor = new[] { "#28a745", "#dc3545" },
                                borderColor = new[] { "#1e7e34", "#bd2130" },
                                borderWidth = 1
                            }
                        }
                    },
                    summary = new
                    {
                        totalProviders,
                        totalLicenses,
                        expiringIn30Days,
                        expiredLicenses,
                        deletedProviders,
                        deletedLicenses
                    },
                    providersExpiringSoon
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving all dashboard data");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occurred while retrieving dashboard data."
                });
            }
        }

        /// <summary>
        /// Health check endpoint for debugging (requires authentication)
        /// </summary>
        [HttpGet("health")]
        public async Task<ActionResult<object>> Health()
        {
            try
            {
                var providerCount = await _context.Providers.CountAsync();
                var licenseCount = await _context.Licenses.CountAsync();

                return Ok(new
                {
                    status = "healthy",
                    timestamp = DateTime.UtcNow,
                    providerCount,
                    licenseCount,
                    user = "System"
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Health check failed");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "Health check failed."
                });
            }
        }
    }
}
