# Entity Framework Core Migration Guide

This document outlines the migration from Dapper to Entity Framework Core in the ConsularServices project.

## Changes Made

### 1. NuGet Packages Added
- `Microsoft.EntityFrameworkCore` (9.0.0)
- `Microsoft.EntityFrameworkCore.Design` (9.0.0)
- `Npgsql.EntityFrameworkCore.PostgreSQL` (9.0.2)
- `Microsoft.EntityFrameworkCore.Tools` (9.0.0)

### 2. DbContext Created
- Created `ConsularDbContext` in `/Data/ConsularDbContext.cs`
- Configured PostgreSQL-specific features (arrays, identity columns)
- Added seed data for permissions
- Configured relationships and composite keys

### 3. Entity Classes Updated
All entity classes were updated with:
- Proper `using` statements for EF Core
- `[Key]` and `[DatabaseGenerated]` attributes
- Fixed nullable reference types with `required` keyword or `?` for optional properties

### 4. Repository Classes
- Created `ServiceRepositoryEF.cs` - Entity Framework implementation
- Created `UserRepositoryEF.cs` - Entity Framework implementation
- Both implement the same interfaces as the original Dapper versions

### 5. Dependency Injection
- Updated `DependencyInjection.cs` to configure EF Core DbContext
- Modified `Program.cs` to pass connection string to DI configuration

## Database Schema Generation

The `script.sql` file is **no longer needed**. Entity Framework Core will generate the database schema automatically from your entity classes.

### To Apply Database Changes:

```bash
# Navigate to project root
cd /Users/shafqat/git/ConsularServices

# Update the database (creates tables if they don't exist)
export PATH="$PATH:/Users/shafqat/.dotnet/tools"
dotnet ef database update --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web
```

## Benefits of EF Core

1. **Code-First Development**: Database schema generated from C# classes
2. **Type Safety**: IntelliSense and compile-time checking
3. **Automatic Change Tracking**: EF tracks entity changes automatically
4. **Migration System**: Version control for database schema changes
5. **LINQ Support**: Write queries using LINQ instead of raw SQL
6. **Async Support**: Built-in async/await patterns
7. **Connection Management**: Automatic connection handling

## Backwards Compatibility

- All public interfaces (`IServiceRepository`, `IUserRepository`) remain unchanged
- Controllers and other consuming code require no changes
- Custom attributes (`MetaData`, `ActionVerb`) continue to work as before

## Next Steps

1. **Test the Migration**: Run the application and verify all functionality works
2. **Update Database**: Run the EF migration to create/update database schema
3. **Performance Testing**: Compare performance with the Dapper implementation
4. **Remove Old Code**: Once confirmed working, remove:
   - `ServiceRepository.cs` (Dapper version)
   - `UserRepository.cs` (Dapper version)
   - `script.sql` file
   - Dapper-related NuGet packages (if no longer needed)

## Connection String

Ensure your `appsettings.json` contains a valid PostgreSQL connection string:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Database=consular_services;Username=postgres;Password=your_password"
  }
}
```

## Entity Framework Commands

Common EF Core commands for future schema changes:

```bash
# Add a new migration
dotnet ef migrations add MigrationName --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web

# Update database
dotnet ef database update --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web

# Remove last migration (if not applied to database)
dotnet ef migrations remove --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web

# Generate SQL script for migration
dotnet ef migrations script --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web
```
