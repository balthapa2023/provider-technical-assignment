using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;
using ProviderAssignmentStarter.Helpers;
using System.Text.Json;

namespace ProviderAssignmentStarter.Controllers
{
    /// <summary>
    /// Controller for managing Provider CRUD operations with audit logging.
    /// Requires authentication for all operations.
    /// </summary>
    public class ProviderController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<ProviderController> _logger;

        public ProviderController(AppDbContext context, ILogger<ProviderController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: Provider/Index - Display list of providers (excludes soft-deleted)
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                var providers = await _context.Providers.OrderByDescending(p => p.CreatedDate).ToListAsync();
                return View(providers);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving provider list");
                ModelState.AddModelError("", "Error loading providers. Please try again.");
                return View(new List<Provider>());
            }
        }

        /// <summary>
        /// GET: Provider/Details/5 - Display details of a specific provider
        /// </summary>
        public async Task<IActionResult> Details(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var provider = await _context.Providers
                    .Include(p => p.Licenses)
                    .FirstOrDefaultAsync(m => m.ProviderId == id);

                if (provider == null)
                    return NotFound();

                return View(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error retrieving provider details for ID: {id}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// GET: Provider/Create - Display create form
        /// </summary>
        public IActionResult Create()
        {
            try
            {
                ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                return View();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error loading create form");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Provider/Create - Create new provider and log the action
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Provider provider)
        {
            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                    return View(provider);
                }

                provider.CreatedDate = DateTime.UtcNow;
                provider.IsDeleted = false;

                _context.Add(provider);
                await _context.SaveChangesAsync();

                // Log the create action
                LogAudit("Provider", provider.ProviderId, "Create",
                    oldValues: null,
                    newValues: JsonSerializer.Serialize(new { provider.ProviderId, provider.ProviderName, provider.County, provider.Status }));

                _logger.LogInformation($"Provider created: {provider.ProviderName} (ID: {provider.ProviderId})");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, "Database error creating provider");
                ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                ModelState.AddModelError("", "Error creating provider due to database error. Please try again.");
                return View(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error creating provider: {ex.Message}");
                ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                ModelState.AddModelError("", "Error creating provider. Please try again.");
                return View(provider);
            }
        }

        /// <summary>
        /// GET: Provider/Edit/5 - Display edit form
        /// </summary>
        public async Task<IActionResult> Edit(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var provider = await _context.Providers.FindAsync(id);
                if (provider == null)
                    return NotFound();

                ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                return View(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading edit form for provider ID: {id}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Provider/Edit/5 - Update provider and log the action
        /// </summary>
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Provider provider)
        {
            if (id != provider.ProviderId)
                return NotFound();

            try
            {
                if (!ModelState.IsValid)
                {
                    ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                    return View(provider);
                }

                var existingProvider = await _context.Providers.FindAsync(id);
                if (existingProvider == null)
                    return NotFound();

                // Store old values for audit
                var oldValues = JsonSerializer.Serialize(new 
                { 
                    existingProvider.ProviderName, 
                    existingProvider.County, 
                    existingProvider.Status 
                });

                // Update values
                existingProvider.ProviderName = provider.ProviderName;
                existingProvider.County = provider.County;
                existingProvider.Status = provider.Status;

                _context.Update(existingProvider);
                await _context.SaveChangesAsync();

                // Log the edit action
                LogAudit("Provider", existingProvider.ProviderId, "Edit",
                    oldValues: oldValues,
                    newValues: JsonSerializer.Serialize(new 
                    { 
                        existingProvider.ProviderName, 
                        existingProvider.County, 
                        existingProvider.Status 
                    }));

                _logger.LogInformation($"Provider updated: {existingProvider.ProviderName} (ID: {id})");
                return RedirectToAction(nameof(Index));
            }
            catch (DbUpdateConcurrencyException ex)
            {
                _logger.LogError(ex, $"Concurrency error updating provider ID: {id}");
                ModelState.AddModelError("", "The provider was modified by another user. Please reload and try again.");
                ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                return View(provider);
            }
            catch (DbUpdateException ex)
            {
                _logger.LogError(ex, $"Database error updating provider ID: {id}");
                ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                ModelState.AddModelError("", "Error updating provider due to database error. Please try again.");
                return View(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error updating provider ID: {id}");
                ViewData["Counties"] = GeorgiaCounties.ActiveCounties;
                ModelState.AddModelError("", "Error updating provider. Please try again.");
                return View(provider);
            }
        }

        /// <summary>
        /// GET: Provider/Delete/5 - Display delete confirmation
        /// </summary>
        public async Task<IActionResult> Delete(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var provider = await _context.Providers
                    .Include(p => p.Licenses)
                    .FirstOrDefaultAsync(m => m.ProviderId == id);

                if (provider == null)
                    return NotFound();

                return View(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading delete form for provider ID: {id}");
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Provider/Delete/5 - Soft-delete provider and mark related licenses as deleted
        /// </summary>
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                _logger.LogInformation($"DeleteConfirmed called for provider ID: {id}");

                var provider = await _context.Providers
                    .Include(p => p.Licenses)
                    .FirstOrDefaultAsync(p => p.ProviderId == id);

                if (provider == null)
                    return NotFound();

                // Soft-delete provider
                provider.IsDeleted = true;
                provider.DeletedAt = DateTime.UtcNow;

                // Soft-delete related licenses
                var relatedLicenses = provider.Licenses.Where(l => !l.IsDeleted).ToList();
                foreach (var license in relatedLicenses)
                {
                    license.IsDeleted = true;
                    license.DeletedAt = DateTime.UtcNow;
                }

                _context.Update(provider);
                _context.UpdateRange(relatedLicenses);
                await _context.SaveChangesAsync();

                // Log the delete action
                _logger.LogInformation($"About to call LogAudit for provider ID: {id}");
                LogAudit("Provider", provider.ProviderId, "Delete",
                    oldValues: null,
                    newValues: $"Soft-deleted provider and {relatedLicenses.Count} related licenses");

                _logger.LogInformation($"Provider soft-deleted: {provider.ProviderName} (ID: {id}) with {relatedLicenses.Count} licenses");
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error deleting provider ID: {id}");
                ModelState.AddModelError("", "Error deleting provider. Please try again.");
                return RedirectToAction(nameof(Delete), new { id });
            }
        }

        /// <summary>
        /// Log audit trail entry
        /// </summary>
        private void LogAudit(string entityType, int entityId, string action, string? oldValues, string? newValues)
        {
            try
            {
                var audit = new AuditLog
                {
                    EntityType = entityType,
                    EntityId = entityId,
                    Action = action,
                    OldValues = oldValues,
                    NewValues = newValues,
                    UserId = User?.FindFirst("UserId")?.Value ?? "Unknown",
                    Timestamp = DateTime.UtcNow,
                    Description = $"{action} {entityType} {entityId}"
                };

                _context.AuditLogs.Add(audit);
                _context.SaveChanges();
                _logger.LogInformation($"Audit log created: {action} {entityType} ID:{entityId} at {audit.Timestamp:yyyy-MM-dd HH:mm:ss}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error logging audit entry for {action} {entityType} {entityId}: {ex.Message}");
                // Don't throw - audit logging failures shouldn't break the operation
            }
        }

        /// <summary>
        /// GET: Provider/Deleted - Display all soft-deleted providers
        /// </summary>
        public async Task<IActionResult> Deleted()
        {
            try
            {
                var deletedProviders = await _context.Providers
                    .IgnoreQueryFilters()
                    .Where(p => p.IsDeleted)
                    .OrderByDescending(p => p.DeletedAt)
                    .ToListAsync();

                return View(deletedProviders);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving deleted providers");
                TempData["ErrorMessage"] = "An error occurred while retrieving deleted providers.";
                return RedirectToAction(nameof(Index));
            }
        }

        /// <summary>
        /// POST: Provider/Restore/5 - Restore a soft-deleted provider and its licenses
        /// </summary>
        [HttpPost, ActionName("Restore")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> RestoreConfirmed(int id)
        {
            try
            {
                _logger.LogInformation($"RestoreConfirmed called for provider ID: {id}");

                var provider = await _context.Providers
                    .IgnoreQueryFilters()
                    .Include(p => p.Licenses)
                    .FirstOrDefaultAsync(p => p.ProviderId == id);

                if (provider == null)
                    return NotFound();

                // Restore provider
                provider.IsDeleted = false;
                provider.DeletedAt = null;

                // Restore related licenses
                var deletedLicenses = provider.Licenses.Where(l => l.IsDeleted).ToList();
                foreach (var license in deletedLicenses)
                {
                    license.IsDeleted = false;
                    license.DeletedAt = null;
                }

                _context.Update(provider);
                _context.UpdateRange(deletedLicenses);
                await _context.SaveChangesAsync();

                // Log the restore action
                _logger.LogInformation($"About to call LogAudit for provider restore ID: {id}");
                _logger.LogInformation($"Provider state before LogAudit: IsDeleted={provider.IsDeleted}, DeletedAt={provider.DeletedAt}");
                LogAudit("Provider", provider.ProviderId, "Restore",
                    oldValues: "IsDeleted=true",
                    newValues: $"Restored provider and {deletedLicenses.Count} related licenses");
                _logger.LogInformation($"Provider state after LogAudit: IsDeleted={provider.IsDeleted}, DeletedAt={provider.DeletedAt}");

                _logger.LogInformation($"Provider restored: {provider.ProviderName} (ID: {id}) with {deletedLicenses.Count} licenses");
                TempData["SuccessMessage"] = $"Provider '{provider.ProviderName}' has been restored successfully.";
                return RedirectToAction(nameof(Deleted));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error restoring provider ID: {id}");
                TempData["ErrorMessage"] = "Error restoring provider. Please try again.";
                return RedirectToAction(nameof(Deleted));
            }
        }

        /// <summary>
        /// GET: Provider/Restore/5 - Display confirmation page before restoring
        /// </summary>
        public async Task<IActionResult> Restore(int? id)
        {
            try
            {
                if (id == null)
                    return NotFound();

                var provider = await _context.Providers
                    .IgnoreQueryFilters()
                    .Include(p => p.Licenses)
                    .FirstOrDefaultAsync(p => p.ProviderId == id);

                if (provider == null)
                    return NotFound();

                return View(provider);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Error loading restore form for provider ID: {id}");
                return RedirectToAction(nameof(Deleted));
            }
        }
    }
}
