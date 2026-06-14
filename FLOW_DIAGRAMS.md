# Authentication Flow Diagrams

## 1. Overall Architecture

```
┌──────────────────────────────────────────────────────────────┐
│                    ASP.NET Core Application                   │
├──────────────────────────────────────────────────────────────┤
│                                                                │
│  HTTP Request Pipeline:                                        │
│  ┌─────────────────────────────────────────────────────────┐ │
│  │ 1. HTTPS Redirection                                    │ │
│  │ 2. Static Files                                         │ │
│  │ 3. Session                                              │ │
│  │ 4. Routing                                              │ │
│  │ 5. Authentication ← Reads cookie, validates signature   │ │
│  │ 6. Authorization ← Checks [Authorize] attributes        │ │
│  │ 7. Endpoint (MVC/Razor Pages)                           │ │
│  └─────────────────────────────────────────────────────────┘ │
│                                                                │
│  Route Handlers:                                               │
│  ├─ MVC Controllers (Traditional)                              │
│  │  ├─ HomeController (public)                                │
│  │  ├─ ProviderController (public)                            │
│  │  └─ AuditLogController (protected)                         │
│  │                                                             │
│  └─ Razor Pages (Modern)                                       │
│     └─ Identity/Account/                                       │
│        ├─ Login                                                │
│        ├─ Register                                             │
│        ├─ Logout                                               │
│        └─ AccessDenied                                         │
│                                                                │
│  Services Layer:                                               │
│  ├─ UserManager<IdentityUser>                                 │
│  ├─ SignInManager<IdentityUser>                               │
│  ├─ RoleManager<IdentityRole>                                 │
│  └─ DbContext                                                  │
│                                                                │
│  Data Layer:                                                   │
│  ├─ Entity Framework Core                                      │
│  └─ SQLite Database                                            │
│     ├─ AspNetUsers                                             │
│     ├─ AspNetRoles                                             │
│     └─ ... other tables ...                                    │
│                                                                │
└──────────────────────────────────────────────────────────────┘
```

---

## 2. Authentication Decision Tree

```
User Requests Protected Route (/AuditLog)
│
├─ Request contains authentication cookie?
│  ├─ NO ──→ No User.Identity
│  │         │
│  │         └─→ [Authorize] Check
│  │             │
│  │             └─→ NOT AUTHENTICATED
│  │                 │
│  │                 └─→ Redirect to LoginPath (/Identity/Account/Login)
│  │
│  └─ YES ──→ Validate cookie signature & expiration
│             │
│             ├─ INVALID ──→ Cookie discarded
│             │              │
│             │              └─→ [Authorize] Check → NOT AUTHENTICATED
│             │                  │
│             │                  └─→ Redirect to LoginPath
│             │
│             └─ VALID ──→ Extract ClaimsPrincipal from cookie
│                          │
│                          ├─→ User.Identity.IsAuthenticated = true
│                          │
│                          └─→ [Authorize] Check
│                              │
│                              └─→ AUTHENTICATED
│                                  │
│                                  └─→ Allow request to proceed
│                                      │
│                                      └─→ Execute controller action
│                                          │
│                                          └─→ Return resource
```

---

## 3. Registration Flow

```
User Navigates to /Identity/Account/Register
│
└─ GET /Identity/Account/Register
   │
   └─→ RegisterModel.OnGetAsync()
	   │
	   └─→ Display registration form
		   │
		   └─→ User enters:
			   ├─ Email
			   ├─ Password
			   └─ Confirm Password

User Submits Form (POST)
│
└─ POST /Identity/Account/Register
   │
   ├─→ RegisterModel.OnPostAsync()
   │
   ├─→ ModelState validation (required fields, password match)
   │
   ├─→ new IdentityUser { Email, UserName }
   │
   ├─→ UserManager.CreateAsync(user, password)
   │   │
   │   ├─ Validate password policy (8+, upper, lower, digit, special)
   │   │
   │   ├─ Hash password using PBKDF2
   │   │
   │   └─ INSERT INTO AspNetUsers
   │
   ├─→ SignInManager.SignInAsync(user)
   │   │
   │   ├─ Create cookie with user claims
   │   │
   │   ├─ Encrypt and sign cookie
   │   │
   │   └─ Set cookie in response
   │
   └─→ Redirect to ReturnUrl (typically /AuditLog)
	   │
	   └─→ User now authenticated and accessing protected resource
```

