# 📊 License Management Visual Guide

## 🎯 Application Flow

```
┌─────────────────────────────────────────────────────────────┐
│                    LANDING PAGE                             │
│              (Public - No Auth Required)                    │
└────────────────────┬────────────────────────────────────────┘
					 │
					 ↓
		 ┌───────────────────────┐
		 │  LOGIN REQUIRED       │
		 │ [Authorize] Check     │
		 └────────┬──────────────┘
				  │
		┌─────────┴─────────┐
		↓                   ↓
	┌─────────┐         ┌──────────┐
	│PROVIDERS│         │ LICENSE  │
	│Management       │Management│
	└─────────┘       └──────────┘
		↓                   ↓
	┌─────────────────────────────┐
	│   Provider Details Page     │
	├─────────────────────────────┤
	│ • Provider Info (Left)      │
	│ • Associated Licenses Table │
	│ • "View Licenses" Button    │
	│ • "Add License" Button      │
	│ • Quick Info Panel (Right)  │
	│   - Total Licenses          │
	│   - Active Licenses         │
	│   - Expired Licenses        │
	└─────────────────────────────┘
```

---

## 🔄 License Lifecycle

```
┌──────────────┐
│   LICENSE    │  ← Initial State
│   CREATED    │    (All new licenses)
└──────┬───────┘
	   │
	   ↓ (Time passes)
┌──────────────────────┐
│   LICENSE            │
│   ACTIVELY IN USE    │ ← Main State
│   (1-3 years)        │    (Normal operation)
└──────┬───────────────┘
	   │ (Renewal time)
	   ├─→ RENEW (Edit → extend expiration)
	   │
	   ↓ (Expiration date reached)
┌──────────────────────┐
│   LICENSE            │
│   EXPIRING SOON      │ ← Yellow Alert
│   (<30 days)         │    (Needs action)
└──────┬───────────────┘
	   │
	   ↓
┌──────────────────────┐
│   LICENSE            │
│   EXPIRED            │ ← Red Alert
│   (Action needed)    │    (Non-compliant)
└──────┬───────────────┘
	   │
	   ↓ (Optional: Delete old expired licenses)
┌──────────────────────┐
│   LICENSE            │
│   SOFT-DELETED       │ ← Archived
│   (Archived)         │    (Kept for audit)
└──────────────────────┘
```

---

## 📱 UI Navigation Map

```
╔═════════════════════════════════════════════════════════════╗
║                   MAIN NAVIGATION                           ║
╠═════════════════════════════════════════════════════════════╣
║ HOME → PROVIDER MANAGEMENT → LICENSE MANAGEMENT → AUDIT LOG ║
╚═════════════════════════════════════════════════════════════╝

Provider Management                License Management
├─ All Providers                   ├─ All Licenses
│  └─ [List table]                 │  └─ [List table]
│     ├─ View Details              │     ├─ By Provider
│     ├─ Edit                       │     ├─ Create New
│     └─ Delete                     │     ├─ Edit
│                                   │     └─ Delete
├─ Create Provider                 │
│  └─ [Form]                       ├─ License Details
│                                  │  ├─ View Info
├─ Provider Details               │  ├─ Edit License
│  ├─ [Provider Info Card]        │  ├─ Delete License
│  ├─ [Associated Licenses]       │  └─ View Provider
│  ├─ "Add License" Button →→→→→┐
│  ├─ "View Licenses" Button  ┌┐ │
│  └─ [Quick Info Panel]      ││ │
│                             ││ │
							  └┼─┴─→ License/Create
							   └────→ License/ByProvider
```

---

## 📊 Statistics Dashboard

### Provider Details Page (Right Sidebar)
```
┌─────────────────────────────────────┐
│      QUICK INFO                     │
├─────────────────────────────────────┤
│ Total Licenses                      │
│ ┌────────────────────────────────┐  │
│ │            3                   │  │ ← Count of all licenses
│ └────────────────────────────────┘  │
│                                     │
│ Active Licenses                     │
│ ┌────────────────────────────────┐  │
│ │            3                   │  │ ← Only Status="Active"
│ └────────────────────────────────┘  │
│                                     │
│ Expired Licenses                    │
│ ┌────────────────────────────────┐  │
│ │            0                   │  │ ← Only Status="Expired"
│ └────────────────────────────────┘  │
└─────────────────────────────────────┘
```

