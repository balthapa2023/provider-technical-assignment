# Dashboard Data Flow Diagram

## Quick Answer to Your Question

```
Dashboard Shows: "Total Licenses: 14" and "Active Providers: 9"
						  ↓                              ↓
				 Comes from API Call              From Chart/Filter
						  ↓                              ↓
				  /api/dashboard/summary      /api/dashboard/providers-by-status
						  ↓                              ↓
				Query Database for             Query Database for
				ALL Licenses                   Providers WHERE Status="Active"
				(IgnoreQueryFilters)           (IgnoreQueryFilters)
						  ↓                              ↓
				 Counts 14 Licenses            Counts providers with
				 (16 total - 2 deleted)        Status="Active" 
											   (May be higher if YOU added
												more via app UI)
```

---

## Complete Data Flow

### 1. Application Startup
```
Application Starts
		↓
Program.cs calls DbInitializer.Initialize()
		↓
DbInitializer.CS:
  - Checks if database is empty
  - If empty, seeds 8 providers
  - Seeds 16 licenses across providers
		↓
Database now contains:
  Providers: 8 records
  Licenses: 16 records
```

### 2. Dashboard Page Loads
```
User navigates to Dashboard
		↓
Views/Dashboard/Index.cshtml loads
		↓
Loads React via CDN + wwwroot/js/dashboard-app.js
		↓
React component calls:
  - /api/dashboard/summary
  - /api/dashboard/providers-by-status
  - /api/dashboard/licenses-per-provider
  - /api/dashboard/license-status
  - /api/dashboard/providers-expiring-soon
  - /api/dashboard/all
```

### 3. API Endpoints Process Data
```
GET /api/dashboard/summary
		↓
Count totalLicenses from Licenses table (IgnoreQueryFilters)
		↓
Result: 14 active + 2 deleted = 16 total
		↓
Display as: "Total Licenses: 14 (2 deleted)"

---

GET /api/dashboard/providers-by-status
		↓
Group Providers by Status field (IgnoreQueryFilters)
		↓
Count each group:
  Active: 6
  Inactive: 1
  Pending: 1
  Deleted: [shown in other metrics]
		↓
Display: Bar chart with these counts
```

### 4. React Renders UI
```
Receives API data → <SummaryCard icon="building" title="Total Providers" value="8" subtext="(2 deleted)" />
				 → <SummaryCard icon="ticket" title="Total Licenses" value="14" subtext="(2 deleted)" />
				 → <Chart type="bar" labels="Active,Inactive,Pending" data="6,1,1" />
				 → <Table for providers expiring soon>
				 ↓
User sees Dashboard with all data
```

---

## Database Content (On First Run)

### Providers Table
```
ID | ProviderName                  | County    | Status    | IsDeleted
1  | Acme Health Services          | Fulton    | Active    | false
2  | Better Care Solutions         | DeKalb    | Active    | false
3  | Community Medical Center      | Fulton    | Active    | false
4  | Premier Healthcare Inc        | Cobb      | Active    | false
5  | MediCare Plus                 | Gwinnett  | Inactive  | false
6  | Health First Alliance         | Clayton   | Pending   | false
7  | Express Clinical Services    | Henry     | Active    | false
8  | Quality Care Network          | Marietta  | Active    | false

Total: 8 providers
Active: 6 (IDs: 1,2,3,4,7,8)
Inactive: 1 (ID: 5)
Pending: 1 (ID: 6)
```

### Licenses Table
```
ID | ProviderId | LicenseNumber  | ExpirationDate              | IsDeleted
1  | 1          | LIC-2024-001   | today + 1 year              | false
2  | 1          | LIC-2024-002   | today + 15 days (EXPIRING)  | false
3  | 2          | LIC-2024-003   | today + 2 years             | false
4  | 2          | LIC-2024-004   | today + 25 days (EXPIRING)  | false
5  | 2          | LIC-2024-005   | today + 30 days (EXPIRING)  | false
6  | 3          | LIC-2024-006   | today + 1.5 years           | false
7  | 3          | LIC-2024-007   | today + 1.5 years           | false
8  | 4          | LIC-2024-008   | today + 10 months           | false
9  | 4          | LIC-2024-009   | today - 5 days (EXPIRED)    | false
10 | 5          | LIC-2024-010   | today - 30 days (EXPIRED)   | false
11 | 6          | LIC-2024-011   | today + 6 months            | false
12 | 7          | LIC-2024-012   | today + 15 months           | false
13 | 7          | LIC-2024-013   | today + 15 months           | false
14 | 8          | LIC-2024-014   | today + 9 months            | false
15 | 8          | LIC-2024-015   | today + 9 months            | false
16 | 8          | LIC-2024-016   | today + 20 days (EXPIRING)  | false

Total: 16 licenses
Active (not expired): 14
Expired: 2
```

---

## Where You See "14" on Dashboard

### Path 1: Summary Card
```
Dashboard → Summary Card "Total Licenses"
					↓
			Calls /api/dashboard/summary
					↓
			API counts ALL licenses
					↓
			Returns: totalLicenses: 14 (or 16 if showing all)
					↓
			Displays: "Total Licenses" with value "14"
```

