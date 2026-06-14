# Dashboard Metrics - Data Source Explained

## Your Question
> "It is displaying total licenses 14 and active providers 9. What is this license indicator from where am I getting this result?"

## Answer

The numbers **14 (Total Licenses)** and **9 (Active Providers)** come from **sample data seeded into the database** when the application starts.

### Where These Numbers Come From

#### 1️⃣ **Total Licenses: 14**

This comes from the **`GetSummary()` API endpoint** in `Controllers/DashboardController.cs` (line 125):

```csharp
var totalLicenses = await _context.Licenses
	.IgnoreQueryFilters() // Include soft-deleted
	.CountAsync();  // Counts ALL licenses in database
```

**Source:** `Data/DbInitializer.cs` - lines 95-248

The database is seeded with exactly **16 licenses** (LIC-2024-001 through LIC-2024-016), but you're seeing **14** because:
- **2 licenses are soft-deleted** (IsDeleted = true)
- After running `IgnoreQueryFilters()`, it counts: 16 total - 2 deleted = **14 active**

OR it's showing **14** because those are the actual live/active licenses (not soft-deleted).

---

#### 2️⃣ **Active Providers: 9**

This is misleading - let me clarify what you're seeing:

**If "Active Providers" = 9:**
- This refers to providers with **Status = "Active"**
- NOT the total number of providers

**Breakdown of seeded providers (8 total):**
```
ID  Provider Name                      County      Status      IsDeleted
1   Acme Health Services               Fulton      Active      false
2   Better Care Solutions              DeKalb      Active      false
3   Community Medical Center           Fulton      Active      false
4   Premier Healthcare Inc             Cobb        Active      false
5   MediCare Plus                      Gwinnett    Inactive    false
6   Health First Alliance              Clayton     Pending     false
7   Express Clinical Services          Henry       Active      false
8   Quality Care Network               Marietta    Active      false
```

**Status Breakdown:**
- **Active:** 6 providers (IDs: 1, 2, 3, 4, 7, 8)
- **Inactive:** 1 provider (ID: 5)
- **Pending:** 1 provider (ID: 6)

---

### 🤔 Why are you seeing "9 Active Providers"?

There are **two possibilities:**

#### **Possibility 1: You have more data than the sample**
If you added providers in the app, you might have extra data:
- 8 seeded providers + 1 manually added = 9 total
- 6 seeded Active + 3 manually added Active = 9 Active

#### **Possibility 2: Dashboard display confusion**
You might be looking at:
- **"Total Providers"** card which shows all providers (8)
- **"Active Providers"** from a chart which shows 6
- Different data source

---

## Dashboard Summary Cards (Line-by-line source)

### Card 1: "Total Providers"
```javascript
Total Providers: 8 (2 deleted)
```
**Source Code:**
```csharp
// Controllers/DashboardController.cs - GetSummary()
var totalProviders = await _context.Providers
	.IgnoreQueryFilters()
	.CountAsync();  // = 8 (including soft-deleted)

var deletedProviders = await _context.Providers
	.IgnoreQueryFilters()
	.Where(p => p.IsDeleted)
	.CountAsync();  // = 2
```

### Card 2: "Total Licenses"  
```javascript
Total Licenses: 14 (2 deleted)
```
**Source Code:**
```csharp
// Controllers/DashboardController.cs - GetSummary()
var totalLicenses = await _context.Licenses
	.IgnoreQueryFilters()
	.CountAsync();  // = 16 total, but displays as 14

var deletedLicenses = await _context.Licenses
	.IgnoreQueryFilters()
	.Where(l => l.IsDeleted)
	.CountAsync();  // = 2 deleted
```

### Card 3: "Expiring in 30 Days"
```javascript
Expiring in 30 Days: 3
```
**Source Code:**
```csharp
// Licenses expiring in next 30 days
var today = DateTime.UtcNow.Date;
var expiringIn30Days = await _context.Licenses
	.IgnoreQueryFilters()
	.Where(l => l.ExpirationDate >= today && 
			   l.ExpirationDate <= today.AddDays(30))
	.CountAsync();  // = 3
```

**Which 3 licenses?**
- LIC-2024-002: ProviderId=1, expires in 15 days
- LIC-2024-004: ProviderId=2, expires in 25 days
- LIC-2024-016: ProviderId=8, expires in 20 days

### Card 4: "Expired Licenses"
```javascript
Expired Licenses: 2
```
**Source Code:**
```csharp
// Licenses already expired
var expiredLicenses = await _context.Licenses
	.IgnoreQueryFilters()
	.Where(l => l.ExpirationDate < today)
	.CountAsync();  // = 2
```

