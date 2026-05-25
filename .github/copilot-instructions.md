# Copilot instructions for RescueSystem

## Build, test, and lint
- Build (solution): `dotnet build .\src\src.sln`
- Test (all): `dotnet test .\src\src.sln`
- Test (single): `dotnet test .\src\src.sln --filter "FullyQualifiedName~Namespace.ClassName.TestName"`
- Format/lint: `dotnet format .\src\src.sln`

## High-level architecture
- **Clients → WebAPI → Application → Infrastructure → SQL Server**: mobile/web/admin clients call `RescueSystem.Api`, which dispatches MediatR Commands/Queries to the Application layer; handlers call repositories/services in Infrastructure backed by EF Core (`ApplicationDbContext`) and SQL Server.
- **Clean Architecture layout** with four projects under `src/`:  
  - `RescueSystem.Domain` holds core entities/enums; **single Identity-based user model** (`ApplicationUser`) with roles (Citizen/Rescuer/Dispatcher/Commander).  
  - `RescueSystem.Application` implements CQRS (Commands/Queries + Handlers), DTOs, interfaces, and validation pipeline.  
  - `RescueSystem.Infrastructure` provides EF Core/Identity persistence (`ApplicationDbContext`), repositories, and services.  
  - `RescueSystem.Api` is the ASP.NET Core Web API (controllers, JWT auth, Swagger, middleware).
- **Startup behavior**: `Program.cs` registers Application/Infrastructure DI and runs `ApplicationSeeder`, which applies migrations and seeds roles/users/requests/missions on app start.

## Key conventions
- **CQRS + MediatR**: Controllers should send Commands/Queries from `RescueSystem.Application.Features.<Area>`; implement logic in matching Handlers.
- **Validation**: FluentValidation is wired via `ValidationBehavior`; validators throw `ValidationException`, which the API middleware maps to a structured error response.
- **Error and response shape**: Use `ApiResponse<T>.SuccessResponse`/`ErrorResponse` for consistent JSON. `GlobalExceptionMiddleware` maps Application exceptions by full type name:
  - `RescueSystem.Application.Common.Exception.BadRequestException`
  - `RescueSystem.Application.Common.Exception.NotFoundException`
  - `RescueSystem.Application.Common.Exception.UnauthorizedException`
  - `RescueSystem.Application.Common.Exception.InternalServerErrorException`
- **Migrations**: EF Core migrations live in `RescueSystem.Infrastructure`. Use the Infrastructure project as the target and the API as the startup project (see `TEMP_MIGRATION_INSTRUCTIONS.txt`).