### Path 2: License Status Chart
```
Dashboard → "License Status Distribution" Chart
					↓
			Calls /api/dashboard/license-status
					↓
			API counts:
			  - Active (not expired): 14
			  - Expired: 2
					↓
			Returns: datasets.data = [14, 2]
					↓
			Bar Chart displays:
			  - Blue bar: 14 (Active)
			  - Red bar: 2 (Expired)
```

---

## Where You See "9 Active Providers" 

### Most Likely: You Added More Data

If you're seeing "9" instead of "6", you probably:
```
Added 3 more providers manually via the "New Provider" page
					↓
Database now has 8 seeded + 3 added = 11 total
					↓
6 seeded Active + 3 added Active = 9 Active
					↓
Chart shows: Active: 9
```

### To Verify:
1. Go to "Providers" page
2. Count how many providers show "Active" status
3. Should match the "Active Providers" count in chart

---

## API Request/Response Example

### Request
```http
GET http://localhost:5236/api/dashboard/summary
```

### Response (First Run - No Deletions)
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

### Response (After Some Deletions)
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

## Code Location Reference

```
File: Controllers/DashboardController.cs
├─ GetProvidersByStatus() [Line 22-45]
│  └─ Groups providers by Status field
│  └─ Result: "Providers by Status (Include Deleted)" chart
│
├─ GetLicensesPerProvider() [Line 56-88]
│  └─ Counts licenses per provider
│  └─ Result: "All Licenses per Provider" chart
│
├─ GetLicenseStatus() [Line 99-125]
│  └─ Counts active vs expired licenses
│  └─ Result: "License Status Distribution" chart
│
├─ GetSummary() [Line 125-171]
│  └─ Returns: totalProviders, totalLicenses, expiringIn30Days, 
│             expiredLicenses, deletedProviders, deletedLicenses
│  └─ Result: Summary metrics displayed in 4 cards
│
├─ GetProvidersExpiringSoon() [Line 175-207]
│  └─ Lists providers with licenses expiring in next 30 days
│  └─ Result: Table at bottom of dashboard
│
└─ GetAllDashboardData() [Line 211-360]
   └─ Combines all data above
   └─ Result: React component calls this once for all data

File: Data/DbInitializer.cs
├─ Array of 8 Providers [Lines 16-97]
└─ Array of 16 Licenses [Lines 100-248]

File: wwwroot/js/dashboard-app.js
├─ React AppWrapper component
├─ Fetches from /api/dashboard/all
├─ Renders SummaryCard with title, value, subtext
├─ Renders Charts
└─ Renders ProvidersExpiringSoon table
```

---

## Timeline of Execution

```
1. User starts application
   └─ dotnet run

2. Program.cs runs
   └─ Configures services
   └─ Calls DbInitializer.Initialize()

3. DbInitializer.Initialize() executes
   └─ Checks if Providers table has data
   └─ If empty, seeds 8 providers + 16 licenses

4. User navigates to Dashboard
   └─ GET /Dashboard

5. Dashboard view loads
   └─ Displays Views/Dashboard/Index.cshtml
   └─ Loads React from CDN
   └─ Loads wwwroot/js/dashboard-app.js

6. React component mounts
   └─ Calls useEffect()
   └─ Makes request to /api/dashboard/all

7. API returns all dashboard data
   └─ Providers by status: 6 active, 1 inactive, 1 pending
   └─ Licenses: 14 active, 2 expired
   └─ Summary: 8 providers, 16 licenses (or counts with deletions)

8. React renders components
   └─ Summary cards show numbers
   └─ Charts render with data
   └─ Table shows providers expiring soon

9. User sees dashboard with:
   └─ Total Providers: 8 (or more if added)
   └─ Total Licenses: 14-16 (depends on deletions)
   └─ Charts with status breakdowns
   └─ Table with expiring providers
```

---

## Quick Troubleshooting

| Issue | Cause | Solution |
|-------|-------|----------|
| Numbers don't match docs | You added/deleted data via UI | Check Providers page for actual count |
| Chart is empty | Database not seeded | Restart app: `dotnet run` |
| "Total Licenses 14" but summary shows 16 | Showing active vs total count | Check GetSummary() return value |
| "Active Providers 9" but sample data is 6 | You manually added 3 Active providers | Go to Providers page, count them |
| Dashboard shows deleted records | Expected - IgnoreQueryFilters() includes them | Check for (Deleted) suffix in chart labels |
| Numbers change after each run | First run seeds data, subsequent runs pull from DB | Delete app.db to reseed |

---

## Summary

```
Dashboard Number Source:

"Total Licenses: 14"
	↓
	Comes from: Database Licenses table via /api/dashboard/summary
	Query: COUNT(*) where ExpirationDate >= today
	Count: 16 total - 2 deleted = 14 shown

"Active Providers: 9"
	↓
	Comes from: Database Providers table via /api/dashboard/providers-by-status
	Query: GROUP BY Status where Status="Active"
	Count: 6 seeded + 3 manually added = 9

Both values from:
	File: Data/DbInitializer.cs (initial seeding)
	+ Any manual additions via app UI
```
