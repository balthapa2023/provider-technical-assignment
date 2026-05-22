using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;
using ProviderAssignmentStarter.Services;
namespace ProviderAssignmentStarter.Controllers
{
    public class ProvidersController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserContextService _userContext;

        public ProvidersController(ApplicationDbContext db, IUserContextService userContext)
        {
            _db = db;
            _userContext = userContext;
        }

        // GET: Providers
        public async Task<IActionResult> Index()
        {
            var providers = await _db.Providers
                .Include(p => p.Licenses)
                .ToListAsync();
            return View(providers);
        }

        // GET: Providers/Create
        public IActionResult Create() => View();

        // POST: Providers/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(Provider model)
        {
            if (!ModelState.IsValid) return View(model);

            model.CreatedAt = DateTime.UtcNow;
            model.CreatedBy = _userContext.GetCurrentUserName();

            _db.Providers.Add(model);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Providers/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var provider = await _db.Providers.FindAsync(id);
            if (provider == null) return NotFound();
            return View(provider);
        }

        // POST: Providers/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, Provider model)
        {
            if (id != model.Id) return BadRequest();
            if (!ModelState.IsValid) return View(model);

            var provider = await _db.Providers.FindAsync(id);
            if (provider == null) return NotFound();

            provider.ProviderName = model.ProviderName;
            provider.County = model.County;
            provider.Status = model.Status;
            provider.UpdatedAt = DateTime.UtcNow;
            provider.UpdatedBy = _userContext.GetCurrentUserName();

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // POST: Providers/Delete/5 (soft delete)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var provider = await _db.Providers
                .IgnoreQueryFilters() // ensure we can find it even if previously deleted (defensive)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (provider == null) return NotFound();

            if (provider.IsDeleted)
            {
                // already deleted
                return RedirectToAction(nameof(Index));
            }

            provider.IsDeleted = true;
            provider.DeletedAt = DateTime.UtcNow;
            provider.DeletedBy = _userContext.GetCurrentUserName();
            provider.UpdatedAt = DateTime.UtcNow;
            provider.UpdatedBy = _userContext.GetCurrentUserName();

            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Providers/Deleted
        public async Task<IActionResult> Deleted()
        {
            var deleted = await _db.Providers
                .IgnoreQueryFilters()
                .Where(p => p.IsDeleted)
                .Include(p => p.Licenses)
                .ToListAsync();
            return View(deleted);
        }
    }
}
