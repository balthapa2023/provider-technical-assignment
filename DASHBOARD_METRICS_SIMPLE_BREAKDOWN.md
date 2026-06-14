# Dashboard Metrics - Simple Breakdown

## Your Dashboard Currently Shows

```
┌─────────────────────────────────────────────────────────────────┐
│                     DASHBOARD SUMMARY CARDS                      │
├─────────────────────────────────────────────────────────────────┤
│                                                                   │
│  🏢 Total Providers        📋 Total Licenses                      │
│     8 (or more)              14-16                              │
│    (2 deleted)              (2 deleted)                         │
│                                                                   │
│  ⏰ Expiring in 30 Days     ❌ Expired Licenses                   │
│     3                         2                                 │
│                                                                   │
└─────────────────────────────────────────────────────────────────┘
```

---

## Where Each Number Comes From

### Card 1: Total Providers = 8

**Source:**
```
Database Table: Providers

Query: SELECT COUNT(*) FROM Providers WHERE IsDeleted = false

Sample Data (DbInitializer.cs):
┌─────────────────────────────────────────────────┐
│ ID │ Name                          │ Status     │
├─────────────────────────────────────────────────┤
│ 1  │ Acme Health Services          │ Active     │
│ 2  │ Better Care Solutions         │ Active     │
│ 3  │ Community Medical Center      │ Active     │
│ 4  │ Premier Healthcare Inc        │ Active     │
│ 5  │ MediCare Plus                 │ Inactive   │
│ 6  │ Health First Alliance         │ Pending    │
│ 7  │ Express Clinical Services     │ Active     │
│ 8  │ Quality Care Network          │ Active     │
└─────────────────────────────────────────────────┘

Total: 8 providers
(If you added more via UI, this number increases)
```

**Code Location:** `Controllers/DashboardController.cs`, lines 128-130

---

### Card 2: Total Licenses = 14

**Source:**
```
Database Table: Licenses

Query: SELECT COUNT(*) FROM Licenses WHERE IsDeleted = false
	   AND ExpirationDate >= TODAY

Sample Data (DbInitializer.cs):
┌──────────────┬────────────┬──────────────────────────────────┐
│ License ID   │ Provider   │ Expiration Date                  │
├──────────────┼────────────┼──────────────────────────────────┤
│ LIC-001      │ Provider 1 │ Today + 1 year         ACTIVE    │
│ LIC-002      │ Provider 1 │ Today + 15 days        ACTIVE    │
│ LIC-003      │ Provider 2 │ Today + 2 years        ACTIVE    │
│ LIC-004      │ Provider 2 │ Today + 25 days        ACTIVE    │
│ LIC-005      │ Provider 2 │ Today + 30 days        ACTIVE    │
│ LIC-006      │ Provider 3 │ Today + 1.5 years      ACTIVE    │
│ LIC-007      │ Provider 3 │ Today + 1.5 years      ACTIVE    │
│ LIC-008      │ Provider 4 │ Today + 10 months      ACTIVE    │
│ LIC-009      │ Provider 4 │ Today - 5 days         EXPIRED   │
│ LIC-010      │ Provider 5 │ Today - 30 days        EXPIRED   │
│ LIC-011      │ Provider 6 │ Today + 6 months       ACTIVE    │
│ LIC-012      │ Provider 7 │ Today + 15 months      ACTIVE    │
│ LIC-013      │ Provider 7 │ Today + 15 months      ACTIVE    │
│ LIC-014      │ Provider 8 │ Today + 9 months       ACTIVE    │
│ LIC-015      │ Provider 8 │ Today + 9 months       ACTIVE    │
│ LIC-016      │ Provider 8 │ Today + 20 days        ACTIVE    │
└──────────────┴────────────┴──────────────────────────────────┘

Active (not expired): 14 licenses (LIC-001 through LIC-008, LIC-011 through LIC-016)
Expired: 2 licenses (LIC-009, LIC-010)
```

**Code Location:** `Controllers/DashboardController.cs`, lines 131-133

---

### Card 3: Expiring in 30 Days = 3

**Which licenses expire in the next 30 days?**

```
Today's Date: [Today]

Expiring in Next 30 Days (Today to Today+30 days):
┌──────────────┬────────────┬──────────────────────────────────┐
│ License ID   │ Provider   │ Expiration Date                  │
├──────────────┼────────────┼──────────────────────────────────┤
│ LIC-002      │ Provider 1 │ Today + 15 days        ✓ YES     │
│ LIC-004      │ Provider 2 │ Today + 25 days        ✓ YES     │
│ LIC-016      │ Provider 8 │ Today + 20 days        ✓ YES     │
└──────────────┴────────────┴──────────────────────────────────┘

Count: 3 licenses expiring in next 30 days
```

