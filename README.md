# AspireTodoApp

A production-ready Todo application built with .NET Aspire, ASP.NET Core Minimal APIs, PostgreSQL, and React + Vite.

## Stack

| | |
|---|---|
| Orchestration | .NET Aspire 13 |
| Backend | ASP.NET Core 10 Minimal APIs |
| Database | PostgreSQL + EF Core 10 |
| Cache | Redis |
| Email | Mailpit |
| Frontend | React 19 + Vite + TypeScript |
| Testing | xUnit + WebApplicationFactory |

## Getting Started

**Requirements:** .NET 10, Node 22, Aspire CLI, Docker Desktop

```bash
aspire run
```

This starts everything — PostgreSQL, Redis, Mailpit, the API, and the React frontend — and opens the Aspire dashboard.

```bash
dotnet test
```

## Deploying

```bash
aspire deploy     # Azure Container Apps
aspire publish    # Docker Compose
```

## Inspired By

[davidfowl/TodoApp](https://github.com/davidfowl/TodoApp)
