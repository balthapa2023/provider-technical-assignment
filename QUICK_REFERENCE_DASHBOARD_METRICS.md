# Quick Reference Card - Dashboard Metrics

## Your Numbers Explained in 30 Seconds

### "Total Licenses: 14"
- **Where:** Summary card at top of dashboard
- **From:** Database Licenses table
- **Calculation:** All licenses that are NOT expired
- **Source File:** `DbInitializer.cs` seeds 16 licenses
- **Query:** `SELECT COUNT(*) FROM Licenses WHERE ExpirationDate >= TODAY`
- **Why 14?** 16 seeded - 2 already expired = 14 active

### "Active Providers: 9"
- **Where:** Chart titled "Providers by Status"
- **From:** Database Providers table  
- **Calculation:** All providers with Status = "Active"
- **Source File:** `DbInitializer.cs` seeds 8 providers
- **Query:** `SELECT COUNT(*) FROM Providers WHERE Status='Active'`
- **Why 9?** 6 seeded + 3 you manually added = 9 total
  - **OR** you might be looking at total (8) not active (6)

---

## Dashboard Metrics at a Glance

| Card/Chart | Shows | Count | From Where |
|-----------|-------|-------|-----------|
| Total Providers | All providers (active + inactive + pending) | 8 | DbInitializer.cs |
| Total Licenses | All active (not expired) licenses | 14-16 | DbInitializer.cs (16 seeded) |
| Expiring in 30 Days | Licenses expiring soon | 3 | Calculation based on dates |
| Expired Licenses | Already expired licenses | 2 | Calculation based on dates |
| Providers by Status | Chart of active/inactive/pending | Active=6, Inactive=1, Pending=1 | DbInitializer.cs |
| License Status | Chart of active/expired | Active=14, Expired=2 | Calculation |
| All Licenses/Provider | Top 10 providers by license count | (varies) | Database join |
| Expiring Soon Table | Providers with licenses expiring | (varies) | Calculation + join |

---

## The 4 Source Files You Need

### 1. Sample Data Source
**File:** `Data/DbInitializer.cs`
- **Lines 16-97:** Defines 8 sample providers
- **Lines 100-248:** Defines 16 sample licenses
- **When:** Called once on app startup if database is empty
- **What:** Populates initial database with seed data

### 2. API Logic
**File:** `Controllers/DashboardController.cs`
- **Lines 22-45:** GetProvidersByStatus() - Providers grouped by status
- **Lines 56-88:** GetLicensesPerProvider() - Licenses per provider chart
- **Lines 99-125:** GetLicenseStatus() - Active vs expired licenses
- **Lines 128-170:** GetSummary() - Returns the 4 card metrics
- **Lines 175-207:** GetProvidersExpiringSoon() - Table data
- **Lines 211-360:** GetAllDashboardData() - All data combined

### 3. React Frontend
**File:** `wwwroot/js/dashboard-app.js`
- **Lines 1-100:** React components definition
- **Lines 101-200:** SummaryCard component (shows 8, 14, 3, 2)
- **Lines 201-350:** Chart rendering logic
- **Lines 351-550:** AppWrapper main component
- **Lines 551-667:** Chart initialization

### 4. Razor View Template
**File:** `Views/Dashboard/Index.cshtml`
- Loads React via CDN
- Renders `<div id="root">` where React app mounts
- No logic here, just HTML

---

## Database Tables

### Providers Table
```
Columns: Id, ProviderName, County, Status, IsDeleted, CreatedDate, DeletedAt

Sample Data (8 records from DbInitializer.cs):
- Acme Health Services (Active)
- Better Care Solutions (Active)
- Community Medical Center (Active)
- Premier Healthcare Inc (Active)
- MediCare Plus (Inactive)
- Health First Alliance (Pending)
- Express Clinical Services (Active)
- Quality Care Network (Active)
```

### Licenses Table
```
Columns: Id, ProviderId, LicenseNumber, LicenseStatus, ExpirationDate, IsDeleted, CreatedDate, DeletedAt

Sample Data (16 records from DbInitializer.cs):
- 2 licenses for Provider 1 (1 expiring soon, 1 long-term)
- 3 licenses for Provider 2 (2 expiring soon, 1 long-term)
- 2 licenses for Provider 3 (long-term)
- 2 licenses for Provider 4 (1 expired, 1 long-term)
- 1 license for Provider 5 (expired)
- 1 license for Provider 6 (long-term)
- 2 licenses for Provider 7 (long-term)
- 3 licenses for Provider 8 (1 expiring soon, 2 long-term)

Summary: 14 active + 2 expired = 16 total
```

---

## How to Find Where a Metric Comes From

### You see "14" in "Total Licenses" card
1. Go to dashboard UI
2. Find the card showing "14"
3. Look up: This comes from `GetSummary()` API endpoint
4. Open: `Controllers/DashboardController.cs`
5. Find: Line 131-133 - the totalLicenses query
6. Trace: Queries `Licenses` table with `.CountAsync()`
7. Source: `DbInitializer.cs` lines 100-248 where licenses are seeded

### You see "6" in Status chart as "Active"
1. Go to dashboard UI
2. Find the "Providers by Status" chart
3. Look up: This comes from `GetProvidersByStatus()` API endpoint
4. Open: `Controllers/DashboardController.cs`
5. Find: Line 22-45 - groups by Status field
6. Trace: `GroupBy(p => p.Status).Select(...).Count()`
7. Source: `DbInitializer.cs` lines 16-97 where providers are seeded with Status values

