using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Models;

namespace ProviderAssignmentStarter.Controllers;

public class ProvidersController : Controller
{
    // Gives the controller access to the database.
    private readonly AppDbContext _context;

    // ASP.NET Core injects AppDbContext into the controller.
    public ProvidersController(AppDbContext context)
    {
        _context = context;
    }

    // GET: /Providers
    // Displays all non-deleted providers and their licenses.
    public async Task<IActionResult> Index()
    {
        var providers = await _context.Providers
            .Include(p => p.Licenses)
            .OrderBy(p => p.Name)
            .ToListAsync();

        return View(providers);
    }

    // GET: /Providers/Create
    // Displays the form for creating a provider.
    [HttpGet]
    public IActionResult Create()
    {
        return View();
    }
    // GET: /Providers/Edit/1
    // Displays the existing provider information in the edit form.
    [HttpGet]
    public async Task<IActionResult> Edit(int? id)
    {
        // An ID is required so we know which provider to edit.
        if (id == null)
        {
            return NotFound();
        }

        // Find the provider in the database.
        // The global query filter automatically excludes soft-deleted providers.
        var provider = await _context.Providers.FindAsync(id);

        if (provider == null)
        {
            return NotFound();
        }

        return View(provider);
    }


    // POST: /Providers/Create
    // Receives the submitted form and saves the provider.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Provider provider)
    {
        // Check validation rules from Provider.cs.
        if (ModelState.IsValid)
        {
            // These values are controlled by the application.
            provider.IsDeleted = false;
            provider.CreatedDate = DateTime.UtcNow;

            // Add the new provider to EF Core.
            _context.Providers.Add(provider);

            // Save the provider to SQLite.
            await _context.SaveChangesAsync();

            // Return to the provider listing.
            return RedirectToAction(nameof(Index));
        }

        // If validation fails, show the form again.
        return View(provider);
    }
    // POST: /Providers/Edit/1
    // Receives the edited information and updates the provider.
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(
        int id,
        [Bind("ProviderId,Name,County,Status")] Provider formProvider)
    {
        // Make sure the ID in the URL matches the provider from the form.
        if (id != formProvider.ProviderId)
        {
            return NotFound();
        }

        if (!ModelState.IsValid)
        {
            return View(formProvider);
        }

        // Load the existing provider from the database.
        var provider = await _context.Providers.FindAsync(id);

        if (provider == null)
        {
            return NotFound();
        }

        // Update only the fields the user is allowed to edit.
        provider.Name = formProvider.Name;
        provider.County = formProvider.County;
        provider.Status = formProvider.Status;

        // Save the changes to SQLite.
        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }

    // GET: /Providers/Delete/1
    // Displays a confirmation page before soft-deleting the provider.
    [HttpGet]
    public async Task<IActionResult> Delete(int? id)
    {
        if (id == null)
        {
            return NotFound();
        }

        // Find the provider.
        // The global query filter means already-deleted providers
        // will not be returned here.
        var provider = await _context.Providers
            .FirstOrDefaultAsync(p => p.ProviderId == id);

        if (provider == null)
        {
            return NotFound();
        }

        return View(provider);
    }


    // POST: /Providers/Delete/1
    // Performs a SOFT DELETE.
    // The provider remains in the database.
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var provider = await _context.Providers.FindAsync(id);

        if (provider == null)
        {
            return NotFound();
        }

        // IMPORTANT:
        // Do NOT use _context.Providers.Remove(provider).
        // That would physically delete the database record.

        provider.IsDeleted = true;
        provider.DeletedDate = DateTime.UtcNow;

        await _context.SaveChangesAsync();

        return RedirectToAction(nameof(Index));
    }
}