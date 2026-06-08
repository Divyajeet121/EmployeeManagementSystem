# Architecture Overview

## Layered / Clean Architecture

The solution is split into four projects with a strict dependency direction (outer layers depend on inner layers, never the reverse):

```
EmployeeManagement.Api  ---->  EmployeeManagement.Infrastructure  ---->  EmployeeManagement.Application  ---->  EmployeeManagement.Domain
        |                                                                          ^
        +--------------------------------------------------------------------------+
```

- **Domain**: Plain entities (`Employee`, `User`) and enums (`UserRole`). No framework dependencies.
- **Application**: DTOs, custom exceptions, repository/service interfaces (`IEmployeeRepository`, `IAuthService`, `IPasswordHasher`, `IJwtTokenGenerator`, ...) and their service implementations (`AuthService`, `EmployeeService`). Depends only on `Domain`.
- **Infrastructure**: EF Core `AppDbContext`, entity configurations, EF migrations, repository implementations (`EmployeeRepository`, `UserRepository`), JWT token generation, password hashing, and the `DependencyInjection.AddInfrastructure` extension that wires everything into the DI container. Depends on `Application` and `Domain`.
- **Api**: ASP.NET Core controllers, `Program.cs` composition root, JWT/Swagger/CORS configuration, global exception-handling middleware, and the static `wwwroot` frontend (HTML/CSS/JS). Depends on `Application` and `Infrastructure`.

## Repository Pattern

`IEmployeeRepository` and `IUserRepository` abstract data access behind interfaces defined in the Application layer and implemented in Infrastructure using EF Core. Services depend on these interfaces (constructor injection), which keeps business logic testable and decoupled from EF Core specifics.

## Request Flow (example: Create Employee)

1. `EmployeesController.CreateEmployee` receives the authenticated request (JWT validated by the auth middleware, `Admin` role enforced by `[Authorize(Roles = "Admin")]`).
2. The controller calls `IEmployeeService.CreateAsync`, passing the DTO and the current user's id (extracted from JWT claims).
3. `EmployeeService` validates business rules (duplicate employee code/email via `IEmployeeRepository.ExistsByCodeOrEmailAsync`), maps the DTO to an `Employee` entity, stamps audit fields (`CreatedBy`, `CreatedOn`), and persists it through the repository.
4. `EmployeeRepository` issues parameterized EF Core commands against SQL Server.
5. The result is mapped back to an `EmployeeDto` and returned as `201 Created`.
6. Any domain exception (`NotFoundException`, `ValidationException`, `AuthenticationFailedException`) thrown along the way is caught by `ExceptionHandlingMiddleware` and translated into the correct HTTP status code and a safe JSON error payload.

## Authentication & Authorization

- `AuthService.LoginAsync` validates credentials using `IPasswordHasher` (ASP.NET Core Identity's PBKDF2-based hasher) and issues a signed JWT via `IJwtTokenGenerator`.
- The JWT carries the user id, username, and role as claims; `Program.cs` configures `JwtBearer` authentication to validate issuer, audience, lifetime, and signing key.
- Controllers use `[Authorize]` for any authenticated user and `[Authorize(Roles = "Admin")]` to restrict create/update/delete operations to administrators.
- `LoginResponseDto` returns the username, email, and role alongside the token so the frontend can render a personalized profile experience without a separate lookup call.
- The login form includes a show/hide toggle on the password field, switching the input type between `password` and `text` and swapping the eye icon accordingly.

## Data Layer

- EF Core Code-First with fluent configuration classes (`EmployeeConfiguration`, `UserConfiguration`) define table mappings, required fields, max lengths, and unique indexes (employee code, email, username).
- Migrations in `Persistence/Migrations` create the schema; `DbInitializer` runs `Database.MigrateAsync()` and seeds default users/employees on first run.
- Equivalent raw SQL scripts are provided in `database/` for environments that prefer manual schema management.

## Frontend

The frontend is a static site built with vanilla JavaScript and Bootstrap 5, served directly from `wwwroot`:

- `index.html` + `js/login.js` — the login form validates input, authenticates against `/api/auth/login`, toggles password visibility through an eye-icon button, and stores the returned token, username, email, and role in `sessionStorage`. `Program.cs` serves this page at the root path `/` via `UseDefaultFiles`, so the address bar never shows `index.html`.
- `js/api.js` — a thin Fetch wrapper that attaches the bearer token to every request, reads/writes the session values (`token`, `username`, `email`, `role`), normalizes API error responses into a single `Error`, and redirects back to the login page whenever the server responds with `401`.
- `js/layout.js` — builds the shared navbar for every authenticated page, including a profile dropdown with an avatar (the user's initial on a colored circle), the username, role badge, email address, and a logout action that calls `/api/auth/logout`, clears the session, and redirects to `/`.
- `pages/dashboard.html` + `js/dashboard.js` — a CRM-style landing page with a personalized greeting and current date, colored summary cards (total/active/inactive employees with icons) sourced from `/api/dashboard/summary`, a department distribution panel rendered as horizontal bar rows computed client-side from the employee list, and a "Recently Added" panel listing the latest employees with their status badges.
- `pages/employees.html` + `js/employees.js` — a paginated, searchable, filterable employee table with Bootstrap modals for adding/editing employees and confirming deletions; only Admins see the Add/Edit/Delete controls, enforced by checking the role stored in the session.
- `Program.cs` rewrites the clean paths `/dashboard` and `/employees` to `pages/dashboard.html` and `pages/employees.html` respectively via `UseRewriter`, so users navigate with short URLs while the underlying static files stay organized under `pages/`.

## Cross-Cutting Concerns

- **Exception handling**: a single `ExceptionHandlingMiddleware` maps known application exceptions to HTTP status codes and logs unexpected errors without leaking internal details to clients.
- **Validation**: client-side HTML5/Bootstrap validation plus server-side checks in `EmployeeService` (required fields enforced via DTOs/model binding, duplicate code/email checks).
- **CORS**: configured via a named policy so the static frontend (or another origin during development) can call the API.
- **Swagger**: documents all endpoints and includes a JWT bearer security definition so endpoints can be tested directly from the Swagger UI.
