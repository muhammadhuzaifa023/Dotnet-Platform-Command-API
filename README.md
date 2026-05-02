# 📚 ApiBook — REST API Reference Platform

> A clean-architecture ASP.NET Core 8 Web API for managing developer platforms and their CLI commands. Built with PostgreSQL, JWT authentication, AES-encrypted connection strings, and Serilog structured logging.

[![.NET](https://img.shields.io/badge/.NET-8.0-512BD4?style=flat-square&logo=dotnet)](https://dotnet.microsoft.com/)
[![PostgreSQL](https://img.shields.io/badge/PostgreSQL-16-336791?style=flat-square&logo=postgresql)](https://www.postgresql.org/)
[![EF Core](https://img.shields.io/badge/EF%20Core-8-512BD4?style=flat-square)](https://learn.microsoft.com/en-us/ef/core/)
[![Serilog](https://img.shields.io/badge/Serilog-Structured%20Logging-informational?style=flat-square)](https://serilog.net/)
[![License: MIT](https://img.shields.io/badge/License-MIT-yellow.svg?style=flat-square)](LICENSE)

---

## 📋 Table of Contents

- [Overview](#-overview)
- [Architecture Diagram](#-architecture-diagram)
- [Project Structure](#-project-structure)
- [Project Breakdown](#-project-breakdown)
- [API Endpoints](#-api-endpoints)
- [Authentication & Security Flow](#-authentication--security-flow)
- [Database Schema & Relationships](#-database-schema--relationships)
- [Configuration](#-configuration)
- [Getting Started](#-getting-started)
- [Migration Commands](#-migration-commands)
- [Security: Encrypting Connection Strings](#-security-encrypting-connection-strings)
- [Logging](#-logging)
- [Tech Stack](#-tech-stack)

---

## 🔍 Overview

**ApiBook** is a reference API that stores developer **Platforms** (e.g., Linux, Windows, Docker) and their associated **Commands** (e.g., `ls -la`, `docker ps`). It demonstrates enterprise-grade patterns:

- ✅ Clean Architecture (Domain → Application → Infrastructure → Presentation → API)
- ✅ Repository Pattern with async/await throughout
- ✅ JWT Bearer authentication + API Key middleware
- ✅ AES-256-CBC encrypted PostgreSQL connection strings
- ✅ Serilog structured logging (Console + daily rolling file)
- ✅ Cancellation token support on every async operation
- ✅ Sealed record DTOs (immutable, value-equality)

---

## 🏛 Architecture Diagram

```
┌─────────────────────────────────────────────────────────────────────┐
│                        CLIENT (HTTP/HTTPS)                          │
│                  GET /api/platforms, POST /api/auth/token ...       │
└───────────────────────────┬─────────────────────────────────────────┘
                            │
                            ▼
┌─────────────────────────────────────────────────────────────────────┐
│                      ApiBook.Api  (Entry Point)                     │
│                                                                     │
│  ┌──────────────────────────────────────────────────────────────┐   │
│  │                     Middleware Pipeline                       │   │
│  │                                                              │   │
│  │  Request                                                     │   │
│  │     │                                                        │   │
│  │     ▼                                                        │   │
│  │  SerilogRequestLogging ──────── logs every request          │   │
│  │     │                                                        │   │
│  │     ▼                                                        │   │
│  │  HTTPS Redirection                                           │   │
│  │     │                                                        │   │
│  │     ▼                                                        │   │
│  │  UseAuthentication ─────────── validates JWT Bearer token   │   │
│  │     │                                                        │   │
│  │     ▼                                                        │   │
│  │  ApiKeyMiddleware ──────────── checks X-API-Key header      │   │
│  │     │                         (only POST/PUT/PATCH/DELETE)  │   │
│  │     ▼                                                        │   │
│  │  UseAuthorization ─────────── [Authorize] attribute check   │   │
│  │     │                                                        │   │
│  │     ▼                                                        │   │
│  │  MapControllers                                              │   │
│  └──────────────────────────────────────────────────────────────┘   │
└────────────────────────────┬────────────────────────────────────────┘
                             │
                             ▼
┌────────────────────────────────────────────────────────────────────┐
│                   ApiBook.Presentation (Controllers)               │
│                                                                    │
│   ┌─────────────────┐  ┌──────────────────┐  ┌────────────────┐   │
│   │  AuthController │  │CommandsController│  │PlatformsCont. │   │
│   │  POST /token    │  │  GET/POST/PUT    │  │ GET/POST/PUT  │   │
│   └────────┬────────┘  │  PATCH/DELETE   │  │ PATCH/DELETE  │   │
│            │           └────────┬─────────┘  └───────┬────────┘   │
└────────────┼────────────────────┼────────────────────┼────────────┘
             │                    │                     │
             ▼                    ▼                     ▼
┌────────────────────────────────────────────────────────────────────┐
│                    ApiBook.Application (Business Logic)            │
│                                                                    │
│   Contracts (Interfaces)          Services           DTOs          │
│   ┌────────────────────┐   ┌──────────────────┐  ┌────────────┐  │
│   │ ICommandRepository │   │  CommandService  │  │CommandDto  │  │
│   │ IPlatformRepository│   │  PlatformService │  │PlatformDto │  │
│   │ ICommandService    │   └──────────────────┘  │AuthDtos    │  │
│   │ IPlatformService   │                          └────────────┘  │
│   │ IJwtTokenService   │                                          │
│   │ IApiKeyValidator   │                                          │
│   └────────────────────┘                                          │
└──────────────────────────┬─────────────────────────────────────────┘
                           │
                           ▼
┌────────────────────────────────────────────────────────────────────┐
│                  ApiBook.Infrastructure (Data & Security)          │
│                                                                    │
│   Persistence               Repositories        Security           │
│   ┌──────────────────┐   ┌───────────────────┐  ┌─────────────┐  │
│   │   AppDbContext   │   │ CommandRepository │  │JwtTokenSvc  │  │
│   │  (EF Core +      │◄──│ PlatformRepository│  │ApiKeyValid. │  │
│   │   PostgreSQL)    │   └───────────────────┘  └─────────────┘  │
│   │                  │                                            │
│   │  AppDbCtxFactory │                                            │
│   └──────────────────┘                                            │
└──────────────────────────┬─────────────────────────────────────────┘
                           │
                           ▼
┌────────────────────────────────────────────────────────────────────┐
│                      ApiBook.Domain (Entities)                     │
│                                                                    │
│          ┌───────────────┐        ┌───────────────┐               │
│          │   Platform    │───────▶│    Command    │               │
│          │  Id, Name,    │ 1    N │ Id, HowTo,    │               │
│          │  Publisher    │        │ CommandLine,  │               │
│          │  Commands[]   │        │ PlatformId    │               │
│          └───────────────┘        └───────────────┘               │
└────────────────────────────────────────────────────────────────────┘

┌────────────────────────────────────────────────────────────────────┐
│                    Cross-Cutting Concerns                          │
│                                                                    │
│   ApiBook.Logging              ApiBook.Security                    │
│   ┌────────────────────┐      ┌──────────────────────────────┐    │
│   │ LoggingExtensions  │      │ EncryptedConnectionString    │    │
│   │ - AddStructured    │      │   Resolver                   │    │
│   │   Logging()        │      │ - AES-256-CBC Encryption     │    │
│   │ - UseStructured    │      │ - Env var key management     │    │
│   │   RequestLogging() │      └──────────────────────────────┘    │
│   └────────────────────┘                                          │
└────────────────────────────────────────────────────────────────────┘
```

---

## 📁 Project Structure

```
ApiBook/
│
├── ApiBook.sln
│
├── ApiBook.Api/                          # 🚀 Entry point & host
│   ├── Middleware/
│   │   └── ApiKeyMiddleware.cs           # X-API-Key header validation
│   ├── Properties/
│   │   └── launchSettings.json          # HTTP: 5227 | HTTPS: 7097
│   ├── appsettings.json                  # App configuration
│   ├── appsettings.Development.json      # Dev overrides
│   └── Program.cs                        # DI setup & middleware pipeline
│
├── ApiBook.Application/                  # 💼 Business logic layer
│   ├── Contracts/
│   │   ├── IApiKeyValidator.cs
│   │   ├── ICommandRepository.cs
│   │   ├── ICommandService.cs
│   │   ├── IJwtTokenService.cs
│   │   ├── IPlatformRepository.cs
│   │   └── IPlatformService.cs
│   ├── DTOs/
│   │   ├── AuthLoginRequestDto.cs        # record(Username, Password)
│   │   ├── AuthTokenResponseDto.cs       # record(AccessToken, ExpiresAtUtc)
│   │   ├── CommandCreateDto.cs           # record(HowTo, CommandLine, PlatformId)
│   │   ├── CommandReadDto.cs             # record(Id, HowTo, CommandLine, PlatformId)
│   │   ├── CommandUpdateDto.cs           # record(HowTo, CommandLine, PlatformId)
│   │   ├── PlatformCreateDto.cs          # record(Name, Publisher)
│   │   ├── PlatformReadDto.cs            # record(Id, Name, Publisher)
│   │   └── PlatformUpdateDto.cs          # record(Name, Publisher)
│   └── Services/
│       ├── CommandService.cs             # CRUD + DTO mapping
│       └── PlatformService.cs            # CRUD + DTO mapping
│
├── ApiBook.Domain/                       # 🏗 Core domain entities
│   └── Entities/
│       ├── Command.cs                    # Id, HowTo, CommandLine, PlatformId, Platform?
│       └── Platform.cs                   # Id, Name, Publisher, Commands[]
│
├── ApiBook.Infrastructure/               # 🗄 Data access & security impl
│   ├── Persistence/
│   │   ├── AppDbContext.cs               # EF Core DbContext (Platforms + Commands)
│   │   └── AppDbContextFactory.cs        # Design-time factory (migrations)
│   ├── Repositories/
│   │   ├── CommandRepository.cs          # ICommandRepository implementation
│   │   └── PlatformRepository.cs         # IPlatformRepository implementation
│   ├── Security/
│   │   ├── ConfigurationApiKeyValidator.cs
│   │   └── JwtTokenService.cs            # HMAC-SHA256 JWT generation
│   └── DependencyInjection.cs            # AddInfrastructureServices() extension
│
├── ApiBook.Logging/                      # 📝 Serilog cross-cutting concern
│   └── LoggingExtensions.cs             # AddStructuredLogging(), UseStructuredRequestLogging()
│
├── ApiBook.Presentation/                 # 🎮 Controllers (MVC Presentation layer)
│   └── Controllers/
│       ├── AuthController.cs             # POST /api/auth/token
│       ├── CommandsController.cs         # Full CRUD /api/commands
│       └── PlatformsController.cs        # Full CRUD /api/platforms
│
└── ApiBook.Security/                     # 🔐 AES encryption utility
    └── EncryptedConnectionStringResolver.cs
```

---

## 📦 Project Breakdown

### 1️⃣ ApiBook.Api
**Role:** Application entry point, host configuration, middleware pipeline.

| File | Purpose |
|------|---------|
| `Program.cs` | Registers all services, configures JWT, builds middleware pipeline |
| `ApiKeyMiddleware.cs` | Intercepts mutating requests (POST/PUT/PATCH/DELETE), validates `X-API-Key` header |
| `appsettings.json` | JWT settings, DB connection string, Serilog config |

**Middleware Pipeline Order (critical):**
```
Request → SerilogLogging → HTTPS Redirect → Authentication → ApiKeyMiddleware → Authorization → Controllers
```

---

### 2️⃣ ApiBook.Application
**Role:** Business logic, interfaces (contracts), DTOs. Zero infrastructure dependencies.

| Component | Files | Purpose |
|-----------|-------|---------|
| Contracts | `I*Repository`, `I*Service`, `IJwtTokenService`, `IApiKeyValidator` | Interface definitions — dependency inversion |
| DTOs | `*CreateDto`, `*ReadDto`, `*UpdateDto`, `Auth*Dto` | Immutable `sealed record` data transfer objects |
| Services | `CommandService`, `PlatformService` | Orchestrate repo calls, map Entity ↔ DTO |

---

### 3️⃣ ApiBook.Domain
**Role:** Pure domain entities. No dependencies on anything else.

```
Platform  ──────────────────────────  Command
─────────                            ─────────
Id (PK)                              Id (PK)
Name (max 100)                       HowTo (max 200)
Publisher (max 100)                  CommandLine (max 200)
Commands → ICollection<Command>      PlatformId (FK)
                                     Platform → Platform?
```

---

### 4️⃣ ApiBook.Infrastructure
**Role:** EF Core, PostgreSQL, JWT, API key — all infrastructure implementations.

| Folder | Files | Purpose |
|--------|-------|---------|
| `Persistence/` | `AppDbContext.cs` | EF Core with Fluent API config, cascade delete |
| `Persistence/` | `AppDbContextFactory.cs` | Design-time context for `dotnet ef migrations` |
| `Repositories/` | `CommandRepository.cs` | Async CRUD, `AsNoTracking()` for reads |
| `Repositories/` | `PlatformRepository.cs` | Async CRUD, `AsNoTracking()` for reads |
| `Security/` | `JwtTokenService.cs` | HMAC-SHA256 JWT token generation |
| `Security/` | `ConfigurationApiKeyValidator.cs` | API key validation from `appsettings.json` |

---

### 5️⃣ ApiBook.Presentation
**Role:** ASP.NET Core controllers. Thin layer — delegates everything to services.

| Controller | Route | Auth Required |
|-----------|-------|--------------|
| `AuthController` | `POST /api/auth/token` | ❌ AllowAnonymous |
| `CommandsController` | `GET /api/commands/**` | ❌ Public |
| `CommandsController` | `POST/PUT/DELETE /api/commands/**` | ✅ JWT + API Key |
| `PlatformsController` | `GET /api/platforms/**` | ❌ Public |
| `PlatformsController` | `POST/PUT/DELETE /api/platforms/**` | ✅ JWT + API Key |

---

### 6️⃣ ApiBook.Logging
**Role:** Serilog setup as extension methods.

```csharp
builder.AddStructuredLogging();         // Serilog: Console + daily file
app.UseStructuredRequestLogging();      // HTTP request logging middleware
```

Log files → `logs/apibook-YYYYMMDD.log` (daily rolling)

---

### 7️⃣ ApiBook.Security
**Role:** AES-256-CBC encryption/decryption for connection strings.

```
Flow:
  env var APIBOOK_CONN_ENCRYPTION_KEY
       │
       ▼ SHA256 hash → 32-byte AES key
       │
  appsettings.json: "enc:<base64(IV + ciphertext)>"
       │
       ▼ Decrypt at runtime
       │
  Plain connection string → EF Core
```

Supports 3 resolution modes:
1. `ConnectionStrings:{name}Plain` override (dev-only plaintext)
2. `enc:` prefixed encrypted value → decrypt with AES
3. Plain string → use as-is

---

## 🌐 API Endpoints

### 🔑 Auth

| Method | Endpoint | Auth | Body | Response |
|--------|----------|------|------|----------|
| `POST` | `/api/auth/token` | None | `AuthLoginRequestDto` | `AuthTokenResponseDto` |

**Request:**
```json
{
  "username": "admin",
  "password": "ChangeMeNow!"
}
```

**Response:**
```json
{
  "accessToken": "eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...",
  "expiresAtUtc": "2025-01-01T13:00:00Z"
}
```

---

### 🖥 Platforms

| Method | Endpoint | Auth | Body | Response |
|--------|----------|------|------|----------|
| `GET` | `/api/platforms` | ❌ None | — | `PlatformReadDto[]` |
| `GET` | `/api/platforms/{id}` | ❌ None | — | `PlatformReadDto` |
| `POST` | `/api/platforms` | ✅ JWT + API Key | `PlatformCreateDto` | `201 PlatformReadDto` |
| `PUT` | `/api/platforms/{id}` | ✅ JWT + API Key | `PlatformUpdateDto` | `204 NoContent` |
| `DELETE` | `/api/platforms/{id}` | ✅ JWT + API Key | — | `204 NoContent` |

**PlatformCreateDto / PlatformUpdateDto:**
```json
{
  "name": "Linux",
  "publisher": "Linus Torvalds"
}
```

**PlatformReadDto:**
```json
{
  "id": 1,
  "name": "Linux",
  "publisher": "Linus Torvalds"
}
```

---

### ⌨️ Commands

| Method | Endpoint | Auth | Body | Response |
|--------|----------|------|------|----------|
| `GET` | `/api/commands` | ❌ None | — | `CommandReadDto[]` |
| `GET` | `/api/commands/{id}` | ❌ None | — | `CommandReadDto` |
| `GET` | `/api/commands/platform/{platformId}` | ❌ None | — | `CommandReadDto[]` |
| `POST` | `/api/commands` | ✅ JWT + API Key | `CommandCreateDto` | `201 CommandReadDto` |
| `PUT` | `/api/commands/{id}` | ✅ JWT + API Key | `CommandUpdateDto` | `204 NoContent` |
| `DELETE` | `/api/commands/{id}` | ✅ JWT + API Key | — | `204 NoContent` |

**CommandCreateDto / CommandUpdateDto:**
```json
{
  "howTo": "List all files",
  "commandLine": "ls -la",
  "platformId": 1
}
```

**CommandReadDto:**
```json
{
  "id": 1,
  "howTo": "List all files",
  "commandLine": "ls -la",
  "platformId": 1
}
```

---

### 📌 HTTP Status Codes

| Code | Meaning |
|------|---------|
| `200 OK` | Successful GET |
| `201 Created` | Successful POST — includes `Location` header |
| `204 No Content` | Successful PUT / DELETE |
| `401 Unauthorized` | Missing/invalid JWT or API key |
| `404 Not Found` | Resource doesn't exist |
| `500 Internal Server Error` | Server misconfiguration |

---

## 🔐 Authentication & Security Flow

```
Mutating Request (POST/PUT/DELETE)
          │
          ▼
  ┌──────────────────────────────────┐
  │      ApiKeyMiddleware            │
  │  Check: X-API-Key header         │
  │                                  │
  │  Missing/Invalid? → 401          │
  │  Valid? → continue               │
  └──────────────┬───────────────────┘
                 │
                 ▼
  ┌──────────────────────────────────┐
  │    JWT Bearer Authentication     │
  │  Check: Authorization: Bearer    │
  │                                  │
  │  [Authorize] endpoints only      │
  │  Invalid? → 401                  │
  │  Valid? → controller action      │
  └──────────────────────────────────┘

Getting a Token:
  POST /api/auth/token
     │  { username, password }
     ▼
  Check appsettings.json credentials
     │
     ▼
  JwtTokenService.GenerateToken(username)
     │  HMAC-SHA256, Issuer, Audience, Expiry
     ▼
  { accessToken, expiresAtUtc }
```

**Use token in subsequent requests:**
```
Authorization: Bearer eyJhbGciOiJIUzI1NiIsInR5cCI6IkpXVCJ9...
X-API-Key: your-api-key-here
```

---

## 🗄 Database Schema & Relationships

### Entity Relationship Diagram

```
┌──────────────────────────────┐         ┌──────────────────────────────────┐
│         platforms            │         │             commands              │
├──────────────────────────────┤         ├──────────────────────────────────┤
│ id          SERIAL  PK       │◄────────│ id           SERIAL  PK          │
│ name        VARCHAR(100) NN  │  1    N │ how_to       VARCHAR(200) NN     │
│ publisher   VARCHAR(100) NN  │         │ command_line  VARCHAR(200) NN     │
└──────────────────────────────┘         │ platform_id   INT FK → platforms │
                                         └──────────────────────────────────┘

Relationship:
  • Platform  1 ──────────── N  Command
  • One Platform has many Commands
  • Deleting a Platform CASCADE DELETES all its Commands
  • PlatformId is a required foreign key on Command
```

### Table Definitions (PostgreSQL)

```sql
-- platforms table
CREATE TABLE platforms (
    id        SERIAL PRIMARY KEY,
    name      VARCHAR(100) NOT NULL,
    publisher VARCHAR(100) NOT NULL
);

-- commands table
CREATE TABLE commands (
    id           SERIAL PRIMARY KEY,
    how_to       VARCHAR(200) NOT NULL,
    command_line VARCHAR(200) NOT NULL,
    platform_id  INTEGER NOT NULL
        REFERENCES platforms(id) ON DELETE CASCADE
);
```

---

## ⚙️ Configuration

### appsettings.json

```json
{
  "ConnectionStrings": {
    "Postgres": "enc:REPLACE_WITH_AES_ENCRYPTED_VALUE"
  },
  "Security": {
    "ApiKey": "REPLACE_WITH_STRONG_API_KEY",
    "AuthUser": {
      "Username": "admin",
      "Password": "ChangeMeNow!"
    },
    "Jwt": {
      "Issuer": "ApiBook",
      "Audience": "ApiBookClients",
      "SigningKey": "REPLACE_WITH_LONG_RANDOM_SIGNING_KEY_32PLUS_CHARS",
      "ExpiryMinutes": 60
    }
  },
  "Serilog": {
    "MinimumLevel": {
      "Default": "Information",
      "Override": {
        "Microsoft": "Warning",
        "Microsoft.AspNetCore": "Warning"
      }
    }
  }
}
```

### Environment Variables

| Variable | Required | Purpose |
|----------|----------|---------|
| `APIBOOK_CONN_ENCRYPTION_KEY` | ✅ Yes (if using encrypted conn str) | AES encryption key for DB connection string |
| `ASPNETCORE_ENVIRONMENT` | Optional | `Development` / `Production` |

---
## 🏛 Architecture Diagram

![Deployment Diagram](images/Deployment%20Diagram%20of%20API%20Book%20Project.png)

## 🚀 Getting Started

### Prerequisites

- [.NET 8 SDK](https://dotnet.microsoft.com/download/dotnet/8.0)
- [PostgreSQL 14+](https://www.postgresql.org/download/)
- [EF Core CLI tools](https://learn.microsoft.com/en-us/ef/core/cli/dotnet)

### 1. Clone the Repository

```bash
git clone https://github.com/yourusername/ApiBook.git
cd ApiBook
```

### 2. Set Environment Variable (for encrypted connection strings)

```bash
# Linux / macOS
export APIBOOK_CONN_ENCRYPTION_KEY="your-secret-key-here"

# Windows (PowerShell)
$env:APIBOOK_CONN_ENCRYPTION_KEY = "your-secret-key-here"

# Windows (CMD)
set APIBOOK_CONN_ENCRYPTION_KEY=your-secret-key-here
```

### 3. Configure appsettings

Update `ApiBook.Api/appsettings.json`:
- Set your PostgreSQL connection string (plain or encrypted — see below)
- Set a strong `ApiKey`
- Set `AuthUser` credentials
- Set a 32+ character `Jwt:SigningKey`

### 4. Apply Migrations

```bash
cd ApiBook.Api
dotnet ef database update --project ../ApiBook.Infrastructure
```

### 5. Run the API

```bash
dotnet run --project ApiBook.Api
```

API will be available at:
- HTTP: `http://localhost:5227`
- HTTPS: `https://localhost:7097`
- OpenAPI docs (dev): `https://localhost:7097/openapi/v1.json`

---

## 🗃 Migration Commands

> All migration commands must be run from the **solution root** or the `ApiBook.Api` directory. The `--startup-project` must point to `ApiBook.Api` and `--project` to `ApiBook.Infrastructure`.

### Install EF Core CLI (once)

```bash
dotnet tool install --global dotnet-ef
```

### Create a New Migration

```bash
dotnet ef migrations add <MigrationName> \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api \
  --output-dir Persistence/Migrations
```

**Example:**
```bash
dotnet ef migrations add InitialCreate \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api \
  --output-dir Persistence/Migrations
```

### Apply Migrations (Update Database)

```bash
dotnet ef database update \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api
```

### Apply a Specific Migration

```bash
dotnet ef database update <MigrationName> \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api
```

### Rollback Last Migration (Revert to Previous)

```bash
dotnet ef database update <PreviousMigrationName> \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api
```

### Remove Last (Unapplied) Migration

```bash
dotnet ef migrations remove \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api
```

### List All Migrations

```bash
dotnet ef migrations list \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api
```

### Generate SQL Script (without applying)

```bash
dotnet ef migrations script \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api \
  --output migration.sql
```

### Drop the Database

```bash
dotnet ef database drop \
  --project ApiBook.Infrastructure \
  --startup-project ApiBook.Api
```

> ⚠️ **Important:** `AppDbContextFactory` reads `appsettings.json` from the `ApiBook.Api` directory. Ensure `APIBOOK_CONN_ENCRYPTION_KEY` is set before running any EF commands if using an encrypted connection string.

---

## 🔒 Security: Encrypting Connection Strings

ApiBook supports AES-256-CBC encrypted connection strings to avoid storing plaintext DB credentials.

### Step 1: Set the encryption key

```bash
export APIBOOK_CONN_ENCRYPTION_KEY="my-super-secret-key"
```

### Step 2: Encrypt your connection string

Write a small .NET script or console app using `EncryptedConnectionStringResolver`:

```csharp
var resolver = new EncryptedConnectionStringResolver();
var encrypted = resolver.EncryptForCurrentUser(
    "Host=localhost;Database=apibook;Username=postgres;Password=secret");
Console.WriteLine(encrypted);
// Output: enc:BASE64_ENCODED_IV_AND_CIPHERTEXT
```

### Step 3: Paste into appsettings.json

```json
{
  "ConnectionStrings": {
    "Postgres": "enc:BASE64_ENCODED_IV_AND_CIPHERTEXT"
  }
}
```

### Development Override (plain text)

For local development, you can skip encryption entirely:

```json
{
  "ConnectionStrings": {
    "PostgresPlain": "Host=localhost;Database=apibook;Username=postgres;Password=dev"
  }
}
```

The resolver checks for `{name}Plain` first and uses it if present.

---

## 📝 Logging

Serilog is configured with:

| Sink | Output | Rolling |
|------|--------|---------|
| Console | All log events | — |
| File | `logs/apibook-YYYYMMDD.log` | Daily |

**Log levels (appsettings.json):**

| Namespace | Level |
|-----------|-------|
| Application | `Information` |
| Microsoft.* | `Warning` |
| Microsoft.AspNetCore.* | `Warning` |

Every HTTP request is logged automatically via `UseSerilogRequestLogging()` with method, path, status code, and duration.

---

## 🧰 Tech Stack

| Technology | Version | Purpose |
|-----------|---------|---------|
| ASP.NET Core | 8.0 | Web API framework |
| Entity Framework Core | 8.0 | ORM |
| Npgsql.EF Core | 8.x | PostgreSQL provider |
| PostgreSQL | 14+ | Database |
| Serilog | 3.x | Structured logging |
| Microsoft.IdentityModel.Tokens | 7.x | JWT validation |
| System.IdentityModel.Tokens.Jwt | 7.x | JWT generation |
| System.Security.Cryptography | Built-in | AES-256-CBC encryption |

---

## 📄 License

This project is licensed under the MIT License. See [LICENSE](LICENSE) for details.

---

<div align="center">

**Built with ❤️ using Clean Architecture principles**

*ApiBook — Where developer knowledge lives*

</div>


