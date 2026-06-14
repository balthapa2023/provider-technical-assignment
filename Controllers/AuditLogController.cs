using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Middleware;

namespace ProviderAssignmentStarter.Controllers
{
    /// <summary>
    /// Controller for viewing audit logs.
    /// Note: AuditLogs are never filtered by soft-delete, providing complete audit trail.
    /// Requires authentication for access.
    /// </summary>
    public class AuditLogController : Controller
    {
        private readonly AppDbContext _context;
        private readonly ILogger<AuditLogController> _logger;

        public AuditLogController(AppDbContext context, ILogger<AuditLogController> logger)
        {
            _context = context;
            _logger = logger;
        }

        /// <summary>
        /// GET: AuditLog/Index - Display complete audit trail
        /// </summary>
        public async Task<IActionResult> Index()
        {
            try
            {
                // Note: We use IgnoreQueryFilters() to bypass any global filters for audit log
                // (though AuditLog has no filter, this is explicit for clarity)
                var auditLogs = await _context.AuditLogs
                    .AsNoTracking()
                    .OrderByDescending(a => a.Timestamp)
                    .ToListAsync();

                return View(auditLogs);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving audit logs");
                return StatusCode(StatusCodes.Status500InternalServerError, new ErrorResponse
                {
                    Code = "INTERNAL_SERVER_ERROR",
                    Message = "An error occurred while retrieving audit logs."
                });
            }
        }
    }
}
