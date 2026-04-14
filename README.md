# UserManagement — Itransition Task 4 (C#)

ASP.NET Core 8 MVC application with ASP.NET Core Identity for user registration, login, and admin-level user management (block / unblock / delete).

## Features

- **Registration** — name, email, password (min 1 char per spec)
- **Login** — with last-login timestamp update
- **User table** — sorted by last login (desc), shows name, email, registered date, last login, status
- **Block / Unblock / Delete** — toolbar actions on selected users (checkbox + select-all)
- **Auto-logout on block** — blocked user is redirected to login on next request (middleware)
- **Self-block / self-delete** → immediate sign-out + redirect to login
- **DB indexes** — on `Email`, `LastLoginAt`, `IsBlocked`
- Dark, responsive UI (Bootstrap 5 + custom CSS)

## Tech Stack

| Layer | Technology |
|---|---|
| Framework | ASP.NET Core 8 MVC |
| Auth | ASP.NET Core Identity |
| ORM | Entity Framework Core 8 |
| Database | PostgreSQL (via Npgsql) |
| Migrations | EF Core (auto-applied on startup) |
| Hosting | Railway (Docker) |

## Local Development

### Prerequisites
- .NET 8 SDK
- PostgreSQL running locally

### Run

```bash
# 1. Clone and enter project
git clone <your-repo-url>
cd UserManagement

# 2. Set connection string (edit appsettings.json OR use env var)
# appsettings.json → "DefaultConnection": "Host=localhost;Database=usermanagement;Username=postgres;Password=yourpassword"

# 3. Restore + run (migrations run automatically on startup)
cd UserManagement
dotnet run
```

Open http://localhost:5000

### Generate migrations manually (if you change models)

```bash
dotnet tool install --global dotnet-ef
dotnet ef migrations add <MigrationName>
dotnet ef database update
```

## Deploy to Railway

1. Push code to a public GitHub repository
2. Go to [railway.app](https://railway.app) → New Project → Deploy from GitHub
3. Add a **PostgreSQL** plugin in Railway
4. Set environment variable:
   ```
   ConnectionStrings__DefaultConnection=Host=<host>;Database=<db>;Username=<user>;Password=<pass>;SSL Mode=Require;Trust Server Certificate=true
   ```
5. Deploy — Railway uses the `Dockerfile` automatically

## Deploy to Render

1. Push to GitHub
2. New Web Service → connect repo
3. Environment: **Docker**
4. Add env var: `ConnectionStrings__DefaultConnection=...`
5. Add a **PostgreSQL** database in Render, copy the connection string

## Environment Variables

| Variable | Description |
|---|---|
| `ConnectionStrings__DefaultConnection` | PostgreSQL connection string |
| `ASPNETCORE_ENVIRONMENT` | `Production` or `Development` |

## Database Indexes

The following indexes are created by the migration:

| Index Name | Column | Purpose |
|---|---|---|
| `IX_Users_Email` | `Email` | Fast user lookup by email |
| `IX_Users_LastLoginAt` | `LastLoginAt` | Efficient sorting |
| `IX_Users_IsBlocked` | `IsBlocked` | Fast blocked-user filtering |

## Project Structure

```
UserManagement/
├── Controllers/
│   ├── AccountController.cs   # Register, Login, Logout
│   └── UsersController.cs     # Index, Block, Unblock, Delete
├── Data/
│   └── ApplicationDbContext.cs
├── Middleware/
│   └── BlockedUserMiddleware.cs
├── Migrations/
│   └── 20240101000000_InitialCreate.cs
├── Models/
│   ├── AppUser.cs
│   └── AccountViewModels.cs
├── Views/
│   ├── Account/
│   │   ├── Login.cshtml
│   │   └── Register.cshtml
│   ├── Users/
│   │   └── Index.cshtml
│   └── Shared/
│       └── _Layout.cshtml
├── wwwroot/
│   ├── css/site.css
│   └── js/site.js
├── Program.cs
├── appsettings.json
└── UserManagement.csproj
```
