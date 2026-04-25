# ApiBook Layered Structure (.NET 10)

## Projects

- `ApiBook.Api`: Host/Web API entrypoint (only composition root)
- `ApiBook.Presentation`: Controllers only
- `ApiBook.Application`: Use-cases/contracts/DTOs
- `ApiBook.Domain`: Entities
- `ApiBook.Infrastructure`: EF Core + PostgreSQL + repositories + migrations
- `ApiBook.Logging`: Centralized Serilog setup
- `ApiBook.Security`: Encrypted connection string resolver

## Dependency Direction

- `ApiBook.Api` -> `Presentation`, `Application`, `Infrastructure`, `Logging`
- `ApiBook.Presentation` -> `Application`
- `ApiBook.Infrastructure` -> `Application`, `Domain`, `Security`
- `ApiBook.Application` -> `Domain`

No lower layer references upward.

## Connection String Security

`ApiBook.Security` expects:

- `ConnectionStrings:Postgres` as encrypted value with prefix `enc:`
- Optional override `ConnectionStrings:PostgresPlain` for local development only
- AES encryption key from environment variable: `APIBOOK_CONN_ENCRYPTION_KEY`

Never commit plain connection strings to source control.

## Migration Commands

From solution root:

```powershell
dotnet ef migrations add InitialCreate --project .\ApiBook.Infrastructure\ --startup-project .\ApiBook.Api\ --output-dir Persistence\Migrations
dotnet ef database update --project .\ApiBook.Infrastructure\ --startup-project .\ApiBook.Api\
```

## Run

```powershell
$env:APIBOOK_CONN_ENCRYPTION_KEY="your-strong-key"
$env:ConnectionStrings__PostgresPlain="Host=localhost;Port=5432;Database=apibook;Username=postgres;Password=postgres"
$env:Security__ApiKey="your-api-key"
$env:Security__AuthUser__Username="admin"
$env:Security__AuthUser__Password="change-me"
$env:Security__Jwt__SigningKey="very-long-random-signing-key-32-plus-chars"
dotnet run --project .\ApiBook.Api\
```

## Security Flow

- Get JWT token from `POST /api/auth/token` using configured username/password.
- Send JWT in `Authorization: Bearer <token>`.
- For mutating endpoints (`POST/PUT/PATCH/DELETE`) also send `X-API-Key: <key>`.

## Commands Endpoints

- `GET /api/commands`
- `GET /api/commands/{id}`
- `GET /api/commands/platform/{platformId}`
- `POST /api/commands` (JWT + API key)
- `PUT /api/commands/{id}` (JWT + API key)
- `DELETE /api/commands/{id}` (JWT + API key)
