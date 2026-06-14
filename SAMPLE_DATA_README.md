# Sample Data: 4 Active Providers with Active Licenses

**Updated**: Database seed data modified
**File Modified**: `Data/DbInitializer.cs`
**Database**: Automatically recreated with new seed data on next application run

---

## Overview

The application now seeds exactly **4 active providers**, each with **2 active licenses**, for a clean, professional demonstration table view.

---

## Providers with Active Licenses

### 1. **Sunny Days Child Care** 
- **Location**: Fulton County, Georgia
- **Status**: ✅ Active
- **Active Licenses**: 2
  - `CC-FUL-2024-001` → Expires in 2 years
  - `CC-FUL-2024-002` → Expires in 6 months

### 2. **Little Stars Academy**
- **Location**: DeKalb County, Georgia
- **Status**: ✅ Active
- **Active Licenses**: 2
  - `CC-DEK-2024-001` → Expires in 1 year
  - `CC-DEK-2024-002` → Expires in 20 days (soon!)

### 3. **Rainbow Kids Care**
- **Location**: Cobb County, Georgia
- **Status**: ✅ Active
- **Active Licenses**: 2
  - `CC-COB-2024-001` → Expires in 2 years, 3 months
  - `CC-COB-2024-002` → Expires in 9 months

### 4. **Golden Hour Preschool**
- **Location**: Henry County, Georgia
- **Status**: ✅ Active
- **Active Licenses**: 2
  - `CC-HEN-2024-001` → Expires in 1 year, 6 months
  - `CC-HEN-2024-002` → Expires in 90 days

---

## Total Sample Data

| Metric | Count |
|--------|-------|
| **Providers** | 4 |
| **All Providers Status** | Active |
| **Total Licenses** | 8 |
| **All Licenses Status** | Active |
| **Counties Represented** | 4 (Fulton, DeKalb, Cobb, Henry) |

---

## Database Schema

```
Provider Table:
├── ProviderId
├── ProviderName ✅
├── County ✅ (Georgia)
├── Status ✅ (Active)
├── CreatedDate
└── IsDeleted

License Table (8 total):
├── LicenseId
├── ProviderId → [1, 2, 3, 4]
├── LicenseNumber ✅
├── LicenseStatus ✅ (All Active)
├── ExpirationDate ✅ (Varied dates)
├── CreatedDate
└── IsDeleted
```

---

## Features Demonstrated

✅ **Active Providers List** - Index view shows all 4 providers  
✅ **License Count Badge** - Shows "2" licenses per provider  
✅ **Details View** - Click [View] to see provider + licenses  
✅ **Expiration Dates** - Variety of expiration dates (10 days to 2+ years)  
✅ **Status Badges** - All providers show "Active" status  
✅ **County Dropdown** - All 4 are in Georgia (can edit counties)  
✅ **CRUD Operations** - Can create/edit/delete (soft-delete)  
✅ **County Representation** - 4 different Georgia counties shown  

---

## How to View the Sample Data

### Step 1: Start the Application
```bash
dotnet run
```

### Step 2: Database Recreation
- Old database deleted automatically
- App runs migrations
- `DbInitializer.Initialize()` seeds these 4 providers + 8 licenses

### Step 3: Navigate to Providers
- Open: `https://localhost:####/providers`
- See table with 4 rows:
  - Sunny Days Child Care | Fulton | Active | 2 licenses
  - Little Stars Academy | DeKalb | Active | 2 licenses
  - Rainbow Kids Care | Cobb | Active | 2 licenses
  - Golden Hour Preschool | Henry | Active | 2 licenses

### Step 4: View Provider Details
- Click [View] button on any row
- See provider info + all 2 active licenses
- Info card shows: Name, County, Status, Created Date
- License sidebar shows: License #, Status (Active), Expiration Date

---

## Sample License Expiration Scenarios

| Provider | License | Expires | Days Remaining | Notes |
|----------|---------|---------|---|---|
| Little Stars | CC-DEK-2024-002 | Today + 20 days | ⚠️ Short | Good demo for renewal alerts |
| Golden Hour | CC-HEN-2024-002 | Today + 90 days | ⏰ Medium | Middle-term expiration |
| Sunny Days | CC-FUL-2024-001 | Today + 2 years | ✅ Long | No urgent renewal needed |
| Rainbow Kids | CC-COB-2024-001 | Today + 2y, 3m | ✅ Very Long | Long-term license |

---

## Data Integrity

✅ **No Deleted Records** - All providers and licenses have `IsDeleted = false`  
✅ **All Active** - No pending or inactive providers in demo set  
✅ **Proper Relationships** - Each provider linked to exactly 2 licenses  
✅ **Valid Dates** - All expiration dates in future (relative to seed time)  
✅ **Unique License Numbers** - No duplicate license numbers  
✅ **County Validation** - All counties exist in AppConstants  

---

## Editing & Testing the Data

### Create a New Provider
1. Click [Create] button
2. Enter Provider Name: `"My New Provider"`
3. Select County: `Fulton` (or any Georgia county)
4. Select Status: `Active`
5. Click Create
6. New provider appears in table with 0 licenses

### Edit a Provider
1. Click [Edit] on any provider
2. Change any field (e.g., County from Fulton to DeKalb)
3. Click Save
4. Changes reflected in table

### View Provider Details
1. Click [View] on any provider
2. See full provider info + licensing sidebar
3. Shows all active licenses for that provider

### Delete (Soft-Delete) a Provider
1. Click [Delete] on any provider
2. Confirm deletion
3. Provider moves to "View Deleted Records"
4. Original data remains in database (not permanently deleted)

### View Deleted Records
1. Click [View Deleted Records] (red button at top)
2. See providers marked for deletion
3. Click [Restore] to undo soft-delete
4. Provider returns to active list

---

## Database File Location

**Before**: `provider_assignment.db` (deleted)  
**After**: Automatically recreated at startup with new seed data  
**Path**: `C:\Users\yida\Divya2026\provider-technical-assignment\provider_assignment.db`

---

## Next Steps (Optional)

- ✅ Add more providers to test pagination
- ✅ Soft-delete a provider and restore it
- ✅ Create a new provider with Edit form
- ✅ Click county dropdown to see all 159 Georgia counties
- ✅ View provider details to see license sidebar
- ✅ Test search/filter by county (if implemented)

---

**Status**: ✅ **READY FOR DEMONSTRATION**

The application now displays a clean table with 4 active providers, each with 2 real active licenses, representing a production-like sample dataset.
