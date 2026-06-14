# 🎯 Visual Guide: Provider List Display

## Application Architecture

```
┌─────────────────────────────────────────────────────────────┐
│                    User Browser                             │
│         https://localhost:5001/providers                    │
└────────────────────┬────────────────────────────────────────┘
					 │
		┌────────────▼────────────┐
		│  ProvidersController    │
		│  Index() Action         │
		└────────────┬────────────┘
					 │
		┌────────────▼────────────────────┐
		│   IProviderService              │
		│  GetActiveProvidersAsync()      │
		└────────────┬────────────────────┘
					 │
		┌────────────▼────────────────────┐
		│   AppDbContext                  │
		│   Providers DbSet               │
		│   (IsDeleted = false filter)    │
		└────────────┬────────────────────┘
					 │
		┌────────────▼────────────────────┐
		│   SQLite Database               │
		│   provider_assignment.db        │
		│   (6 Providers, 12 Licenses)    │
		└─────────────────────────────────┘
```

---

## Data Flow: View Provider List

```
START
  │
  ├─→ User navigates to /providers
  │
  ├─→ ProvidersController.Index() called
  │
  ├─→ ProviderService.GetActiveProvidersAsync()
  │
  ├─→ DbContext queries: Provider WHERE IsDeleted = false
  │
  ├─→ Includes License data (via navigation property)
  │
  ├─→ Returns List<Provider> with licenses
  │
  ├─→ Pass to View: Index.cshtml
  │
  ├─→ Render HTML table:
  │   ├─ 6 rows (providers)
  │   ├─ Each row shows:
  │   │  ├─ ID
  │   │  ├─ Name
  │   │  ├─ County
  │   │  ├─ Status (badge)
  │   │  ├─ Created Date
  │   │  ├─ License Count
  │   │  └─ Actions (View/Edit/Delete)
  │   └─ Plus buttons at top (Add New, View Deleted)
  │
  └─→ Browser displays table
	 └─→ User sees 6 providers ready to interact
```

---

## Table Layout Diagram

```
┌─────────────────────────────────────────────────────────────────────────┐
│ PROVIDERS                                                               │
├─────────────────────────────────────────────────────────────────────────┤
│                                                                         │
│  [+ Add New Provider]  [...Action buttons...]  [🗑 View Deleted Records]
│                                                                         │
├─────────────────────────────────────────────────────────────────────────┤
│ ID  │ Provider Name              │ County │ Status  │ Created │ Lic │ Act│
├─────┼────────────────────────────┼────────┼─────────┼─────────┼─────┼────┤
│ 1   │ Sunny Days Child Care      │ Fulton │ Active  │ 6m ago  │ 2   │▼ED│
├─────┼────────────────────────────┼────────┼─────────┼─────────┼─────┼────┤
│ 2   │ Little Stars Academy       │ DeKalb │ Active  │ 4m ago  │ 3   │▼ED│
├─────┼────────────────────────────┼────────┼─────────┼─────────┼─────┼────┤
│ 3   │ Rainbow Kids Care          │ Cobb   │ Active  │ 3m ago  │ 2   │▼ED│
├─────┼────────────────────────────┼────────┼─────────┼─────────┼─────┼────┤
│ 4   │ Golden Hour Preschool      │ Henry  │ Active  │ 2m ago  │ 2   │▼ED│
├─────┼────────────────────────────┼────────┼─────────┼─────────┼─────┼────┤
│ 5   │ Bright Futures Learn. Ctr. │ Gwinn. │ Active  │ 1m ago  │ 2   │▼ED│
├─────┼────────────────────────────┼────────┼─────────┼─────────┼─────┼────┤
│ 6   │ Happy Beginnings Daycare   │ Clay.  │ Inactive│ 8m ago  │ 1   │▼ED│
└─────┴────────────────────────────┴────────┴─────────┴─────────┴─────┴────┘

Legend:
  ▼ = View (blue eye icon)
  E = Edit (yellow pencil icon)
  D = Delete (red trash icon)
  Active = green badge
  Inactive = gray badge
  Lic = License count
```