---

## 4. Login Flow

```
User Navigates to /Identity/Account/Login
│
└─ GET /Identity/Account/Login
   │
   └─→ LoginModel.OnGetAsync()
	   │
	   └─→ Display login form
		   │
		   └─→ User enters:
			   ├─ Email
			   └─ Password

User Submits Credentials (POST)
│
└─ POST /Identity/Account/Login
   │
   ├─→ LoginModel.OnPostAsync()
   │
   ├─→ UserManager.FindByEmailAsync(email)
   │   │
   │   └─→ SELECT * FROM AspNetUsers WHERE Email = ?
   │
   ├─→ Check user found?
   │   │
   │   ├─ NO ──→ Invalid login attempt
   │   │         │
   │   │         └─→ Show error → Redisplay form
   │   │
   │   └─ YES → Continue
   │
   ├─→ SignInManager.PasswordSignInAsync(email, password, rememberMe, lockoutOnFailure)
   │   │
   │   ├─→ Check Account locked? (LockoutEnd > DateTime.Now)
   │   │   │
   │   │   ├─ YES ──→ Return IsLockedOut
   │   │   │          │
   │   │   │          └─→ Show "Account locked" error
   │   │   │
   │   │   └─ NO → Continue to password check
   │   │
   │   ├─→ Verify password against stored hash
   │   │   │
   │   │   ├─ INVALID ──→ Increment AccessFailedCount
   │   │   │              │
   │   │   │              ├─→ Count = 3? (limit reached)
   │   │   │              │   │
   │   │   │              │   └─→ Set LockoutEnd = now + 15 min
   │   │   │              │
   │   │   │              └─→ Show invalid password error
   │   │   │
   │   │   └─ VALID ──→ Reset AccessFailedCount = 0
   │   │               │
   │   │               └─→ Create authentication cookie
   │   │                   │
   │   │                   ├─ Generate ClaimsPrincipal
   │   │                   │
   │   │                   ├─ Encrypt cookie
   │   │                   │
   │   │                   ├─ Set-Cookie header
   │   │                   │
   │   │                   └─ Succeeded = true
   │
   └─→ if (result.Succeeded)
	   │
	   └─→ Redirect to ReturnUrl (/AuditLog)
		   │
		   └─→ Next request includes cookie
			   │
			   └─→ Cookie valid → User authenticated
```

---

## 5. Access Denied Flow

```
Authenticated User Without Required Permission
│
└─ Request to [Authorize(Roles = "Admin")] action
   │
   ├─ [Authorize] check
   │   │
   │   ├─ User.Identity.IsAuthenticated = true ✓
   │   │
   │   └─ User has required role "Admin"?
   │       │
   │       └─ NO
   │           │
   │           └─ AuthorizeFailed
   │               │
   │               └─→ ChallengeScheme triggered
   │                   │
   │                   └─→ Redirect to AccessDeniedPath
   │
   └─ GET /Identity/Account/AccessDenied
	   │
	   └─→ Policy.OnRedirectToAccessDenied handler
		   │
		   └─→ Display AccessDenied.cshtml
			   │
			   └─→ Show "You don't have permission" message
```

---

## 6. Logout Flow

```
Authenticated User Clicks Logout
│
└─ User clicks logout in navbar (in _LoginPartial.cshtml)
   │
   └─ POST /Identity/Account/Logout
	   │
	   └─→ LogoutModel.OnPost()
		   │
		   ├─→ SignInManager.SignOutAsync()
		   │   │
		   │   ├─ Clear authentication claim
		   │   │
		   │   ├─ Invalidate session
		   │   │
		   │   ├─ Clear cookie
		   │   │
		   │   └─ Response.Cookies.Delete("...") (encrypted cookie)
		   │
		   ├─→ Logger.LogInformation("User logged out")
		   │
		   └─→ Redirect to returnUrl (or /Home/Index)
			   │
			   └─→ Next request has NO cookie
				   │
				   └─→ User.Identity.IsAuthenticated = false
					   │
					   └─→ Accessing /AuditLog redirects to login
```

