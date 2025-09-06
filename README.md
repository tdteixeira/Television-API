# 📺 Television API

Welcome to the Television API — a local ASP.NET Core Web API project that supports user registration, login, and JWT-based authentication. Users are identified by their `username`, and passwords are securely hashed using HMACSHA512.

This guide will walk you through setting up the project, configuring the JWT key, running the database, and testing the API endpoints.

---

## 🔧 Requirements

- [.NET SDK 8.0](https://dotnet.microsoft.com/en-us/download/dotnet/8.0)
- SQLite (included via EF Core)
- PowerShell (Windows) or Bash (Linux/macOS)

---

## 🔐 JWT Key Setup

This API uses a secret key to sign and validate JWT tokens. For security, the key is not stored directly in the codebase. You must set it manually before running the project.

### ✅ Option 1: Temporary Environment Variable (Session-only)

#### Windows (PowerShell)

```powershell
$env:JWT_KEY = "your-super-secret-key"
dotnet run
```

or for persistent ENV_VAR

```powershell
[System.Environment]::SetEnvironmentVariable("JWT_KEY", "your-super-secret-key", "User")
```

#### Linux/macOS

```bash
export JWT_KEY="your-super-secret-key"
dotnet run
```