---

## Provider Details Page (Click View)

```
┌──────────────────────────────────────────────────────────────────┐
│ PROVIDER DETAILS                                          [← Back]│
├──────────────────────────────────────────────────────────────────┤
│                                                                  │
│  ┌─────────────────────────┐    ┌──────────────────────────────┐│
│  │  Provider Information   │    │  Active Licenses             ││
│  │                         │    │                              ││
│  │ Name:                   │    │ License #  │ Status │ Expires││
│  │ Sunny Days Child Care   │    │ ───────────┼────────┼─────── ││
│  │                         │    │ CC-FUL-... │ Active │ 2 yrs  ││
│  │ County:                 │    │ CC-FUL-... │ Active │ 6 mo   ││
│  │ Fulton                  │    │                              ││
│  │                         │    │ (2 licenses)                 ││
│  │ Status: ✅ Active       │    │                              ││
│  │                         │    │                              ││
│  │ Created:                │    │  [Edit]  [Delete]  [Restore] ││
│  │ January 15, 2024        │    │                              ││
│  │                         │    │                              ││
│  └─────────────────────────┘    └──────────────────────────────┘│
│                                                                  │
└──────────────────────────────────────────────────────────────────┘
```

---

## Edit Form (Click Edit)

```
┌──────────────────────────────────────────────────────────────┐
│ EDIT PROVIDER                                         [← Cancel]│
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  Provider Name *                                            │
│  ┌──────────────────────────────────────┐                  │
│  │ Sunny Days Child Care                │                  │
│  └──────────────────────────────────────┘                  │
│                                                              │
│  County (Georgia) *                                          │
│  ┌──────────────────────────────────────┐                  │
│  │ Fulton                          ▼    │  (159 counties)  │
│  └──────────────────────────────────────┘                  │
│    Can select: Appling, Atkinson, ... Wynn                 │
│                                                              │
│  Status *                                                    │
│  ┌──────────────────────────────────────┐                  │
│  │ Active                           ▼   │                  │
│  └──────────────────────────────────────┘                  │
│    Options: Active, Inactive, Pending                       │
│                                                              │
│  Created: January 15, 2024 (read-only)                      │
│                                                              │
│  [Save Changes]  [Cancel]                                   │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## Create New Provider Form

```
┌──────────────────────────────────────────────────────────────┐
│ CREATE NEW PROVIDER                                  [← Cancel]│
├──────────────────────────────────────────────────────────────┤
│                                                              │
│  Provider Name *                                            │
│  ┌──────────────────────────────────────┐                  │
│  │  (empty - enter name)                │                  │
│  └──────────────────────────────────────┘                  │
│                                                              │
│  County (Georgia) *                                          │
│  ┌──────────────────────────────────────┐                  │
│  │ -- Select County --              ▼   │                  │
│  └──────────────────────────────────────┘                  │
│    Shows all 159 Georgia counties                           │
│                                                              │
│  Status *                                                    │
│  ┌──────────────────────────────────────┐                  │
│  │ -- Select Status --              ▼   │                  │
│  └──────────────────────────────────────┘                  │
│    Options: Active, Inactive, Pending                       │
│                                                              │
│  [Create]  [Cancel]                                         │
│                                                              │
└──────────────────────────────────────────────────────────────┘
```

---

## Soft-Delete Flow

```
User clicks [Delete] on provider
		│
		▼
Delete Confirmation Page shows
		│
		├─ Provider Name
		├─ Warning message
		└─ [Confirm Delete]  [Cancel]
		│
		▼
User clicks [Confirm Delete]
		│
		▼
ProvidersController.DeleteConfirmed() called
		│
		▼
ProviderService.SoftDeleteProviderAsync()
		│
		├─ Sets IsDeleted = true
		├─ Sets DeletedAt = DateTime.UtcNow
		└─ Logs to AuditLog
		│
		▼