---

## 7. Account Lockout Mechanism

```
User Failed Login Attempts Timeline
│
├─ Attempt 1: Wrong password
│  │
│  ├─→ AccessFailedCount = 1
│  └─→ LockoutEnd = NULL (not locked)
│
├─ Attempt 2: Wrong password (within any timeframe)
│  │
│  ├─→ AccessFailedCount = 2
│  └─→ LockoutEnd = NULL (still not locked)
│
├─ Attempt 3: Wrong password (LIMIT REACHED)
│  │
│  ├─→ AccessFailedCount = 3 ← THRESHOLD HIT
│  │
│  ├─→ LockoutEnd = DateTime.Now.AddMinutes(15)
│  │
│  └─→ UPDATE AspNetUsers SET AccessFailedCount = 3, LockoutEnd = {timestamp}
│
├─ Meanwhile: 0-15 minutes pass
│  │
│  └─→ User attempts login
│      │
│      ├─→ Check: LockoutEnd > DateTime.Now?
│      │   │
│      │   └─→ YES (still in lockout window)
│      │
│      └─→ Reject login: "Account is locked"
│
└─ After: 15 minutes have passed
   │
   └─→ User attempts login again
	   │
	   ├─→ Check: LockoutEnd > DateTime.Now?
	   │   │
	   │   └─→ NO (lockout period expired)
	   │
	   ├─→ Allow login attempt
	   │
	   ├─→ If correct password:
	   │   │
	   │   ├─→ AccessFailedCount = 0 (reset)
	   │   │
	   │   ├─→ LockoutEnd = NULL
	   │   │
	   │   └─→ User successfully logged in
	   │
	   └─→ If wrong password again:
		   │
		   ├─→ AccessFailedCount = 1 (resets from 3)
		   │
		   └─→ LockoutEnd = NULL
```

---

## 8. Cookie Lifecycle

```
Registration/Login
│
├─ User successfully authenticated
│  │
│  └─→ Create ClaimsPrincipal with user claims
│      │
│      ├─ User ID
│      ├─ Email
│      ├─ Roles (if any)
│      └─ Custom claims
│
├─→ Ticket.Principal = ClaimsPrincipal
│
├─→ Authentication handler encrypts the ticket
│   │
│   └─→ Serialize + Encrypt (AES) + Sign (HMAC)
│
├─→ Create response cookie
│   │
│   ├─ Name: .AspNetCore.Identity.Application
│   ├─ Value: [encrypted data]
│   ├─ HttpOnly: true (no JavaScript access)
│   ├─ Secure: true (HTTPS only)
│   ├─ SameSite: Strict (no cross-site)
│   └─ Expires: DateTime.UtcNow.AddMinutes(20)
│
└─→ Set-Cookie header in response
	│
	└─ Browser stores cookie


Subsequent Requests
│
├─ Browser includes cookie in request
│  │
│  ├─ Cookie header sent with every request
│  └─ Only sent to same domain (HTTPS)
│
├─→ Authentication middleware intercepts
│   │
│   ├─→ Extract encrypted cookie value
│   │
│   ├─→ Validate signature (HMAC check)
│   │
│   ├─→ If invalid: Reject, clear cookie
│   │
│   └─→ If valid: Decrypt to get ClaimsPrincipal
│
├─→ User.Identity set from ClaimsPrincipal
│  │
│  ├─ User.Identity.Name = Email
│  ├─ User.Identity.IsAuthenticated = true
│  ├─ User.IsInRole("Admin") = [check claims]
│  └─ User.FindFirst(ClaimTypes.NameIdentifier) = UserID
│
└─→ Request proceeds with authenticated user context


Logout
│
├─ SignOutAsync() called
│  │
│  ├─→ Response.Cookies.Delete() with same cookie name
│  │
│  └─→ Set-Cookie header: {name}=; expires={past date}
│
└─→ Browser deletes cookie
   │
   └─→ Next request has no cookie
	   │
	   └─→ User.Identity.IsAuthenticated = false
```