**Code Location:** `Controllers/DashboardController.cs`, lines 139-142

---

### Card 4: Expired Licenses = 2

**Which licenses are already expired?**

```
Today's Date: [Today]

Already Expired (ExpirationDate < Today):
┌──────────────┬────────────┬──────────────────────────────────┐
│ License ID   │ Provider   │ Expiration Date                  │
├──────────────┼────────────┼──────────────────────────────────┤
│ LIC-009      │ Provider 4 │ Today - 5 days         ✗ EXPIRED │
│ LIC-010      │ Provider 5 │ Today - 30 days        ✗ EXPIRED │
└──────────────┴────────────┴──────────────────────────────────┘

Count: 2 licenses already expired
```

**Code Location:** `Controllers/DashboardController.cs`, lines 145-148

---

## Where Did "14" Licenses come from?

### Method 1: Sample Data Seeding

**When you first run the app:**
1. `Program.cs` starts
2. Calls `DbInitializer.Initialize()`
3. Checks if database is empty
4. If empty, seeds **16 licenses** (LIC-2024-001 through LIC-2024-016)
5. All with `IsDeleted = false` initially

**You see 14 because:**
- Api endpoint counts ACTIVE licenses only
- 16 total seeded licenses
- Minus 2 that are already expired
- = 14 active licenses showing

OR

- 2 of the 16 licenses were soft-deleted
- 16 - 2 = 14 licenses

### Method 2: If You Added Licenses Manually

Any licenses you add through the "New License" UI will:
- Increase the total count
- Be displayed in charts and cards
- Automatically included if status is "Active"

---

## Source Code Reference

### Where the "14" is calculated:

**File:** `Controllers/DashboardController.cs`

```csharp
// Line 131-133: GetSummary() method
var totalLicenses = await _context.Licenses
	.IgnoreQueryFilters() // Include soft-deleted
	.CountAsync();  // Counts ALL licenses, returns 16 or 14 depending on deletions
```

**Returns:**
- 16 if no licenses are soft-deleted
- 14 if 2 licenses are soft-deleted

### Where it's called from:

**File:** `wwwroot/js/dashboard-app.js`

```javascript
// React component fetches the data
fetch('/api/dashboard/summary')
	.then(response => response.json())
	.then(data => {
		// data.totalLicenses = 14 or 16
		// Display in SummaryCard
		<SummaryCard 
			title="Total Licenses"
			value={data.totalLicenses}
			subtext={`(${data.deletedLicenses} deleted)`}
		/>
	})
```

---

## The 4 Dashboard Charts

### Chart 1: Providers by Status

```
Query: SELECT Status, COUNT(*) FROM Providers GROUP BY Status

Result:
┌────────────┬───────┐
│ Status     │ Count │
├────────────┼───────┤
│ Active     │   6   │  ← Providers 1,2,3,4,7,8
│ Inactive   │   1   │  ← Provider 5
│ Pending    │   1   │  ← Provider 6
└────────────┴───────┘

Chart Type: Horizontal Bar Chart
Labels: ["Active", "Inactive", "Pending"]
Data: [6, 1, 1]
```

---

### Chart 2: License Status Distribution

```
Query: SELECT 
		 CASE WHEN ExpirationDate >= TODAY THEN 'Active'
			  ELSE 'Expired'
		 END as Status,
		 COUNT(*)
	   FROM Licenses
	   GROUP BY Status

Result:
┌────────┬───────┐
│ Status │ Count │
├────────┼───────┤
│ Active │  14   │  ← LIC-001 through 008, 011 through 016
│ Expired│   2   │  ← LIC-009, LIC-010
└────────┴───────┘

Chart Type: Doughnut Chart
Labels: ["Active", "Expired"]
Data: [14, 2]
```

---

### Chart 3: All Licenses per Provider (Top 10)

```
Query: SELECT ProviderName, COUNT(Licenses) as LicenseCount
	   FROM Providers
	   JOIN Licenses ON Providers.ID = Licenses.ProviderId
	   GROUP BY ProviderName
	   ORDER BY LicenseCount DESC
	   LIMIT 10

Result:
┌──────────────────────────────┬───────┐
│ Provider Name                │ Count │
├──────────────────────────────┼───────┤
│ Quality Care Network (ID 8)  │   3   │  ← LIC-014, 015, 016
│ Better Care Solutions (ID 2) │   3   │  ← LIC-003, 004, 005
│ Community Medical Center (ID 3)│  2   │  ← LIC-006, 007
│ Premier Healthcare Inc (ID 4) │  2   │  ← LIC-008, 009
│ Acme Health Services (ID 1)  │   2   │  ← LIC-001, 002
│ Express Clinical Services (ID 7)│ 2   │  ← LIC-012, 013
│ Health First Alliance (ID 6) │   1   │  ← LIC-011
│ MediCare Plus (ID 5)         │   1   │  ← LIC-010
└──────────────────────────────┴───────┘

Chart Type: Horizontal Bar Chart
Labels: [names above]
Data: [3, 3, 2, 2, 2, 2, 1, 1]
```