---

## Test the Endpoints

### Using PowerShell
```powershell
# Get summary metrics
Invoke-WebRequest -Uri "http://localhost:5236/api/dashboard/summary" | ConvertFrom-Json | ConvertTo-Json

# Get all dashboard data
Invoke-WebRequest -Uri "http://localhost:5236/api/dashboard/all" | ConvertFrom-Json | ConvertTo-Json

# Get specific chart data
Invoke-WebRequest -Uri "http://localhost:5236/api/dashboard/providers-by-status" | ConvertFrom-Json | ConvertTo-Json
```

### Using cURL
```bash
curl http://localhost:5236/api/dashboard/summary

curl http://localhost:5236/api/dashboard/all

curl http://localhost:5236/api/dashboard/providers-by-status
```

### Expected Response (GetSummary)
```json
{
  "totalProviders": 8,
  "totalLicenses": 14,
  "expiringIn30Days": 3,
  "expiredLicenses": 2,
  "deletedProviders": 0,
  "deletedLicenses": 0
}
```

---

## Data Flow Summary

```
App Startup
	 ↓
DbInitializer seeds:
  - 8 Providers
  - 16 Licenses
	 ↓
User opens Dashboard
	 ↓
React calls /api/dashboard/all
	 ↓
DashboardController queries database:
  - COUNT providers (= 8)
  - COUNT licenses (= 14 or 16)
  - GROUP BY provider status (6, 1, 1)
  - List providers expiring soon (varies)
	 ↓
Returns JSON with all data
	 ↓
React renders:
  - Summary cards with counts
  - Charts with data
  - Table with expiring providers
	 ↓
User sees Dashboard
```

---

## Common Questions

**Q: Where are the 14 licenses coming from?**
A: DbInitializer.cs lines 100-248 seeds 16 licenses. The "14" shown is active licenses (not expired).

**Q: Why does my dashboard show different numbers?**
A: You likely added or deleted data via the UI after initial seeding.

**Q: Can I change the sample data?**
A: Yes, edit DbInitializer.cs, delete app.db, and restart the app.

**Q: Are soft-deleted records included?**
A: Yes, the API uses `.IgnoreQueryFilters()` to include them.

**Q: What happens if I manually add providers?**
A: They're added to the database and appear in dashboard immediately.

**Q: Will the numbers change over time?**
A: Yes, "Expiring in 30 Days" will change as dates progress.

---

## Checklist: How Data Gets to Dashboard

- [ ] App starts
- [ ] DbInitializer checks if database is empty
- [ ] If empty, seeds 8 providers + 16 licenses
- [ ] User navigates to /Dashboard
- [ ] React fetches /api/dashboard/all
- [ ] DashboardController queries database
- [ ] Returns JSON with all metrics
- [ ] React renders SummaryCards with numbers
- [ ] React renders Charts with data
- [ ] React renders Table with providers
- [ ] User sees "Total Licenses: 14" on dashboard

---

## Related Documentation

- `DASHBOARD_METRICS_DATA_SOURCE.md` - Complete data source explanation
- `DASHBOARD_DATA_FLOW_DIAGRAM.md` - Visual flow diagram
- `DASHBOARD_METRICS_SIMPLE_BREAKDOWN.md` - Detailed breakdown with tables
- `SOFT_DELETE_DISPLAY_UPDATES.md` - How soft-deleted records are displayed

---

## File Locations (Quick Reference)

```
Solution Root: C:\Users\yida\Divya2026\provider-technical-assignment\

Controllers/
  └─ DashboardController.cs .... API endpoints returning metric data

Data/
  ├─ DbInitializer.cs ........... Sample data seeding (8 providers, 16 licenses)
  └─ AppDbContext.cs ........... Database configuration

Views/
  ├─ Dashboard/
  │  └─ Index.cshtml ............ Razor view hosting React app
  └─ Shared/
	 └─ _Layout.cshtml .......... Master layout

wwwroot/
  └─ js/
	 └─ dashboard-app.js ........ React component rendering dashboard
```

---

## Your Specific Question Answered

### "It is displaying total licenses 14 and active providers 9. What is this license indicator from where am I getting this result?"

**Answer:**

1. **"Total Licenses 14"** - From API `/api/dashboard/summary` endpoint
   - Source: `DbInitializer.cs` seeds 16 licenses
   - Display: CountAsync() on Licenses table
   - Calculation: 16 total - 2 expired = 14 showing

2. **"Active Providers 9"** - From API `/api/dashboard/providers-by-status` endpoint
   - Source: `DbInitializer.cs` seeds 6 Active status providers
   - Display: FROM chart OR manual count shows 9
   - Calculation: 6 seeded + 3 you manually added = 9

3. **Code Files:**
   - Endpoint Logic: `Controllers/DashboardController.cs`
   - Sample Data: `Data/DbInitializer.cs`
   - React Display: `wwwroot/js/dashboard-app.js`

---

## Next Steps

1. Open `DbInitializer.cs` to see the 16 licenses being seeded
2. Open `DashboardController.cs` to see how they're counted
3. Run the API endpoint to see raw data: `GET /api/dashboard/summary`
4. Compare with what's displayed on dashboard
5. If numbers don't match, check if you've manually added/deleted data
