# CLAUDE.md

This file provides guidance to Claude Code (claude.ai/code) when working with code in this repository.

## Project Overview

ConsularServices is a .NET 9.0 application for managing consular operations, including queue management, service delivery, and document tracking. The system consists of three main projects:

- **FrameworkQ.ConsularServices**: Core business logic, entities, and data access layer
- **FrameworkQ.ConsularServices.Web**: ASP.NET Core MVC web application with JWT authentication
- **FrameworkQ.Consular.Test**: XUnit test project

## Common Commands

### Build and Run
```bash
# Build the entire solution
dotnet build FrameworkQ.ConsularServices.sln

# Run the web application (from project root)
dotnet run --project FrameworkQ.ConsularServices.Web

# Run on specific URL
dotnet run --project FrameworkQ.ConsularServices.Web --urls "https://localhost:7235;http://localhost:5151"

# Clean build artifacts (PowerShell)
pwsh clean.ps1
```

### Testing
```bash
# Run all tests
dotnet test

# Run tests for specific project
dotnet test FrameworkQ.Consular.Test/FrameworkQ.Consular.Test.csproj
```

### Entity Framework Core (Database)

The project recently migrated from Dapper to Entity Framework Core. Always use EF Core commands with the correct project paths:

```bash
# Update database (creates tables if they don't exist)
export PATH="$PATH:/Users/shafqat/.dotnet/tools"
dotnet ef database update --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web

# Add new migration
dotnet ef migrations add MigrationName --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web

# Remove last migration (if not applied)
dotnet ef migrations remove --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web

# Generate SQL script
dotnet ef migrations script --project FrameworkQ.ConsularServices --startup-project FrameworkQ.ConsularServices.Web
```

## Architecture

### Data Layer Architecture
- **Entity Framework Core** with PostgreSQL database
- **Repository Pattern**: Both EF Core and legacy Dapper repositories exist
  - Current: `ServiceRepositoryEF.cs`, `UserRepositoryEF.cs` 
  - Legacy: `ServiceRepository.cs`, `UserRepository.cs` (Dapper-based)
- **DbContext**: `ConsularDbContext` in `/Data/ConsularDbContext.cs`
- **Entities**: Located in domain-specific folders (`Services/`, `Users/`, `Forms/`)

### Web Layer Architecture
- **ASP.NET Core MVC** with Razor views
- **JWT Authentication** with cookie-based token storage
- **Dependency Injection**: Configured in `DependencyInjection.cs`
- **Static Files**: Served from `www/` directory with no-cache headers
- **Controllers**: API (`ApiController`), Admin (`AdminController`), Forms (`FormController`), Home (`HomeController`)

### Key Components
- **Service Manager**: `IServiceManager` for business logic coordination
- **Token System**: Queue management with 4-digit numeric tokens
- **Forms**: SurveyJS integration for dynamic form creation
- **Custom Attributes**: `MetaDataAttribute`, `ActionVerbAttribute` for reflection-based operations

### Authentication & Authorization
- JWT Bearer authentication with cookie fallback
- Role-based permissions system (`Role`, `Permission`, `RolePermissionMap`)
- Custom authentication events in `Program.cs`

## Important Notes

### Entity Framework Migration Status
- The project has migrated from Dapper to Entity Framework Core
- Both repository implementations exist for backward compatibility
- Current DI configuration uses EF Core repositories (`UserRepositoryEF`, `ServiceRepositoryEF`)
- Connection string must be configured in `appsettings.json` under `ConnectionStrings:DefaultConnection`

### Database Requirements
- PostgreSQL database
- Connection string format: `"Host=localhost;Database=consular_services;Username=postgres;Password=your_password"`
- Entity Framework handles schema generation (no manual `script.sql` needed)

### Development Workflow
1. The system handles consular service queues and document delivery tracking
2. Users scan QR codes to get tokens and join service queues
3. Service providers process requests and generate delivery receipts
4. The system tracks service status and generates various reports

### URL Conventions
The application follows a consistent URL pattern:
- **List views**: `/services`, `/users`, `/stations` - shows list of items
- **Item views**: `/service`, `/user`, `/station` - shows single item in edit mode

### Generic UI System
The application uses a sophisticated generic templating system:

#### Frontend Architecture (main.js + admin.js)
- **SurveyJS Integration**: All single item edit forms use SurveyJS for dynamic rendering
- **Generic Templates**: `buildListTemplate()` and `buildItemTemplate()` functions create reusable UI patterns
- **Dynamic Type Information**: `/api/typeinfo?itemtype={Type}` endpoint provides form configuration
- **Template String Replacement**: `$action` placeholders are replaced with actual entity names

#### View Templates
- **Users**: Custom views (`Users.cshtml`, `User.cshtml`) with hardcoded logic
- **Services/Stations**: Generic views (`ListView.cshtml`, `ItemView.cshtml`) that work with any entity type
- **Generic Implementation**: Services use `buildListTemplate("services")` and `buildItemTemplate("service")`

#### JavaScript Action System
```javascript
// Example from admin.js
services: buildListTemplate("services"),    // Generates list view for /services
service: buildItemTemplate("service")       // Generates item view for /service
```

The system automatically:
1. Converts plural URLs to singular entity names (`services` → `service`)
2. Generates API endpoints (`/api/services`, `/api/service`)
3. Creates form templates using reflection via `/api/typeinfo`
4. Handles CRUD operations through generic patterns

#### Controller-View Flow Pattern
The application follows a consistent pattern for list-to-edit navigation:

**AdminController Implementation:**
```csharp
// List endpoints return specific views or generic ListView
[HttpGet("/users")] → Users.cshtml (custom)
[HttpGet("/services")] → ListView.cshtml (generic)

// Item endpoints receive POST with ID, store in ViewBag, return view
[HttpPost("/user")] → User.cshtml (custom) 
[HttpPost("/service")] → ItemView.cshtml (generic)
```

**List-to-Edit Flow:**
1. **List Page**: JavaScript calls `/api/users` to populate table
2. **Edit Button Click**: `postAction("/user", {"user_id": "123"})` submits hidden form
3. **Controller Receives POST**: `AdminController.User()` gets `user_id` from form, stores in `ViewBag.UserId`
4. **View Renders**: `User.cshtml` calls `GenerateItemPageHiddenIdString("user")` which:
   - Reads the form data from the POST request
   - Uses reflection and `ActionVerbAttribute` to find primary key fields
   - Generates hidden input fields: `<input type="hidden" id="user_id" value="123" />`
5. **JavaScript Initialization**: Page loads, reads hidden fields, calls `/api/user?user_id=123`

**Generic vs Custom Views:**
- **Custom**: `User.cshtml` calls `GenerateItemPageHiddenIdString("user")` (explicit type)
- **Generic**: `ItemView.cshtml` calls `GenerateItemPageHiddenIdString("")` (infers type from route)

### Configuration Files
- `appsettings.json`: Main configuration including JWT settings and connection strings
- `appsettings.Development.json`: Development-specific overrides
- `launchSettings.json`: Development server configuration (ports 5151 HTTP, 7235 HTTPS)