using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Services
{
    /// <summary>
    /// Service layer for provider-related business logic and queries.
    /// Encapsulates all provider data access patterns including soft-delete handling.
    /// </summary>
    public interface IProviderService
    {
        /// <summary>
        /// Get all active (non-deleted) providers, ordered by name.
        /// Automatically excludes soft-deleted records via EF Core global query filters.
        /// </summary>
        Task<List<Provider>> GetActiveProvidersAsync();

        /// <summary>
        /// Get all soft-deleted providers (IsDeleted = true).
        /// Uses IgnoreQueryFilters() to bypass the default soft-delete filter.
        /// </summary>
        Task<List<Provider>> GetDeletedProvidersAsync();

        /// <summary>
        /// Get all providers (active and deleted).
        /// Uses IgnoreQueryFilters() to retrieve complete dataset.
        /// </summary>
        Task<List<Provider>> GetAllProvidersAsync(bool includeDeleted = false);

        /// <summary>
        /// Get a specific provider by ID (active only; respects soft-delete filter).
        /// Returns null if provider not found or is soft-deleted.
        /// </summary>
        Task<Provider?> GetProviderByIdAsync(int id);

        /// <summary>
        /// Get a specific provider by ID, optionally including soft-deleted records.
        /// Useful for audit/recovery scenarios.
        /// </summary>
        Task<Provider?> GetProviderByIdAsync(int id, bool includeDeleted);

        /// <summary>
        /// Create and save a new provider.
        /// </summary>
        Task<Provider> CreateProviderAsync(Provider provider);

        /// <summary>
        /// Update an existing provider.
        /// </summary>
        Task<Provider> UpdateProviderAsync(Provider provider);

        /// <summary>
        /// Soft-delete a provider (sets IsDeleted = true and DeletedAt = UtcNow).
        /// Does not permanently remove the record.
        /// </summary>
        Task<bool> SoftDeleteProviderAsync(int id);

        /// <summary>
        /// Restore a soft-deleted provider (sets IsDeleted = false and clears DeletedAt).
        /// </summary>
        Task<bool> RestoreProviderAsync(int id);
    }

    /// <summary>
    /// Implementation of IProviderService
    /// </summary>
    public class ProviderService : IProviderService
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProviderService> _logger;

        public ProviderService(AppDbContext context, ILogger<ProviderService> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// Get all active providers, ordered by name.
        /// </summary>
        public async Task<List<Provider>> GetActiveProvidersAsync()
        {
            _logger.LogInformation("Retrieving all active providers");
            return await _context.Providers
                .Include(p => p.Licenses)
                .OrderBy(p => p.ProviderName)
                .ToListAsync();
        }

        /// <summary>
        /// Get all soft-deleted providers.
        /// </summary>
        public async Task<List<Provider>> GetDeletedProvidersAsync()
        {
            _logger.LogInformation("Retrieving all soft-deleted providers");
            return await _context.Providers
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted)
                .Include(p => p.Licenses)
                .OrderByDescending(p => p.DeletedAt)
                .ToListAsync();
        }

        /// <summary>
        /// Get all providers with optional deletion filter.
        /// </summary>
        public async Task<List<Provider>> GetAllProvidersAsync(bool includeDeleted = false)
        {
            IQueryable<Provider> query = _context.Providers;

            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
                _logger.LogInformation("Retrieving all providers (including deleted)");
            }
            else
            {
                _logger.LogInformation("Retrieving all active providers");
            }

            return await query
                .Include(p => p.Licenses)
                .OrderBy(p => p.ProviderName)
                .ToListAsync();
        }

        /// <summary>
        /// Get a specific provider by ID (active only).
        /// </summary>
        public async Task<Provider?> GetProviderByIdAsync(int id)
        {
            return await GetProviderByIdAsync(id, includeDeleted: false);
        }

        /// <summary>
        /// Get a specific provider by ID with soft-delete override.
        /// </summary>
        public async Task<Provider?> GetProviderByIdAsync(int id, bool includeDeleted)
        {
            IQueryable<Provider> query = _context.Providers;

            if (includeDeleted)
            {
                query = query.IgnoreQueryFilters();
            }

            return await query
                .Include(p => p.Licenses)
                .FirstOrDefaultAsync(p => p.ProviderId == id);
        }

        /// <summary>
        /// Create a new provider.
        /// </summary>
        public async Task<Provider> CreateProviderAsync(Provider provider)
        {
            provider.CreatedDate = DateTime.UtcNow;
            provider.IsDeleted = false;
            provider.DeletedAt = null;

            _context.Providers.Add(provider);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Provider {provider.ProviderId} ({provider.ProviderName}) created successfully");
            return provider;
        }

        /// <summary>
        /// Update an existing provider.
        /// </summary>
        public async Task<Provider> UpdateProviderAsync(Provider provider)
        {
            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Provider {provider.ProviderId} ({provider.ProviderName}) updated successfully");
            return provider;
        }

        /// <summary>
        /// Soft-delete a provider.
        /// </summary>
        public async Task<bool> SoftDeleteProviderAsync(int id)
        {
            var provider = await _context.Providers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.ProviderId == id);

            if (provider == null)
            {
                _logger.LogWarning($"Provider {id} not found for soft deletion");
                return false;
            }

            provider.IsDeleted = true;
            provider.DeletedAt = DateTime.UtcNow;

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Provider {id} ({provider.ProviderName}) soft-deleted successfully");
            return true;
        }

        /// <summary>
        /// Restore a soft-deleted provider.
        /// </summary>
        public async Task<bool> RestoreProviderAsync(int id)
        {
            var provider = await _context.Providers
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(p => p.ProviderId == id && p.IsDeleted);

            if (provider == null)
            {
                _logger.LogWarning($"Deleted provider {id} not found for restoration");
                return false;
            }

            provider.IsDeleted = false;
            provider.DeletedAt = null;

            _context.Providers.Update(provider);
            await _context.SaveChangesAsync();

            _logger.LogInformation($"Provider {id} ({provider.ProviderName}) restored successfully");
            return true;
        }
    }
}
