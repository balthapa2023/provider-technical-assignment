using Microsoft.EntityFrameworkCore;
using ProviderAssignmentStarter.Data;
using ProviderAssignmentStarter.Services;

var builder = WebApplication.CreateBuilder(args);

// Add services
builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

// Simple user context service for CreatedBy/DeletedBy fields
builder.Services.AddScoped<IUserContextService, UserContextService>();

var app = builder.Build();

// Ensure Data folder exists
var dataDir = Path.Combine(app.Environment.ContentRootPath, "Data");
if (!Directory.Exists(dataDir)) Directory.CreateDirectory(dataDir);

// Apply migrations at startup (for dev convenience)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

// Configure middleware
if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}
app.UseHttpsRedirection();
app.UseStaticFiles();
app.UseRouting();
app.UseAuthorization();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Providers}/{action=Index}/{id?}");

app.Run();
