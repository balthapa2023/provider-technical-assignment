using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;
using ProviderAssignmentStarter.Services;

namespace ProviderAssignmentStarter.Controllers
{
    public class LicensesController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly IUserContextService _userContext;

        public LicensesController(ApplicationDbContext db, IUserContextService userContext)
        {
            _db = db;
            _userContext = userContext;
        }

        public async Task<IActionResult> Index(int providerId)
        {
           
            var provider = await _db.Providers
                .Include(p => p.Licenses)
                .FirstOrDefaultAsync(p => p.Id == providerId);
            if (provider == null)
                return NotFound();
            return View(provider);
        }

        public IActionResult Create(int providerId)
        {
            var model = new ProviderLicense { ProviderId = providerId };
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProviderLicense model)
        {
           
            if (!ModelState.IsValid)

                return View(model);

            _db.Licenses.Add(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", "Licenses", new { providerId = model.ProviderId });
        }

        // POST: Soft delete
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete(int id)
        {
            var license = await _db.Licenses.FindAsync(id);

            if (license == null)
                return NotFound();

            license.IsDeleted = true;               // ✅ Mark as deleted
            license.DeletedAt = DateTime.UtcNow;    // ✅ Timestamp

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { providerId = license.ProviderId });
        }
        public async Task<IActionResult> Edit(int id)
        {
            var license = await _db.Licenses.FindAsync(id);
            if (license == null)
                return NotFound();

            return View(license);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProviderLicense model)
        {
            if (!ModelState.IsValid)
                return View(model);

            _db.Licenses.Update(model);
            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { providerId = model.ProviderId });
        }
        public async Task<IActionResult> Deleted(int providerId)
        {
            var provider = await _db.Providers
                .IgnoreQueryFilters()                     // <-- IMPORTANT
                .Include(p => p.Licenses.Where(l => l.IsDeleted))
                .FirstOrDefaultAsync(p => p.Id == providerId);

            if (provider == null)
                return NotFound();

            return View(provider);
        }

        [HttpPost]
        public async Task<IActionResult> Restore(int id)
        {
            var license = await _db.Licenses
                .IgnoreQueryFilters()
                .FirstOrDefaultAsync(l => l.Id == id);

            if (license == null)
                return NotFound();

            license.IsDeleted = false;
            license.DeletedAt = null;

            await _db.SaveChangesAsync();

            return RedirectToAction("Index", new { providerId = license.ProviderId });
        }
    }
}
