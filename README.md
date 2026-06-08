# Secure Employee Management System

A full-stack employee management application built with ASP.NET Core 8 Web API, SQL Server (EF Core), JWT authentication, and a vanilla JavaScript / Bootstrap 5 frontend served from `wwwroot`.

## Tech Stack

- **Backend**: ASP.NET Core 8 Web API, Entity Framework Core 8 (SQL Server)
- **Frontend**: HTML5, CSS3, Bootstrap 5, vanilla JavaScript (Fetch API)
- **Auth**: JWT bearer tokens, role-based authorization (Admin / User)
- **Docs**: Swagger / OpenAPI (Swashbuckle)
- **Architecture**: Clean architecture with Domain / Application / Infrastructure / Api layers and the repository pattern

## Solution Structure

```
EmployeeManagementSystem.sln
src/
  EmployeeManagement.Domain          Entities, enums (no external dependencies)
  EmployeeManagement.Application     DTOs, service interfaces/implementations, repository interfaces, exceptions
  EmployeeManagement.Infrastructure  EF Core DbContext, migrations, repositories, JWT/password hashing, DI registration
  EmployeeManagement.Api             Controllers, Program.cs, middleware, Swagger config, wwwroot frontend
database/
  01_CreateSchema.sql                Raw SQL schema script (alternative to EF migrations)
  02_SeedData.sql                    Raw SQL seed data script
```

## Prerequisites

- .NET 8 SDK
- SQL Server (LocalDB, Express, or full instance)

## Setup & Run

1. **Clone the repository** and open `EmployeeManagementSystem.sln`, or work from the `EmployeeManagementSystem` folder in a terminal.

2. **Configure the connection string** in `src/EmployeeManagement.Api/appsettings.json` (defaults to LocalDB):

   ```json
   "ConnectionStrings": {
     "DefaultConnection": "Server=(localdb)\\mssqllocaldb;Database=EmployeeManagementDb;Trusted_Connection=True;MultipleActiveResultSets=true;TrustServerCertificate=True"
   }
   ```

   Update `Jwt:SigningKey` with your own long random secret before deploying anywhere beyond local development.

3. **Run the API** — the database is created and seeded automatically on first run via EF Core migrations and `DbInitializer`:

   ```bash
   cd src/EmployeeManagement.Api
   dotnet run
   ```

   Alternatively, run the raw SQL scripts in `database/` against your SQL Server instance if you prefer manual schema creation (`01_CreateSchema.sql` then `02_SeedData.sql`); in that case remove/skip the automatic migration step by not calling `DbInitializer.InitializeAsync` (or simply let it run against the already-seeded database — it only seeds when the `Users` table is empty).

4. **Open the application**:
   - Frontend: `https://localhost:<port>/index.html` (the port is printed in the console; static files are served from `wwwroot`)
   - Swagger UI: `https://localhost:<port>/swagger`

## Demo Accounts

| Role  | Username   | Password   |
|-------|------------|------------|
| Admin | `admin`     | `Admin@123` |
| User  | `john.user` | `User@123`  |

Admin can create, edit, and delete employees. Standard users have read-only access to the employee list, search/filter, and the dashboard.

## Key Features

- JWT-based login/logout with role-based authorization (`Admin`, `User`)
- Employee CRUD with server-side validation, uniqueness checks (code/email), and audit fields (Created By/On, Modified By/On)
- Dashboard summary cards (Total / Active / Inactive employees)
- Search, department filter, status filter, and pagination on the employee list
- Centralized exception-handling middleware mapping domain exceptions to proper HTTP status codes
- Swagger/OpenAPI documentation with JWT bearer auth support built in

## Security Notes

- Passwords are hashed with ASP.NET Core Identity's `PasswordHasher` (PBKDF2/HMAC-SHA256, salted)
- All employee endpoints require a valid JWT; write operations (create/update/delete) require the `Admin` role
- EF Core parameterized queries prevent SQL injection; `EF.Functions.Like` is used for safe pattern search
- A global exception-handling middleware avoids leaking internal details in error responses
- HTTPS redirection and CORS are configured; update the allowed origins in `Program.cs` for your deployment

## Running Tests / Build

```bash
dotnet build
```

(No automated test project is included in this mini-project; manual end-to-end verification was performed against the running API and UI — see `AI_USAGE_REPORT.md`.)