---

## 9. Password Hashing Process

```
User Registration with Password "TestUser123!"
│
├─ Password received over HTTPS
│
├─→ UserManager.CreateAsync(user, password)
│   │
│   ├─→ IPasswordHasher<IdentityUser>.HashPassword(user, password)
│   │
│   ├─→ Generate random salt (128 bits)
│   │
│   ├─→ PBKDF2-SHA256 iteration (10,000 iterations by default)
│   │   │
│   │   └─ Input: password + salt
│   │      Output: 256-bit hash
│   │
│   ├─→ Combine version + algorithm + iteration count + salt + hash
│   │
│   └─→ Return base64 encoded result
│       │
│       Example: AQAAAAEAAYAoAAAAEKx77oxxxxxxxxxxxxxxxxxxxxxxxxxx
│
├─→ Store in database
│  │
│  └─ user.PasswordHash = "AQAAAAEAAYAoAAAAEKx77oxxxxxxxxxxxxxxxxxxxxxxxxxx"
│      INSERT INTO AspNetUsers (Id, Email, PasswordHash)
│
└─→ Never store plaintext password ✓


User Login with Password "TestUser123!"
│
├─ Password received over HTTPS
│
├─→ SignInManager.PasswordSignInAsync(email, password, ...)
│   │
│   ├─→ Find user by email in database
│   │
│   ├─→ Get user.PasswordHash from database
│   │
│   ├─→ IPasswordHasher.VerifyHashedPassword(user, storedHash, providedPassword)
│   │   │
│   │   ├─→ Extract salt from storedHash
│   │   │
│   │   ├─→ Apply PBKDF2-SHA256 with same iterations to provided password
│   │   │
│   │   ├─→ Compare computed hash with extracted hash (timing-safe comparison)
│   │   │
│   │   └─→ Return SuccessRehashNeeded or Failure
│   │
│   └─→ If match: Password verified ✓
	   │
	   └─→ Create authentication cookie


Password Comparison (Timing-Safe)
│
├─ Prevention: Timing attacks
│  │
│  └─ Attacker tries to infer password by measuring response time
│
├─→ Use constant-time comparison
│   │
│   ├─ Compares EVERY byte, even after mismatch found
│   ├─ Takes same time regardless of where strings differ
│   └─ Prevents information leakage via timing
│
└─→ No plaintext password ever used in comparison ✓
```

---

## 10. Security Headers Applied

```
HTTP Response Headers Set by ASP.NET Core + Program.cs
│
├─ app.UseHttpsRedirection()
│  │
│  └─→ HLS Strict-Transport-Security: max-age=31536000
│      │
│      └─ Enforce HTTPS for all future requests
│
├─ Middleware in Program.cs
│  │
│  ├─→ X-Content-Type-Options: nosniff
│  │   │
│  │   └─ Prevent MIME type sniffing
│  │
│  ├─→ X-Frame-Options: DENY
│  │   │
│  │   └─ Prevent clickjacking attacks
│  │
│  ├─→ X-XSS-Protection: 1; mode=block
│  │   │
│  │   └─ Enable browser XSS filters
│  │
│  ├─→ Referrer-Policy: strict-origin-when-cross-origin
│  │   │
│  │   └─ Control referrer information
│  │
│  └─→ Permissions-Policy: geolocation=(), microphone=(), camera=()
│      │
│      └─ Disable unused browser features
│
├─ Cookie Security (ConfigureApplicationCookie)
│  │
│  ├─→ HttpOnly: true
│  │   │
│  │   └─ JavaScript cannot access authentication cookie
│  │
│  ├─→ Secure: true
│  │   │
│  │   └─ Cookie only sent over HTTPS
│  │
│  └─→ SameSite: Strict
│      │
│      └─ Cookie not sent for cross-site requests
│
└─ Sent with every response ✓
```

---

This comprehensive set of diagrams shows every layer of the authentication implementation!