### Provider Licenses Page (Top)
```
┌─────────────┐  ┌─────────────┐  ┌─────────────┐  ┌─────────────┐
│   TOTAL     │  │   ACTIVE    │  │  SUSPENDED  │  │   EXPIRED   │
├─────────────┤  ├─────────────┤  ├─────────────┤  ├─────────────┤
│      5      │  │      3      │  │      2      │  │      0      │
├─────────────┤  ├─────────────┤  ├─────────────┤  ├─────────────┤
│   All       │  │  Ready to   │  │ Temporarily │  │   Past due  │
│  licenses   │  │    use      │  │  inactive   │  │   renewal   │
└─────────────┘  └─────────────┘  └─────────────┘  └─────────────┘
```

---

## 🔗 Provider ↔ License Relationship

```
ONE PROVIDER CAN HAVE MANY LICENSES
┌──────────────────┐
│     PROVIDER     │
│  "Sunny Days"    │
│    (ID: 1)       │
└────────┬─────────┘
		 │
	┌────┴────┬─────────┬──────────┐
	│          │         │          │
	↓          ↓         ↓          ↓
┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐
│LICENSE1│ │LICENSE2│ │LICENSE3│ │LICENSE4│
│LIC-001 │ │LIC-002 │ │LIC-003 │ │LIC-004 │
│Active  │ │Active  │ │Expired │ │Susp.   │
└────────┘ └────────┘ └────────┘ └────────┘


WHEN PROVIDER DELETED, ALL RELATED LICENSES ALSO SOFT-DELETE
┌──────────────────┐
│ PROVIDER DELETED │
│  (IsDeleted=1)   │
└────────┬─────────┘
		 │
	┌────┴────┬─────────┬──────────┐
	│          │         │          │
	↓          ↓         ↓          ↓
┌────────┐ ┌────────┐ ┌────────┐ ┌────────┐
│LICENSE1│ │LICENSE2│ │LICENSE3│ │LICENSE4│
│       │ │       │ │       │ │       │
│Deleted│ │Deleted│ │Deleted│ │Deleted│
└────────┘ └────────┘ └────────┘ └────────┘
```

---

## 🎨 Status Badge Color System

```
LICENSE STATUS          COLOR    MEANING
┌──────────────────────┬────────┬──────────────────────┐
│ Active               │ 🟢     │ License is valid     │
│ (most common)        │ Green  │ Ready to use         │
├──────────────────────┼────────┼──────────────────────┤
│ Suspended            │ 🟡     │ Temporarily inactive │
│ (temporary hold)     │ Yellow │ May be renewed       │
├──────────────────────┼────────┼──────────────────────┤
│ Expired              │ 🔴     │ Past expiration      │
│ (needs action)       │ Red    │ Needs renewal ASAP   │
└──────────────────────┴────────┴──────────────────────┘

EXPIRATION DATE WARNINGS
┌──────────────────────┬────────┬──────────────────────┐
│ Expired (Now)        │ 🔴     │ Immediate action     │
│ (icon + text)        │ Red    │ required             │
├──────────────────────┼────────┼──────────────────────┤
│ Expiring Soon        │ 🟡     │ Within 30 days       │
│ (<30 days)           │ Yellow │ Start renewal        │
├──────────────────────┼────────┼──────────────────────┤
│ Valid                │ ⚪     │ > 30 days left       │
│ (> 30 days)          │ Gray   │ Normal status        │
└──────────────────────┴────────┴──────────────────────┘
```

---

## 📋 License Creation Form

