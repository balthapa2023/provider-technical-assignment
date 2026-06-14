# 📊 Dummy Data Reference Guide

## Complete Sample Dataset

### PROVIDERS TABLE (6 Rows)

```
┌────┬──────────────────────────────────────┬──────────┬──────────┬───────────┬─────────────┐
│ ID │ Provider Name                        │ County   │ Status   │ Created   │ IsDeleted   │
├────┼──────────────────────────────────────┼──────────┼──────────┼───────────┼─────────────┤
│ 1  │ Sunny Days Child Care                │ Fulton   │ Active   │ 6m ago    │ false       │
│ 2  │ Little Stars Academy                 │ DeKalb   │ Active   │ 4m ago    │ false       │
│ 3  │ Rainbow Kids Care                    │ Cobb     │ Active   │ 3m ago    │ false       │
│ 4  │ Golden Hour Preschool                │ Henry    │ Active   │ 2m ago    │ false       │
│ 5  │ Bright Futures Learning Center       │ Gwinnett │ Active   │ 1m ago    │ false       │
│ 6  │ Happy Beginnings Daycare             │ Clayton  │ Inactive │ 8m ago    │ false       │
└────┴──────────────────────────────────────┴──────────┴──────────┴───────────┴─────────────┘
```

### LICENSES TABLE (12 Rows)

```
┌───┬──────────┬──────────────────┬────────────┬────────────────────┐
│ID │ Provider │ License Number   │ Status     │ Expiration Date    │
├───┼──────────┼──────────────────┼────────────┼────────────────────┤
│ 1 │ 1        │ CC-FUL-2024-001 │ Active     │ +2 years           │
│ 2 │ 1        │ CC-FUL-2024-002 │ Active     │ +6 months          │
│ 3 │ 2        │ CC-DEK-2024-001 │ Active     │ +1 year            │
│ 4 │ 2        │ CC-DEK-2024-002 │ Active     │ +90 days           │
│ 5 │ 2        │ CC-DEK-2024-003 │ Active     │ +3 months          │
│ 6 │ 3        │ CC-COB-2024-001 │ Active     │ +2 years, 3 months │
│ 7 │ 3        │ CC-COB-2024-002 │ Active     │ +9 months          │
│ 8 │ 4        │ CC-HEN-2024-001 │ Active     │ +1 year, 6 months  │
│ 9 │ 4        │ CC-HEN-2024-002 │ Active     │ +120 days          │
│10 │ 5        │ CC-GWI-2024-001 │ Active     │ +18 months         │
│11 │ 5        │ CC-GWI-2024-002 │ Active     │ +8 months          │
│12 │ 6        │ CC-CLA-2024-001 │ Active     │ +45 days           │
└───┴──────────┴──────────────────┴────────────┴────────────────────┘
```

---

## Data by Provider

### Provider #1: Sunny Days Child Care
- **County**: Fulton
- **Status**: Active ✅
- **Created**: 6 months ago
- **Licenses**: 2
  - CC-FUL-2024-001 (expires in 2 years)
  - CC-FUL-2024-002 (expires in 6 months)

### Provider #2: Little Stars Academy
- **County**: DeKalb
- **Status**: Active ✅
- **Created**: 4 months ago
- **Licenses**: 3
  - CC-DEK-2024-001 (expires in 1 year)
  - CC-DEK-2024-002 (expires in 90 days) ⚠️ Soon
  - CC-DEK-2024-003 (expires in 3 months)

### Provider #3: Rainbow Kids Care
- **County**: Cobb
- **Status**: Active ✅
- **Created**: 3 months ago
- **Licenses**: 2
  - CC-COB-2024-001 (expires in 2 years, 3 months)
  - CC-COB-2024-002 (expires in 9 months)

### Provider #4: Golden Hour Preschool
- **County**: Henry
- **Status**: Active ✅
- **Created**: 2 months ago
- **Licenses**: 2
  - CC-HEN-2024-001 (expires in 1 year, 6 months)
  - CC-HEN-2024-002 (expires in 120 days)

### Provider #5: Bright Futures Learning Center
- **County**: Gwinnett
- **Status**: Active ✅
- **Created**: 1 month ago
- **Licenses**: 2
  - CC-GWI-2024-001 (expires in 18 months)
  - CC-GWI-2024-002 (expires in 8 months)

### Provider #6: Happy Beginnings Daycare
- **County**: Clayton
- **Status**: Inactive ⏸️
- **Created**: 8 months ago
- **Licenses**: 1
  - CC-CLA-2024-001 (expires in 45 days)

---

## Statistics

### By Status
- **Active Providers**: 5
- **Inactive Providers**: 1
- **Deleted Providers**: 0
- **Total Providers**: 6

### By License Count
- **1 License**: 1 provider
- **2 Licenses**: 4 providers
- **3 Licenses**: 1 provider
- **Total Licenses**: 12

### By County
- **Fulton**: 1 provider
- **DeKalb**: 1 provider
- **Cobb**: 1 provider
- **Henry**: 1 provider
- **Gwinnett**: 1 provider
- **Clayton**: 1 provider

### By License Status
- **Active Licenses**: 12
- **Expired Licenses**: 0
- **Inactive Licenses**: 0

### Expiration Timeline
| Range | Count | Examples |
|-------|-------|----------|
| 0-3 months | 1 | CC-CLA-2024-001 (45 days) |
| 3-6 months | 2 | CC-DEK-2024-003, CC-FUL-2024-002 |
| 6-12 months | 3 | CC-COB-2024-002, CC-DEK-2024-001, CC-GWI-2024-002 |
| 1-2 years | 4 | CC-HEN-2024-002, CC-HEN-2024-001, CC-GWI-2024-001, CC-DEK-2024-002 |
| 2+ years | 2 | CC-COB-2024-001, CC-FUL-2024-001 |

---

## Display Order

When you view the provider list, you'll see them in this order:
1. Sunny Days Child Care (2 licenses)
2. Little Stars Academy (3 licenses) ← Most licenses
3. Rainbow Kids Care (2 licenses)
4. Golden Hour Preschool (2 licenses)
5. Bright Futures Learning Center (2 licenses)
6. Happy Beginnings Daycare - Inactive (1 license)

---

## Testing Scenarios Possible

✅ **View Active Providers** - 5 shown  
✅ **View Inactive Providers** - 1 shown  
✅ **Check License Counts** - Verify accuracy  
✅ **Edit Provider** - Change details  
✅ **View Details** - See all licenses  
✅ **Test Expiration Dates** - Range from 45 days to 2+ years  
✅ **Create New** - Add your own  
✅ **Delete/Restore** - Soft-delete functionality  

---

## Notes

- All providers have `IsDeleted = false`
- All licenses have `IsDeleted = false`
- Creation dates use realistic offsets (1-8 months ago)
- Expiration dates are calculated from today
- License numbers follow pattern: `CC-[COUNTY]-2024-[SEQUENCE]`
- Mix of expiration scenarios for testing

---

**Ready to View**: This data will automatically seed into the database when you start the application!
