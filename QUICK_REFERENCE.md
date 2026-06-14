# Quick Reference - Authentication Setup

## 🎯 What's Protected
**AuditLog Page** - Requires login
- Route: `/AuditLog/Index`
- Status: ✅ Protected with [Authorize]

## 🎯 What's Public
- Home Page `/Home/Index`
- Provider Page `/Provider/...`
- Any page without [Authorize]

---

## 🔑 Test Account

**Register a new account:**
- Email: `user@example.com`
- Password: `TestUser123!` (must meet requirements)
  - 8+ characters
  - Uppercase letter
  - Lowercase letter
  - Number
  - Special character

---

## 📍 URL Mappings

| Path | Page | Purpose |
|------|------|---------|
| `/Identity/Account/Login` | Login.cshtml | User authentication |
| `/Identity/Account/Register` | Register.cshtml | Create account |
| `/Identity/Account/Logout` | Logout.cshtml | Sign out |
| `/Identity/Account/AccessDenied` | AccessDenied.cshtml | Permission error |

---

## 🔍 How to Access AuditLog

### Before Login
```
https://localhost:7035/AuditLog
		↓
(Redirects to login page)
https://localhost:7035/Identity/Account/Login
```

### After Login
```
https://localhost:7035/AuditLog
		↓
(Shows audit log page)
```

---

## 🛠️ Configuration Locations

| Setting | Location |
|---------|----------|
| Login redirect | Program.cs `ConfigureApplicationCookie()` |
| Authorization | `Controllers/AuditLogController.cs [Authorize]` |
| Password policy | Program.cs `AddIdentity()` options |
| Database | `Data/provider_assignment.db` |
| Pages layout | `Pages/_ViewStart.cshtml` |

---

## ⚡ Common Commands

**Create account via DB (alternative method)**
```sql
INSERT INTO AspNetUsers (Id, UserName, NormalizedUserName, Email, NormalizedEmail, EmailConfirmed, PasswordHash, SecurityStamp, ConcurrencyStamp, PhoneNumberConfirmed, TwoFactorEnabled, LockoutEnabled, AccessFailedCount)
VALUES (
  'guid-here',
  'user@example.com',
  'USER@EXAMPLE.COM',
  'user@example.com',
  'USER@EXAMPLE.COM',
  1,
  'hashedpassword',
  'stamp',
  'stamp',
  0, 0, 1, 0
);
```

**Check logged-in users**
```sql
SELECT * FROM AspNetUsers;
```

**Reset account lockout**
```sql
UPDATE AspNetUsers SET AccessFailedCount = 0, LockoutEnd = NULL WHERE Email = 'user@example.com';
```

---

## ✅ Verification Checklist

Run through these to verify setup:

1. [ ] Start application without errors
2. [ ] Open `https://localhost:7035/AuditLog`
3. [ ] Redirected to login page
4. [ ] Click "Register" link
5. [ ] Create account with strong password
6. [ ] Logged in automatically after registration
7. [ ] Redirected back to `/AuditLog`
8. [ ] Can view audit logs
9. [ ] Click "Logout"
10. [ ] Logged out, redirected to home
11. [ ] Cannot access `/AuditLog` without login
12. [ ] Other pages still public

---

## 📞 Support

**Issue: "Page not found" on login redirect**
- Solution: Rebuild solution (Ctrl+Shift+B)
- Check: `app.MapRazorPages()` in Program.cs

**Issue: Password doesn't meet requirements**
- Solution: Use pattern like `StrongPass123!`
- Check: Uppercase, lowercase, digit, special char present

**Issue: Account locked after 3 failed logins**
- Solution: Wait 15 minutes or reset via SQL
- Check: `LockoutEnd` date in database
