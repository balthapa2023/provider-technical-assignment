using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;
using System.Reflection;

namespace ProviderAssignmentStarter.Controllers
{
    /// <summary>
    /// Diagnostic controller for debugging database and audit log issues
    /// </summary>
    public class DiagnosticsController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<DiagnosticsController> _logger;

        public DiagnosticsController(AppDbContext context, ILogger<DiagnosticsController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: Diagnostics/DatabaseStatus - Check database schema and data counts
        /// </summary>
        public async Task<IActionResult> DatabaseStatus()
        {
            try
            {
                var diagnostics = new
                {
                    timestamp = DateTime.UtcNow,
                    database = new
                    {
                        providersCount = await _context.Providers.CountAsync(),
                        licensesCount = await _context.Licenses.CountAsync(),
                        auditLogsCount = await _context.AuditLogs.CountAsync(),
                        providersDeleted = await _context.Providers.IgnoreQueryFilters().Where(p => p.IsDeleted).CountAsync(),
                        licensesDeleted = await _context.Licenses.IgnoreQueryFilters().Where(l => l.IsDeleted).CountAsync(),
                    },
                    latestAuditLogs = await _context.AuditLogs
                        .OrderByDescending(a => a.Timestamp)
                        .Take(10)
                        .Select(a => new
                        {
                            a.AuditLogId,
                            a.EntityType,
                            a.EntityId,
                            a.Action,
                            a.Timestamp,
                            a.UserId,
                            a.Description
                        })
                        .ToListAsync(),
                    connectionString = _context.Database.GetConnectionString(),
                    databaseProvider = _context.Database.ProviderName,
                };

                return Ok(diagnostics);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving diagnostics");
                return StatusCode(500, new { error = ex.Message, stackTrace = ex.StackTrace });
            }
        }

        /// <summary>
        /// GET: Diagnostics/TestAuditLog - Test if we can save to AuditLog table
        /// </summary>
        public IActionResult TestAuditLog()
        {
            try
            {
                _logger.LogInformation("=== STARTING AUDIT LOG TEST ===");

                // Test 1: Check if AuditLogs DbSet exists
                _logger.LogInformation("Test 1: Checking if AuditLogs DbSet exists");
                var auditLogsDbSet = _context.AuditLogs;
                _logger.LogInformation("✓ AuditLogs DbSet exists");

                // Test 2: Try to create an audit log entry
                _logger.LogInformation("Test 2: Creating test audit log entry");
                var testAudit = new AuditLog
                {
                    EntityType = "TEST",
                    EntityId = 999,
                    Action = "Test",
                    UserId = "DiagnosticTest",
                    Timestamp = DateTime.UtcNow,
                    Description = "This is a test entry from diagnostics endpoint"
                };

                _context.AuditLogs.Add(testAudit);
                _logger.LogInformation("✓ Test entry added to context");

                // Test 3: Try to save
                _logger.LogInformation("Test 3: Calling SaveChanges()");
                _context.SaveChanges();
                _logger.LogInformation("✓ SaveChanges() completed successfully");

                // Test 4: Verify it was saved
                _logger.LogInformation("Test 4: Verifying entry was saved");
                var savedCount = _context.AuditLogs.Count(a => a.EntityId == 999);
                _logger.LogInformation($"✓ Found {savedCount} test entries in database");

                return Ok(new
                {
                    success = true,
                    message = "Audit log test completed successfully!",
                    testEntrySaved = savedCount > 0,
                    timestamp = DateTime.UtcNow
                });
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"AUDIT LOG TEST FAILED: {ex.Message}");
                _logger.LogError(ex, $"Stack trace: {ex.StackTrace}");

                return StatusCode(500, new
                {
                    success = false,
                    error = ex.Message,
                    innerError = ex.InnerException?.Message,
                    stackTrace = ex.StackTrace
                });
            }
        }
    }
}
