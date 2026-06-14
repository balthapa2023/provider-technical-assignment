// ============ FINAL PROGRAM.CS STRUCTURE ============
// This shows the key sections added for authentication

// Services Section:
// ============ SERVICES ============
builder.Services.AddControllersWithViews();
builder.Services.AddRazorPages();  // ← ADDED: For Identity UI pages

// ============ SESSION ============
builder.Services.AddSession(options =>
{
	options.IdleTimeout = TimeSpan.FromMinutes(30);
	options.Cookie.HttpOnly = true;
	options.Cookie.IsEssential = true;
	options.Cookie.SecurePolicy = CookieSecurePolicy.Always;
});

// ============ AUTHENTICATION CONFIGURATION ============  ← ADDED
builder.Services.ConfigureApplicationCookie(options =>
{
	options.LoginPath = "/Identity/Account/Login";
	options.LogoutPath = "/Identity/Account/Logout";
	options.AccessDeniedPath = "/Identity/Account/AccessDenied";
	options.Cookie.SameSite = SameSiteMode.Strict;
});

// Middleware Section:
// ============ ROUTING ============
app.MapControllerRoute(
	name: "default",
	pattern: "{controller=Home}/{action=Index}/{id?}");

app.MapRazorPages();  // ← ADDED: Maps Razor Pages (Identity UI)

app.Run();

// Controller Change:
// Controllers/AuditLogController.cs
using Microsoft.AspNetCore.Authorization;  // ← ADDED

[Authorize]  // ← ADDED: Protects the entire controller
public class AuditLogController : Controller { }