```
CREATE NEW LICENSE
┌──────────────────────────────────────────┐
│ Provider *                               │
│ ┌──────────────────────────────────────┐ │ ← Dropdown showing:
│ │ -- Select a Provider --              │ │    • Sunny Days
│ │ ☒ Sunny Days                         │ │    • Little Stars
│ │   Little Stars Academy               │ │    • etc.
│ │   Happy Kids Center                  │ │    (Non-deleted only)
│ └──────────────────────────────────────┘ │
│                                          │
│ License Number *                         │
│ ┌──────────────────────────────────────┐ │
│ │ LIC-2024-001                         │ │ ← Min 3, max 100 chars
│ └──────────────────────────────────────┘ │
│                                          │
│ License Status *                         │
│ ┌──────────────────────────────────────┐ │
│ │ ☒ Active                             │ │ ← Choose:
│ │   Suspended                          │ │    • Active
│ │   Expired                            │ │    • Suspended
│ └──────────────────────────────────────┘ │    • Expired
│                                          │
│ Expiration Date *                        │
│ ┌──────────────────────────────────────┐ │
│ │ 2025-12-31                           │ │ ← Date picker
│ └──────────────────────────────────────┘ │    (future date only)
│                                          │
│ [Create License] [Back to List]          │
└──────────────────────────────────────────┘
```

---

## 🔄 Edit License Form

```
EDIT LICENSE
┌──────────────────────────────────────────┐
│ License Number                           │
│ ┌──────────────────────────────────────┐ │
│ │ LIC-2024-001                         │ │ ← Can change
│ └──────────────────────────────────────┘ │
│                                          │
│ Associated Provider                      │
│ ┌──────────────────────────────────────┐ │
│ │ ☒ Sunny Days                         │ │ ← Can reassign
│ │   Little Stars Academy               │ │    to different
│ │   Happy Kids Center                  │ │    provider
│ └──────────────────────────────────────┘ │
│                                          │
│ License Status                           │
│ ┌──────────────────────────────────────┐ │
│ │ ☒ Active                             │ │ ← Can change
│ │   Suspended                          │ │
│ │   Expired                            │ │
│ └──────────────────────────────────────┘ │
│                                          │
│ Expiration Date                          │
│ ┌──────────────────────────────────────┐ │
│ │ 2025-12-31                           │ │ ← Can change
│ └──────────────────────────────────────┘ │
│                                          │
│ [Update] [Delete] [Back to List]         │
└──────────────────────────────────────────┘
```

---

## 📊 License List Table

```
ALL LICENSES
┌─────────────┬──────────────────┬──────────┬─────────────┬──────────────┬─────────────┐
│ License #   │ Provider         │ Status   │ Expiration  │ Created      │ Actions     │
├─────────────┼──────────────────┼──────────┼─────────────┼──────────────┼─────────────┤
│ LIC-2024-01 │ Sunny Days       │ 🟢Active │ 2025-12-31  │ 2024-01-15   │ 👁 ✏️ 🗑️  │
│ LIC-2024-02 │ Little Stars     │ 🟢Active │ 2024-02-15  │ 2024-01-10   │ 👁 ✏️ 🗑️  │
│ LIC-2024-03 │ Happy Kids       │ 🟡Susp.  │ 2025-06-30  │ 2024-01-08   │ 👁 ✏️ 🗑️  │
│ LIC-2024-04 │ Rainbow Academy  │ 🔴Exp.   │ 2023-12-31⚠️ │ 2023-12-15   │ 👁 ✏️ 🗑️  │
│ LIC-2024-05 │ Bright Future    │ 🟢Active │ 2025-03-15⚠️ │ 2024-01-01   │ 👁 ✏️ 🗑️  │
└─────────────┴──────────────────┴──────────┴─────────────┴──────────────┴─────────────┘

Legend: 👁=View  ✏️=Edit  🗑️=Delete  ⚠️=Alert  🟢=Active  🟡=Suspended  🔴=Expired
```

---

## 🧭 Data Flow Diagram