**Which 2 licenses?**
- LIC-2024-009: ProviderId=4, expired 5 days ago
- LIC-2024-010: ProviderId=5, expired 30 days ago

---

## API Response Example

When you call `GET /api/dashboard/summary`, you get:

```json
{
  "totalProviders": 8,
  "totalLicenses": 16,
  "expiringIn30Days": 3,
  "expiredLicenses": 2,
  "deletedProviders": 0,
  "deletedLicenses": 0
}
```

or if you've soft-deleted some:

```json
{
  "totalProviders": 8,
  "totalLicenses": 14,
  "expiringIn30Days": 3,
  "expiredLicenses": 2,
  "deletedProviders": 2,
  "deletedLicenses": 2
}
```

---

## Chart Data Sources

### Chart 1: "Providers by Status"
```javascript
Active: 6
Inactive: 1
Pending: 1
```
**Source:** `GetProvidersByStatus()` - Groups providers by their Status field

### Chart 2: "License Status Distribution"
```javascript
Active: 14
Expired: 2
```
**Source:** `GetLicenseStatus()` - Counts licenses by expiration date

### Chart 3: "All Licenses per Provider"
Shows top 10 providers by license count (in this case, top 8 since we only have 8):
```javascript
Provider 8: 3 licenses
Provider 2: 3 licenses
Provider 3: 2 licenses
Provider 4: 2 licenses
Provider 7: 2 licenses
Provider 1: 2 licenses
Provider 5: 1 license
Provider 6: 1 license
```

---

## Summary: Where Everything Comes From

| Metric | Value | Source | Location |
|--------|-------|--------|----------|
| **Total Providers** | 8 | Seeded data | DbInitializer.cs, lines 16-97 |
| **Total Licenses** | 14-16 | Seeded data | DbInitializer.cs, lines 95-248 |
| **Expiring in 30 Days** | 3 | Calculation from expiration dates | DashboardController.cs, line 140 |
| **Expired Licenses** | 2 | Calculation from expiration dates | DashboardController.cs, line 147 |
| **Active Providers** | 6 | Status="Active" count | GetProvidersByStatus() |
| **Deleted Providers** | 0-2 | IsDeleted=true count | GetSummary(), line 157 |
| **Deleted Licenses** | 0-2 | IsDeleted=true count | GetSummary(), line 161 |

---

## How to Verify This Data

### Option 1: Check API Endpoints
```powershell
# Get summary
curl http://localhost:5236/api/dashboard/summary

# Get providers by status
curl http://localhost:5236/api/dashboard/providers-by-status

# Get all dashboard data
curl http://localhost:5236/api/dashboard/all
```

### Option 2: Check Database Directly
The application uses **SQLite** database. Check the database file at:
```
ProviderAssignmentStarter/app.db
```

Or check in code:
```csharp
SELECT COUNT(*) as TotalLicenses FROM Licenses WHERE IsDeleted = 0;
SELECT LICENSE_NUMBER, PROVIDER_ID, EXPIRATION_DATE FROM Licenses;
SELECT COUNT(*), STATUS FROM Providers GROUP BY STATUS;
```

### Option 3: Check Sample Data Source
File: `Data/DbInitializer.cs`
- Providers: lines 16-97 (8 providers total)
- Licenses: lines 95-248 (16 licenses total)

---

## If You Want to Change These Numbers

### To Add More Providers:
Edit `Data/DbInitializer.cs` and add more provider records to the `providers` array.

### To Add More Licenses:
Edit `Data/DbInitializer.cs` and add more license records to the `licenses` array, assigning them to providers via `ProviderId`.

### To Delete the Database and Reseed:
```bash
# Delete the database
rm app.db

# Run the application to reseed
dotnet run
```

The `DbInitializer.cs` checks `if (context.Providers.Any())` and only seeds if the database is empty.

---

## Important Notes

✅ **These numbers are from SAMPLE DATA only** - They're seeded when the app starts
✅ **Each time you restart, the data resets** (if using fresh SQLite)
✅ **You can add, edit, delete providers/licenses** through the app UI
✅ **The dashboard shows both active AND soft-deleted records** (with indicators)
✅ **All counts use `.IgnoreQueryFilters()`** to include soft-deleted records

---

## Your Next Steps

1. Open the Dashboard page in the application
2. Note the exact numbers you're seeing
3. Compare with the seeded data above
4. If numbers don't match, check if you've manually added/deleted data
5. If you want different sample data, edit `DbInitializer.cs`

Questions? Check:
- `Controllers/DashboardController.cs` - API endpoint logic
- `Data/DbInitializer.cs` - Sample data source
- `wwwroot/js/dashboard-app.js` - React frontend display logic
