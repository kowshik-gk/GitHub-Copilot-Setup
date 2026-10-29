# Project Overview — OpenAI-DotNet-Integration

**Type:** ASP.NET Core Web API (customers CRUD) with EF Core **code-first** SQL Server persistence
**Framework:** .NET 10 (`net10.0`) · ASP.NET Core MVC (controllers) · Minimal-hosting bootstrap
**Database:** Microsoft SQL Server 2022 (Developer Edition) — database `EmployeeDb`
**Solution:** `OpenAI-DotNet-Integration.slnx` → single project `OpenAI-DotNet-Integration/OpenAI-DotNet-Integration.csproj`
**Last updated:** 2026-10-26

---

## 1. Solution layout

```
OpenAI-DotNet-Integration/
├─ OpenAI-DotNet-Integration.slnx
├─ PROJECT-OVERVIEW.md          <- this file
├─ CHAT-HISTORY.md              <- session log (reusable context)
└─ OpenAI-DotNet-Integration/
   ├─ Program.cs                <- host bootstrap, DI, middleware pipeline
   ├─ appsettings.json           <- connection string + logging + allowed hosts
   ├─ appsettings.Development.json
   ├─ Properties/launchSettings.json  (http://localhost:5242, https://localhost:7162)
   ├─ API/
   │  ├─ Program.cs                         <- host bootstrap and middleware pipeline
   │  ├─ Controllers/                       <- HTTP surface (api/customers, api/chat)
   │  └─ Middleware/                        <- exception handling and ProblemDetails
   ├─ Domain/
   │  ├─ Dtos/                              <- request/response records + validation
   │  ├─ Exceptions/                        <- domain/application exceptions
   │  ├─ Interfaces/                        <- service abstractions
   │  └─ Models/                            <- domain entities
   ├─ Infrastructure/Services/             <- concrete business and integration services
   ├─ Data/
   │  ├─ CustomerDbContext.cs               <- EF Core DbContext + fluent model config
   │  └─ Migrations/                        <- code-first migrations + model snapshot
   └─ Test/                                 <- automated test projects and fixtures
```

## 2. Request pipeline (`Program.cs`)

1. `AddControllers()`, `AddOpenApi()`, `AddProblemDetails()`
2. `AddDbContext<CustomerDbContext>(o => o.UseSqlServer(Configuration.GetConnectionString("DefaultConnection")))`
3. `AddScoped<ICustomerService, CustomerService>()`, `AddExceptionHandler<ApiExceptionHandler>()`
4. Middleware order: `UseExceptionHandler()` → `UseStatusCodePages()` → `UseHttpsRedirection()` → `UseAuthorization()` → `MapControllers()`
5. OpenAPI document is exposed only in the **Development** environment (`app.MapOpenApi()`)

## 3. API endpoints

| Method | Route | Behaviour | Success | Errors |
|---|---|---|---|---|
| GET | `/api/customers` | All customers, ordered by `Id`, `AsNoTracking` | 200 + `[]` | 500 |
| GET | `/api/customers/{id:int}` | Single customer | 200 | 404, 500 |
| POST | `/api/customers` | Create (email must be unique) | 201 + `Location` | 400 (validation), 409 (duplicate email), 500 |
| PUT | `/api/customers/{id:int}` | Full update (email must stay unique) | 200 | 400, 404, 409, 500 |
| DELETE | `/api/customers/{id:int}` | Delete | 204 | 404, 500 |

## 4. DTOs and validation (`Domain/Dtos/CustomerDtos.cs`)

- `CreateCustomerRequest` / `UpdateCustomerRequest` — `required` strings with `[Required]`, `[StringLength]`,
  `[EmailAddress]`, `[Phone]`.
- `CustomerResponse` — positional record: `Id, FirstName, LastName, Email, PhoneNumber, CreatedAt, UpdatedAt`.
- Entity never leaves the service layer; all input is trimmed before persistence.

## 5. Data layer

**Entity `Domain/Models/Customer.cs`**

| Property | Type | Notes |
|---|---|---|
| `Id` | `int` | Identity PK (`SqlServer:Identity 1,1`) |
| `FirstName` | `string` | `nvarchar(100)`, required |
| `LastName` | `string` | `nvarchar(100)`, required |
| `Email` | `string` | `nvarchar(200)`, required, **unique index** |
| `PhoneNumber` | `string` | `nvarchar(30)`, required |
| `CreatedAt` | `DateTimeOffset` | set to `UtcNow` on create |
| `UpdatedAt` | `DateTimeOffset` | refreshed on every update |

**`Data/CustomerDbContext.cs`** — fluent configuration in `OnModelCreating`:
`ToTable("Customers")`, key on `Id`, max-length/required constraints, `HasIndex(Email).IsUnique()`.
`DbSet<Customer> Customers` is the only set.

