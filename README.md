# FixMyCampus API

Backend API for **FixMyCampus**, a campus issue reporting and maintenance tracking system. Students and staff can report campus problems, while administrators and technicians manage tickets through a controlled workflow.

## Features

- JWT authentication with role-based authorization
- Reporter, administrator, and technician roles
- Campus ticket creation and tracking
- Building and status filtering
- Technician assignment by administrators
- Ticket status history with timestamps
- EF Core database migrations and seed data
- OpenAPI documentation with Scalar
- Centralized exception handling

## Technology Stack

- ASP.NET Core Web API
- .NET 10
- Entity Framework Core
- PostgreSQL
- Npgsql
- JWT Bearer authentication
- BCrypt password hashing
- Scalar OpenAPI UI

## Prerequisites

Install the following before running the API:

- .NET 10 SDK
- PostgreSQL
- `dotnet-ef` CLI tool

Install the EF Core CLI tool if needed:

```powershell
dotnet tool install --global dotnet-ef
```

## Database Configuration

The API reads its PostgreSQL connection string from `appsettings.json`:

```json
{
  "ConnectionStrings": {
    "DefaultConnection": "Host=localhost;Port=5433;Database=FixMyCampusDb;Username=postgres;Password=root"
  }
}
```

Update the host, port, database, username, and password to match your local PostgreSQL installation.

## Database Setup

From the project directory, create or apply the database schema:

```powershell
dotnet ef migrations add InitialCreate
dotnet ef database update
```

The application also checks for pending migrations during startup and seeds the initial users and buildings.

For a development-only database reset:

```powershell
dotnet ef database drop --force
dotnet ef database update
```

Do not use the database drop command where existing data must be preserved.

## Run the API

From the project directory:

```powershell
dotnet restore
dotnet build
dotnet run
```

The HTTP development profile runs at:

```text
http://localhost:5143
```

The HTTPS profile runs at:

```text
https://localhost:7051
```

Run the HTTPS profile explicitly with:

```powershell
dotnet run --launch-profile https
```

## API Documentation

With the API running, open Scalar:

```text
http://localhost:5143/scalar/v1
```

The OpenAPI document is available at:

```text
http://localhost:5143/openapi/v1.json
```

Authorize protected requests with:

```text
Bearer YOUR_JWT_TOKEN
```

## Seeded Accounts

These accounts are created automatically for development:

| Role | Email | Password |
|---|---|---|
| Admin | `admin@hackathon.local` | `Admin123!` |
| Reporter | `user@hackathon.local` | `User123!` |
| Reporter | `sara@hackathon.local` | `User123!` |
| Technician | `abebe@hackathon.local` | `Tech123!` |
| Technician | `marta@hackathon.local` | `Tech123!` |

Change these credentials before using the application outside development.

## Authentication

### Log in

```http
POST /api/Auth/login
Content-Type: application/json
```

```json
{
  "email": "user@hackathon.local",
  "password": "User123!"
}
```

The response contains a JWT token, user ID, full name, and role. Use the token in the `Authorization` header for protected endpoints.

## Endpoints

### Authentication

| Method | Route | Access | Description |
|---|---|---|---|
| `POST` | `/api/Auth/login` | Public | Authenticate a user and return a JWT token |

### Tickets

| Method | Route | Access | Description |
|---|---|---|---|
| `GET` | `/api/Tickets` | Admin, Reporter, Technician | List tickets with optional building and status filters |
| `GET` | `/api/Tickets/{id}` | Admin, Reporter, Technician | Get ticket details and status history |
| `GET` | `/api/Tickets/my` | Reporter | List tickets created by the current reporter |
| `POST` | `/api/Tickets` | Reporter | Create a new ticket |
| `PUT` | `/api/Tickets/{id}/assign` | Admin | Assign a new ticket to a technician |
| `PUT` | `/api/Tickets/{id}/status` | Admin, Technician | Move a ticket to the next valid status |
| `GET` | `/api/Tickets/buildings` | Admin, Reporter, Technician | List available buildings |
| `GET` | `/api/Tickets/technicians` | Admin | List available technicians |

### Ticket Filters

Filter tickets by building name or status:

```text
GET /api/Tickets?building=Engineering&status=New
```

Supported statuses:

```text
New
Assigned
InProgress
Resolved
```

## Ticket Workflow

Ticket statuses must follow this order:

```text
New -> Assigned -> InProgress -> Resolved
```

The API rejects skipped or backward transitions. A resolved ticket cannot be changed through the current status endpoint.

## Example Ticket Request

```http
POST /api/Tickets
Authorization: Bearer YOUR_JWT_TOKEN
Content-Type: application/json
```

```json
{
  "category": "Broken projector",
  "room": "Room 201",
  "description": "The projector is not displaying anything.",
  "buildingId": 1
}
```

New tickets are created with the `New` status automatically.

## Error Responses

The API returns JSON error responses for handled exceptions:

```json
{
  "statusCode": 400,
  "message": "Invalid status transition."
}
```

Common status codes:

| Status | Meaning |
|---|---|
| `200` | Request completed successfully |
| `201` | Resource created successfully |
| `400` | Invalid request or business rule violation |
| `401` | Missing or invalid authentication |
| `403` | Authenticated user lacks the required role |
| `404` | Resource was not found |
| `500` | Unexpected server error |

## Project Structure

```text
Controllers/       HTTP API controllers
Data/              EF Core DbContext and database seeding
DTOs/              API request and response models
Enums/             Roles and ticket statuses
Middleware/        Global exception handling
Models/            Entity models
Services/          Business logic and service interfaces
Program.cs         Application startup and dependency injection
```

## Development Notes

- Keep database migration execution in one startup location.
- Keep seed data in `DbSeeder`.
- Use role authorization on protected endpoints.
- Test both valid and invalid ticket status transitions.
- Do not commit real database passwords or production JWT keys.