```
USER INTERACTION
		│
		├─→ Click "Add License" on Provider Page
		│   ↓
		│   License/Create (with ProviderId pre-selected)
		│   ↓
		│   Fill form
		│   ↓
		│   POST Create()
		│
		├─→ Click "Edit" on License
		│   ↓
		│   License/Edit/{id}
		│   ↓
		│   Modify fields
		│   ↓
		│   POST Edit()
		│   ↓
		│   Provider relationship can change
		│
		└─→ Click "View Licenses" on Provider
			↓
			License/ByProvider/{ProviderId}
			↓
			Shows all licenses for provider
			↓
			Statistics calculated in real-time


SERVER PROCESSING
		│
		├─→ CREATE
		│   ├─ Validate input
		│   ├─ Check provider exists
		│   ├─ Insert to Licenses table
		│   ├─ Insert to AuditLogs table
		│   └─ Return to Index
		│
		├─→ UPDATE
		│   ├─ Validate input
		│   ├─ Check provider exists
		│   ├─ Compare old vs new values
		│   ├─ Update Licenses table
		│   ├─ Insert to AuditLogs (with old/new)
		│   └─ Return to Index
		│
		└─→ DELETE
			├─ Load license
			├─ Set IsDeleted = true
			├─ Set DeletedAt = now
			├─ Update Licenses table
			├─ Insert to AuditLogs
			└─ Return to Index


DATABASE UPDATES
		│
		├─→ Licenses
		│   ├─ New row (Create)
		│   ├─ Update row (Update)
		│   └─ Soft-delete (Delete)
		│
		└─→ AuditLogs
			├─ Insert audit record
			├─ UserId captured
			├─ Timestamp recorded
			├─ Action logged (Create/Edit/Delete)
			└─ Before/after values stored (for Edit)
```

---

## ✅ Validation Rules

```
┌──────────────────────┬────────────┬─────────────────────────┐
│ Field                │ Type       │ Validation Rules        │
├──────────────────────┼────────────┼─────────────────────────┤
│ ProviderId           │ Int        │ [Required]              │
│                      │            │ Must exist in Providers │
├──────────────────────┼────────────┼─────────────────────────┤
│ LicenseNumber        │ String     │ [Required]              │
│                      │            │ [StringLength(100)]     │
│                      │            │ Min 3 characters        │
├──────────────────────┼────────────┼─────────────────────────┤
│ LicenseStatus        │ String     │ [Required]              │
│                      │            │ Active|Suspended|       │
│                      │            │ Expired only            │
├──────────────────────┼────────────┼─────────────────────────┤
│ ExpirationDate       │ DateTime   │ [Required]              │
│                      │            │ [DataType(Date)]        │
│                      │            │ [Future] Custom attr.   │
│                      │            │ Must be future date     │
└──────────────────────┴────────────┴─────────────────────────┘
```

---

## 🔒 Security Flow

```
USER REQUEST
		│
		↓
   [Authorize] Check
		│
	┌───┴────┐
	│        │
   YES      NO
	│        │
	│        ↓
	│    Redirect to Login
	│
	↓
Route to Controller
		│
		↓
[ValidateAntiForgeryToken]
Check (POST only)
		│
	┌───┴────────┐
	│            │
   VALID      INVALID
	│            │
	│            ↓
	│        Error 400
	│
	↓
Business Logic
		│
		├─→ Input Validation
		│   ├─ Check not null
		│   ├─ Check format
		│   ├─ Check constraints
		│   └─ Return with errors if invalid
		│
		├─→ Security Checks
		│   ├─ Verify provider exists
		│   ├─ Check soft-delete status
		│   └─ Prevent unauthorized access
		│
		├─→ Database Operation
		│   ├─ Parameterized queries (prevents SQL injection)
		│   ├─ Foreign key constraints
		│   └─ Transaction handling
		│
		└─→ Audit Logging
			├─ Record action
			├─ Capture user context
			├─ Store old/new values
			└─ Timestamp for compliance
```

---

## 📞 Quick Reference

| Action | Button | Page | Result |
|--------|--------|------|--------|
| Create | "Add License" | Provider Details | License/Create with ProviderId |
| View All | "All Licenses" | Navigation | License/Index |
| View Provider | Provider Name | License list | Provider/Details |
| By Provider | "View Licenses" | Provider Details | License/ByProvider |
| Edit | Pencil ✏️ | License list | License/Edit |
| Delete | Trash 🗑️ | License list | License/Delete |
| Details | Eye 👁 | License list | License/Details |

---

**Created**: 2024
**Status**: ✅ Complete & Ready for Use
**Version**: 1.0
