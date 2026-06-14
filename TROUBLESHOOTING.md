# Authentication Troubleshooting Guide

## Issue 1: "Page not found" when accessing `/AuditLog`

**Expected Behavior**: Should redirect to `/Identity/Account/Login`

**Solutions**:
- Verify `app.MapRazorPages()` is in Program.cs AFTER `app.MapControllerRoute()`
- Check that `[Authorize]` attribute is on AuditLogController
- Ensure database migrations completed successfully

---

## Issue 2: Login page shows but won't accept credentials

**Possible Causes**:
- Users table not properly initialized
- Password not meeting complexity requirements (8+ chars, uppercase, lowercase, digit, special)
- Account locked after 3 failed attempts

**Solutions**:
1. Register a new user first
2. Wait 15 minutes if locked out, or delete user from database and re-register
3. Use strong password: `TestUser123!`

---

## Issue 3: Users already exist from seeding

**Check database**:
```sql
SELECT * FROM AspNetUsers;
```

**Default seeded users**: Check `Data/DbInitializer.cs` for any created users

---

## Issue 4: Other pages (Home, Provider) are also locked

**This is expected if you added [Authorize] globally**

**To fix**: Remove [Authorize] from other controllers or add it only to AuditLogController

---

## Issue 5: Layout/CSS not showing on Identity pages

**Solutions**:
- Verify `Pages/_ViewStart.cshtml` references correct layout
- Check `Views/Shared/_Layout.cshtml` exists
- Browser cache: Use Ctrl+Shift+Delete to clear

---

## Issue 6: "Identity.Account.Login" route not found

**Solutions**:
- Add `builder.Services.AddRazorPages();` before building app
- Add `app.MapRazorPages();` to routing section
- Verify folder structure: `Pages/Identity/Account/`

---

## Database Reset (if needed)

```csharp
// Delete existing database files
File.Delete("Data/provider_assignment.db");
File.Delete("Data/provider_assignment.db-shm");
File.Delete("Data/provider_assignment.db-wal");

// Restart application - migrations will recreate DB
```

---

## Testing Checklist

- [ ] Can navigate to `/AuditLog` (redirects to login)
- [ ] Can register new account
- [ ] Can login with registered account
- [ ] Can access `/AuditLog` after login
- [ ] Login persists across page navigation
- [ ] Logout clears authentication
- [ ] Home page is still public (no [Authorize])
- [ ] Failed login shows error message