Database saved
		│
		▼
Provider no longer appears in /providers
		│
		├─ But still in database!
		└─ Accessible via [View Deleted Records]
		│
		▼
User can [Restore] to undo soft-delete
```

---

## Restore from Deleted

```
User clicks [View Deleted Records]
		│
		▼
Shows table of soft-deleted providers
		│
		├─ Same columns as main list
		├─ Deleted date column
		└─ [Restore] button instead of [Delete]
		│
		▼
User clicks [Restore]
		│
		▼
ProviderService.RestoreProviderAsync()
		│
		├─ Sets IsDeleted = false
		├─ Logs to AuditLog
		└─ Removes DeletedAt
		│
		▼
Provider reappears in /providers
```

---

## Data Query Filter

```
Index View Request:
		│
		▼
DbContext.Providers (auto-applied filter)
		│
		▼
.Where(p => !p.IsDeleted)  ◄─── Global Query Filter
		│
		▼
Only returns 5 active providers (6 is inactive but not deleted)

Deleted View Request:
		│
		▼
DbContext.Providers.IgnoreQueryFilters()  ◄─── Override filter
		│
		▼
.Where(p => p.IsDeleted)
		│
		▼
Returns only deleted providers (if any)
```

---

## Database Schema Relationship

```
┌─────────────────┐              ┌─────────────────┐
│   PROVIDER      │              │    LICENSE      │
├─────────────────┤   1      ∞   ├─────────────────┤
│ ProviderId (PK) │──────────────│ LicenseId (PK)  │
│ ProviderName    │              │ ProviderId (FK) │
│ County          │              │ LicenseNumber   │
│ Status          │              │ LicenseStatus   │
│ CreatedDate     │              │ ExpirationDate  │
│ IsDeleted       │              │ CreatedDate     │
└─────────────────┘              │ IsDeleted       │
								 └─────────────────┘

A Provider can have multiple Licenses:
  Provider 1 (Sunny Days) ──┬─→ License 1
							└─→ License 2

  Provider 2 (Little Stars) ┬─→ License 3
							├─→ License 4
							└─→ License 5

  Provider 3-6 ... similar relationships
```

---

## Count Summary Display

```
On Page Load:
		│
		▼
Query: SELECT COUNT(*) FROM License WHERE ProviderId = X
		│
		├─ Provider 1: 2 licenses ──→ Display "2" badge
		├─ Provider 2: 3 licenses ──→ Display "3" badge
		├─ Provider 3: 2 licenses ──→ Display "2" badge
		├─ Provider 4: 2 licenses ──→ Display "2" badge
		├─ Provider 5: 2 licenses ──→ Display "2" badge
		└─ Provider 6: 1 license  ──→ Display "1" badge
		│
		▼
Render in table as colored badges
```

---

## Status Badge Display

```
Status Field Value  │  Display
────────────────────┼──────────────────
"Active"            │  🟢 Active (green)
"Inactive"          │  ⚫ Inactive (gray)
"Pending"           │  🟡 Pending (yellow)
```

---

## Sample Data Loading on Startup

```
Application Start
		│
		▼
Program.cs: DbContext.Database.Migrate()
		│
		├─ Check if migrations applied
		├─ If not, apply all migrations
		└─ Create tables if needed
		│
		▼
DbInitializer.Initialize(dbContext)
		│
		├─ Check: if (context.Providers.Any())
		│
		├─ If providers exist:
		│  └─ Return (skip seeding)
		│
		└─ If empty:
		   ├─ Create 6 providers
		   ├─ Save to database
		   ├─ Create 12 licenses
		   ├─ Save to database
		   └─ Log: "Database seeding completed"
		│
		▼
Application ready
		│
		▼
User accesses /providers
		│
		▼
Sees 6 providers in table ✅
```

---

**This architecture ensures a clean, organized, and maintainable provider management system with full CRUD and soft-delete capabilities!**
