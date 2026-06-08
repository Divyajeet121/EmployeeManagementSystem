# AI Usage Report

## AI Tools Used

- Claude (Anthropic) via Claude Code — used as a pair-programming assistant for scaffolding, writing, and debugging code, and for drafting documentation.

## Prompts Used (representative)

1. "Scaffold a clean-architecture ASP.NET Core 8 Web API solution with Domain, Application, Infrastructure, and Api projects for an employee management system."
2. "Add EF Core SQL Server packages, JWT bearer authentication, and Swagger to the API project, and wire up project references between the layers."
3. "Create the Employee and User entities, DTOs, repository interfaces, and service interfaces/implementations for authentication and employee CRUD with role-based rules (Admin can write, User can read)."
4. "Implement JWT token generation, password hashing with ASP.NET Core Identity's PasswordHasher, and a global exception-handling middleware that maps domain exceptions to HTTP status codes."
5. "Generate the EF Core migration for the schema and a startup seeder that creates default Admin/User accounts and sample employees."
6. "Build a vanilla JS + Bootstrap 5 frontend (no React/Angular) with a login page, a dashboard with summary cards, and an employee management page with search/filter, pagination, and add/edit/delete modals, with role-aware UI."
7. "Write the SQL schema and seed scripts that mirror the EF Core model."
8. "Run the application end-to-end, verify login, dashboard, and employee CRUD through the API and the browser UI, and fix any issues found."

## Areas Where AI Assisted

- Initial project/solution scaffolding (`dotnet new`, project references, NuGet packages)
- Boilerplate for entities, DTOs, repository/service interfaces and implementations
- JWT authentication & authorization configuration, password hashing, and exception-handling middleware
- EF Core configuration classes, migration generation, and the database seeder
- Frontend HTML/CSS/JS pages (login, dashboard, employee CRUD with modals, pagination, search/filter)
- SQL schema/seed scripts and project documentation (README, architecture overview, this report)

## Validation Approach

- `dotnet build` run after each layer was added to catch compilation errors early; the solution builds with 0 warnings/errors.
- The API was run locally against SQL Server LocalDB; `curl` was used to exercise every endpoint (`/api/auth/login`, `/api/auth/logout`, `/api/dashboard/summary`, `/api/employees` GET/POST/PUT/DELETE, `/api/employees/{id}`), confirming correct status codes (200/201/204/401/403/404/400) and JSON payloads for both `Admin` and `User` roles.
- A Playwright browser script drove the actual UI end-to-end: login, dashboard load, and a full add → edit → delete cycle on the employee list, with screenshots captured at each step to visually confirm the UI, modals, validation, and audit fields render correctly.
- The seeded test database was dropped after manual/automated verification so the grader gets a clean first-run seed.
- Code was reviewed and adjusted manually (e.g., simplifying the employee-to-DTO mapping, removing unused dependencies, tightening query parameters) rather than accepting the first generated version as-is.

## Percentage of AI-Generated Code

Approximately 70-75% of the initial code (scaffolding, boilerplate entities/DTOs/controllers, configuration, and frontend markup/scripts) was AI-generated and then reviewed, corrected, and adapted by the developer; the remaining 25-30% — architectural decisions, fixes found during end-to-end testing, query/validation refinements, and documentation tailoring — was manually driven.
