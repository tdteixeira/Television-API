# Television API

Welcome to the Television API — a local ASP.NET Core Web API project that supports user registration, login, and JWT-based authentication. Users are identified by their `username`, and passwords are securely hashed using HMACSHA512.

This guide will walk you through setting up the project, configuring the JWT key, running the database, and testing the API endpoints.

---

## Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)

---

## JWT Key Setup

This API uses a secret key to sign and validate JWT tokens. For security, the key is not stored directly in the codebase. You must set it manually before running the project.

### Generate a key
Generate a key using your prefered method.<br>
Example: 
```bash
openssl rand -hex 64
```

### Option 1: Temporary Environment Variable (Session-only)

#### Windows (PowerShell)

```powershell
$env:JWT_KEY = "your-super-secret-key"
dotnet run
```

#### Linux/macOS

```bash
export JWT_KEY="your-super-secret-key"
dotnet run
```

### Option 2: Persistent Environment Variable

#### Windows (PowerShell)

```powershell
[System.Environment]::SetEnvironmentVariable("JWT_KEY", "your-super-secret-key", "User")
```

#### Linux/macOS

```bash
export JWT_KEY="your-super-secret-key"
source ~/.bashrc
dotnet run
```

## Setup & Run

Clone the repository
```bash
git clone https://github.com/tdteixeira/Television-API.git
cd Television-API/Television\ API/
```
Restore dependencies
```bash
dotnet restore
```
Run project (don't forget the Jwt setup)
```bash
dotnet run
```

## Production
To use the release build just compile it with:
```bash
dotnet publish -c Release -o ./publish
```
And run the .exe (once again, don't forget Jwt setup)

---

## Testing Endpoints
Run the API in the Development environment and use Swagger UI to interact with the endpoints.

## Documentation
Provided on Swagger.
