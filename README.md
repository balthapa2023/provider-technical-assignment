# Provider Assignment – Starter Solution

This is a starter project for the 24‑hour take‑home technical assignment.

- ASP.NET Core MVC application
- Builds and runs successfully
- No business logic implemented
- SQLite and EF Core packages installed

All application logic, database schema, and optional frontend enhancements
are expected to be implemented by the candidate.

Project Overview
This project manages:

Providers (organizations or individuals)

Licenses associated with each provider

Soft delete for both Providers and Licenses

Edit, Delete, Restore operations

Active vs Deleted license views

The system uses Entity Framework Core, SQLite, and ASP.NET Core MVC.

Architecture
Layers
Models → Data structures (Provider, ProviderLicense)

Data → ApplicationDbContext (EF Core)

Controllers → ProvidersController, LicensesController

Views → Razor pages for CRUD operations

Database
SQLite database

EF Core migrations

Soft delete implemented using global query filters

Provider
Represents a provider entity.

Key fields:

Id

ProviderName

County

Status

IsDeleted (soft delete)

Licenses (navigation property)

ProviderLicense
Represents a license belonging to a provider.

Key fields:

Id

ProviderId

LicenseNumber

LicenseStatus

ExpirationDate

IsDeleted (soft delete)

DeletedAt

ApplicationDbContext
Key Features
Configures Provider → Licenses relationship

Enables soft delete using global query filters

Ensures default values for IsDeleted