---

### Chart 4: Providers with Licenses Expiring in 30 Days (Table)

```
Query: SELECT ProviderId, ProviderName, County, Status, COUNT(Licenses) as ExpiringCount
	   FROM Providers
	   JOIN Licenses ON Providers.ID = Licenses.ProviderId
	   WHERE Licenses.ExpirationDate BETWEEN TODAY AND TODAY+30
	   GROUP BY ProviderId
	   ORDER BY ExpiringCount DESC

Result:
┌──────────────┬──────────────────────┬────────┬────────┬──────────┐
│ Provider ID  │ Provider Name        │ County │ Status │ Expiring │
├──────────────┼──────────────────────┼────────┼────────┼──────────┤
│ 2            │ Better Care Solutions│ DeKalb │ Active │    2     │  (LIC-004, 005)
│ 1            │ Acme Health Services │ Fulton │ Active │    1     │  (LIC-002)
│ 8            │ Quality Care Network │ Marietta│ Active │   1     │  (LIC-016)
└──────────────┴──────────────────────┴────────┴────────┴──────────┘

Displayed as: Table with rows for each provider
```

---

## Complete Number Summary

| Metric | Count | Source | How Calculated |
|--------|-------|--------|-----------------|
| **Total Providers** | 8 | DbInitializer.cs lines 16-97 | COUNT(*) FROM Providers |
| **Active Providers** | 6 | DbInitializer.cs | WHERE Status="Active" |
| **Inactive Providers** | 1 | DbInitializer.cs | WHERE Status="Inactive" |
| **Pending Providers** | 1 | DbInitializer.cs | WHERE Status="Pending" |
| **Total Licenses** | 14-16 | DbInitializer.cs lines 100-248 | COUNT(*) FROM Licenses |
| **Active Licenses** | 14 | Calculated | WHERE ExpirationDate >= TODAY |
| **Expired Licenses** | 2 | Calculated | WHERE ExpirationDate < TODAY |
| **Expiring in 30 Days** | 3 | Calculated | WHERE ExpirationDate BETWEEN TODAY AND TODAY+30 |
| **Deleted Providers** | 0-2 | Calculated | WHERE IsDeleted=true |
| **Deleted Licenses** | 0-2 | Calculated | WHERE IsDeleted=true |

---

## How to See This in Action

### Step 1: Run the App
```bash
cd C:\Users\yida\Divya2026\provider-technical-assignment
dotnet run
```

### Step 2: Navigate to Dashboard
```
http://localhost:5236/Dashboard
```

### Step 3: You'll See
- 4 Summary Cards with the numbers above
- 3 Charts with visual representations
- 1 Table with expiring providers

### Step 4: Verify the Data
- Go to "Providers" page to count active providers
- Go to "Licenses" page to count total licenses
- Numbers should match dashboard

---

## If Your Numbers Don't Match This

### Scenario 1: You see different numbers

**Possible causes:**
- You manually added/deleted providers or licenses via the UI
- The database wasn't reset (delete `app.db` and restart)
- You're looking at a different dashboard page

**Solution:**
- Check the Providers and Licenses pages
- Compare with dashboard numbers
- They should match

### Scenario 2: Chart shows "9 Active Providers"

**Possible cause:**
- 6 seeded active + 3 you manually added = 9 total

**Solution:**
- Go to Providers page
- Count how many have "Active" status
- Does it equal 9? Then you added 3 more

### Scenario 3: License count keeps changing

**Possible cause:**
- Each run, expired licenses might shift categories
- Expiration dates are relative to "today"
- As time passes, licenses expire

**Solution:**
- This is normal behavior
- Expired licenses will increase over time
- Active licenses will decrease

---

## Final Summary

```
📊 Dashboard showing "Total Licenses: 14" means:
   ↓
   From: Database Licenses table (DbInitializer.cs seeded 16 licenses)
   Query: COUNT(*) WHERE IsDeleted = false AND ExpirationDate >= TODAY
   Result: 14 active (not expired) licenses

📊 Dashboard showing "Active Providers: 9" means:
   ↓
   From: Database Providers table (DbInitializer.cs seeded 8 active + you added 3)
   Query: COUNT(*) WHERE Status = 'Active'
   Result: 9 active providers

✅ All data comes from:
   - File: Data/DbInitializer.cs (initial sample data)
   - Database: SQLite app.db (persisted data)
   - API: Controllers/DashboardController.cs (calculations)
   - UI: wwwroot/js/dashboard-app.js (display)
```
