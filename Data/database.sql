-- ============================================================
-- Provider Technical Assignment
-- SQLite database reference and verification queries
-- ============================================================

-- The application schema is created and maintained through
-- Entity Framework Core migrations.
--
-- This script documents useful queries for reviewing and
-- verifying the Provider and License data.


-- ============================================================
-- 1. STANDARD PROVIDER QUERY
-- ============================================================
-- Returns providers that have NOT been soft deleted.
-- This mirrors the normal behavior of the application.

SELECT
    ProviderId,
    Name,
    County,
    Status,
    IsDeleted,
    CreatedDate,
    DeletedDate
FROM Providers
WHERE IsDeleted = 0
ORDER BY Name;


-- ============================================================
-- 2. AUDIT / TROUBLESHOOTING QUERY
-- ============================================================
-- Soft-deleted providers remain physically stored in the
-- database and can still be queried for audit purposes.

SELECT
    ProviderId,
    Name,
    County,
    Status,
    IsDeleted,
    CreatedDate,
    DeletedDate
FROM Providers
WHERE IsDeleted = 1
ORDER BY DeletedDate DESC;


-- ============================================================
-- 3. PROVIDERS WITH THEIR LICENSES
-- ============================================================

SELECT
    p.ProviderId,
    p.Name AS ProviderName,
    p.County,
    p.Status AS ProviderStatus,
    l.LicenseId,
    l.LicenseNumber,
    l.Status AS LicenseStatus,
    l.ExpirationDate
FROM Providers p
LEFT JOIN Licenses l
    ON p.ProviderId = l.ProviderId
WHERE p.IsDeleted = 0
ORDER BY p.Name, l.LicenseNumber;


-- ============================================================
-- 4. ACTIVE PROVIDERS WITH ACTIVE LICENSES
-- ============================================================

SELECT
    p.ProviderId,
    p.Name AS ProviderName,
    l.LicenseNumber,
    l.Status AS LicenseStatus,
    l.ExpirationDate
FROM Providers p
INNER JOIN Licenses l
    ON p.ProviderId = l.ProviderId
WHERE p.IsDeleted = 0
  AND p.Status = 'Active'
  AND l.Status = 'Active'
ORDER BY p.Name;


-- ============================================================
-- 5. ACTIVE PROVIDERS WITH EXPIRED LICENSES
-- ============================================================
-- License expiration is determined from ExpirationDate.
-- It is intentionally separate from the license Status field.

SELECT
    p.ProviderId,
    p.Name AS ProviderName,
    l.LicenseNumber,
    l.Status AS LicenseStatus,
    l.ExpirationDate
FROM Providers p
INNER JOIN Licenses l
    ON p.ProviderId = l.ProviderId
WHERE p.IsDeleted = 0
  AND p.Status = 'Active'
  AND date(l.ExpirationDate) < date('now')
ORDER BY p.Name, l.ExpirationDate;


-- ============================================================
-- 6. ALL PROVIDERS INCLUDING SOFT-DELETED RECORDS
-- ============================================================
-- Useful for audit and troubleshooting.

SELECT
    ProviderId,
    Name,
    County,
    Status,
    IsDeleted,
    CreatedDate,
    DeletedDate
FROM Providers
ORDER BY ProviderId;