**Service behaviour (`Infrastructure/Services/CustomerService.cs`)**
- Reads use `AsNoTracking()`; writes use `SaveChangesAsync`.
- Duplicate email is pre-checked and raised as `ConflictException` (409).
- Missing entity is raised as `NotFoundException` (404).
- `ArgumentException` maps to 400; anything else → 500 with a generic message.

## 6. Error handling

`API/Middleware/ApiExceptionHandler.cs` implements `IExceptionHandler` and writes RFC 7807 `ProblemDetails`:

| Exception | Status | Title |
|---|---|---|
| `NotFoundException` | 404 | Not Found |
| `ConflictException` | 409 | Conflict |
| `ArgumentException` | 400 | Bad Request |
| any other | 500 | Internal Server Error (`"An unexpected error occurred."`) |

Every handled exception is logged via `ILogger<ApiExceptionHandler>`.

## 7. Database state (`EmployeeDb`)

**Applied by this project**

| Table | Columns / constraints |
|---|---|
| `Customers` | `Id` int identity PK, `FirstName` nvarchar(100) NOT NULL, `LastName` nvarchar(100) NOT NULL, `Email` nvarchar(200) NOT NULL, `PhoneNumber` nvarchar(30) NOT NULL, `CreatedAt` datetimeoffset NOT NULL, `UpdatedAt` datetimeoffset NOT NULL |
| `PK_Customers` | clustered primary key on `Id` |
| `IX_Customers_Email` | **unique** index on `Email` |

**`__EFMigrationsHistory`**

| MigrationId | ProductVersion | Origin |
|---|---|---|
| `20260505160340_InitialCreate` | 8.0.8 | previous project — left untouched |
| `20260505161624_InitialCreate2` | 8.0.8 | previous project — left untouched |
| `20261026130942_InitialCreate` | 10.0.13 | **this project** — applied 2026-10-26 |

> `EmployeeDb` is shared with an earlier project that owns `Employees` and `Departments`. Those tables are not
> touched by this project's migrations.

## 8. Migrations workflow (code-first)

The project already uses code-first; there is **no** Database First / EDMX model.

```powershell
# one-time tooling
dotnet tool install --global dotnet-ef --version 10.0.13

# after every model change
dotnet ef migrations add <MigrationName> --project OpenAI-DotNet-Integration
dotnet ef database update              --project OpenAI-DotNet-Integration

# diagnostics
dotnet ef migrations has-pending-model-changes --project OpenAI-DotNet-Integration
dotnet ef migrations list                --project OpenAI-DotNet-Integration
```

Migrations live in `OpenAI-DotNet-Integration/Data/Migrations/`; `CustomerDbContextModelSnapshot.cs` is the baseline that
`migrations add` diffs against.

## 9. Configuration

`appsettings.json`

```json
"ConnectionStrings": {
  "DefaultConnection": "Server=localhost;Database=EmployeeDb;Integrated Security=True;TrustServerCertificate=True;"
}
```

- Integrated Security (Windows auth) — no SQL credentials in the repo.
- Logging: default `Information`, `Microsoft.AspNetCore` → `Warning`.
- `AllowedHosts: *`.

## 10. Environment notes (this machine)

- **SQL Server instance:** `MSSQLSERVER` under `MSSQL16.MSSQLSERVER`, service running as `NT Service\MSSQLSERVER`.
- **TCP/IP is disabled** on the instance (`SuperSocketNetLib\Tcp` entries all `Enabled=0`); only the local
  connection provider (`\\.\pipe\sql\query`, shared memory) and the DAC on port 1434 are listening.
  `Server=localhost` therefore still works — `Microsoft.Data.SqlClient` falls back to the local pipe provider.
  `sqlcmd` (ODBC) needs the explicit `lpc:` prefix on this box.
- **Windows logins:** `KOWSHIK\Nallas` (VS process owner, sysadmin) and `KOWSHIK\Kowshik Ganesan`
  (interactive console account) both have SQL Server logins; the latter is `db_owner` on `EmployeeDb`.
  Without it, connections failed with *"Could not find a login matching the name provided"*.
- `dotnet-ef` 10.0.13 is installed as a global tool (`%USERPROFILE%\.dotnet\tools`).

## 11. Running and verifying

```powershell
dotnet run --project OpenAI-DotNet-Integration --urls http://127.0.0.1:5099
curl http://127.0.0.1:5099/api/customers
```

Last verification (2026-10-26): `GET` → 200 `[]`, `POST` → 201 (id=1), `GET` by id → 200, `DELETE` → 204,
final `GET` → `[]`; `dotnet ef database update` re-run reported the database already up to date.
